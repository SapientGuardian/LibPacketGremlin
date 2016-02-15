// -----------------------------------------------------------------------
//  <copyright file="IEEE802_11Crypto.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Utilities
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Extensions;
    using OutbreakLabs.LibPacketGremlin.PacketFactories;
    using OutbreakLabs.LibPacketGremlin.Packets;
    using OutbreakLabs.LibPacketGremlin.Packets.IEEE802_11Support;

    /// <summary>
    /// Functions for working with 802.11 cryptography
    /// </summary>
    public class IEEE802_11Crypto
    {
        /// <summary>
        /// Modes in which decryption can handle headers
        /// </summary>
        public enum DecryptionHeaderMode
        {
            Untouched,

            Modified,

            Removed
        }

        /// <summary>
        /// Decrypt a packet encrypted with CCMP
        /// </summary>
        /// <param name="encryptedPacket">The encrypted packet</param>
        /// <param name="temporalKey">The temporal key</param>
        /// <param name="headerMode">Mode in which headers should be handled</param>
        /// <param name="decrypted">Decrypted packet</param>
        /// <returns>True if successful, false if not</returns>
        /// <remarks>Ported from airdecap-ng</remarks>
        public static bool TryDecryptCCMP(
            IEEE802_11 encryptedPacket,
            byte[] temporalKey,
            DecryptionHeaderMode headerMode,
            out IPacket decrypted)
        {
            if (temporalKey.Length != 16)
            {
                // throw new ArgumentException("temporalKey must be 16 bytes");
                decrypted = null;
                return false;
            }

            var decryptedBytes = encryptedPacket.ToArray();
            int z, data_len, blocks, last, offset;
            bool is_a4, is_qos;

            var B0 = new byte[16];
            var B = new byte[16];
            var MIC = new byte[16];
            var PacketNumber = new byte[6];
            var AdditionalAuthData = new byte[32];

            is_a4 = (decryptedBytes[1] & 3) == 3;
            is_qos = (decryptedBytes[0] & 0x8C) == 0x88;
            z = 24 + 6 * (is_a4 ? 1 : 0);
            z += 2 * (is_qos ? 1 : 0);

            PacketNumber[0] = decryptedBytes[z + 7];
            PacketNumber[1] = decryptedBytes[z + 6];
            PacketNumber[2] = decryptedBytes[z + 5];
            PacketNumber[3] = decryptedBytes[z + 4];
            PacketNumber[4] = decryptedBytes[z + 1];
            PacketNumber[5] = decryptedBytes[z + 0];

            data_len = decryptedBytes.Length - z - 8 - 8;

            B0[0] = 0x59;
            B0[1] = 0;
            Array.Copy(decryptedBytes, 10, B0, 2, 6);
            Array.Copy(PacketNumber, 0, B0, 8, 6);
            B0[14] = (byte)((data_len >> 8) & 0xFF);
            B0[15] = (byte)(data_len & 0xFF);

            AdditionalAuthData[2] = (byte)(decryptedBytes[0] & 0x8F);
            AdditionalAuthData[3] = (byte)(decryptedBytes[1] & 0xC7);
            Array.Copy(decryptedBytes, 4, AdditionalAuthData, 4, 3 * 6);
            AdditionalAuthData[22] = (byte)(decryptedBytes[22] & 0x0F);

            if (is_a4)
            {
                Array.Copy(decryptedBytes, 24, AdditionalAuthData, 24, 6);

                if (is_qos)
                {
                    AdditionalAuthData[30] = (byte)(decryptedBytes[z - 2] & 0x0F);
                    AdditionalAuthData[31] = 0;
                    B0[1] = AdditionalAuthData[30];
                    AdditionalAuthData[1] = 22 + 2 + 6;
                }
                else
                {
                    AdditionalAuthData[30] = 0;
                    AdditionalAuthData[31] = 0;

                    B0[1] = 0;
                    AdditionalAuthData[1] = 22 + 6;
                }
            }
            else
            {
                if (is_qos)
                {
                    AdditionalAuthData[24] = (byte)(decryptedBytes[z - 2] & 0x0F);
                    AdditionalAuthData[25] = 0;
                    B0[1] = AdditionalAuthData[24];
                    AdditionalAuthData[1] = 22 + 2;
                }
                else
                {
                    AdditionalAuthData[24] = 0;
                    AdditionalAuthData[25] = 0;

                    B0[1] = 0;
                    AdditionalAuthData[1] = 22;
                }
            }

            using (var aesFactory = Aes.Create())
            {
                aesFactory.Mode = CipherMode.ECB;
                aesFactory.Key = temporalKey;
                var aes = aesFactory.CreateEncryptor();

                aes.TransformBlock(B0, 0, B0.Length, MIC, 0);
                CFunctions.XOR(MIC, 0, AdditionalAuthData, 16);
                aes.TransformBlock(MIC, 0, MIC.Length, MIC, 0);
                CFunctions.XOR(MIC, 0, AdditionalAuthData.Skip(16).ToArray(), 16);
                aes.TransformBlock(MIC, 0, MIC.Length, MIC, 0);

                B0[0] &= 0x07;
                B0[14] = 0;
                B0[15] = 0;
                aes.TransformBlock(B0, 0, B0.Length, B, 0);
                CFunctions.XOR(decryptedBytes, decryptedBytes.Length - 8, B, 8);

                blocks = (data_len + 16 - 1) / 16;
                last = data_len % 16;
                offset = z + 8;

                for (var i = 1; i <= blocks; i++)
                {
                    var n = last > 0 && i == blocks ? last : 16;

                    B0[14] = (byte)((i >> 8) & 0xFF);
                    B0[15] = (byte)(i & 0xFF);

                    aes.TransformBlock(B0, 0, B0.Length, B, 0);
                    CFunctions.XOR(decryptedBytes, offset, B, n);
                    CFunctions.XOR(MIC, 0, decryptedBytes.Skip(offset).ToArray(), n);
                    aes.TransformBlock(MIC, 0, MIC.Length, MIC, 0);

                    offset += n;
                }
            }

            switch (headerMode)
            {
                case DecryptionHeaderMode.Removed:
                    {
                        decryptedBytes[1] &= 191; // Remove the protected bit
                        var decryptedBytesList = decryptedBytes.ToList();
                        decryptedBytesList.RemoveRange(34 - 8, 8);
                            // Remove CCMP Parameters (otherwise, without the protected bit it will break the parsing)                        
                        IEEE802_11 decryptedWithHeader;
                        if (IEEE802_11Factory.Instance.TryParse(decryptedBytesList.ToArray(), out decryptedWithHeader))
                        {
                            decrypted = decryptedWithHeader.Payload; // Remove the altered 802.11 header
                        }
                        else
                        {
                            decrypted = null;
                            return false;
                        }

                        break;
                    }
                case DecryptionHeaderMode.Modified:
                    {
                        decryptedBytes[1] &= 191; // Remove the protected bit
                        var decryptedBytesList = decryptedBytes.ToList();
                        decryptedBytesList.RemoveRange(34 - 8, 8);
                            // Remove CCMP Parameters (otherwise, without the protected bit it will break the parsing)                        
                        if (!IEEE802_11Factory.Instance.TryParse(decryptedBytesList.ToArray(), out decrypted))
                        {
                            return false;
                        }
                        break;
                    }
                case DecryptionHeaderMode.Untouched:
                    if (!IEEE802_11Factory.Instance.TryParse(decryptedBytes.ToArray(), out decrypted))
                    {
                        return false;
                    }
                    break;

                default:
                    decrypted = null;
                    return false;
            }

            return CFunctions.memcmp(decryptedBytes.Skip(offset).Take(8).ToArray(), MIC.Take(8).ToArray()) == 0;
        }

        /// <summary>
        /// Calculates a pairwise master key
        /// </summary>
        /// <param name="psk">Pre-shared key</param>
        /// <param name="ssid">SSID</param>
        /// <param name="pmk_len">Length of PMK to generate</param>
        /// <returns>Pairwise master key</returns>
        public static byte[] CalculatePMK(string psk, string ssid, int pmk_len = 32)
        {
            return CalculatePMK(Encoding.UTF8.GetBytes(psk), Encoding.UTF8.GetBytes(ssid), pmk_len);
        }

        /// <summary>
        /// Calculates a pairwise master key
        /// </summary>
        /// <param name="psk">Pre-shared key</param>
        /// <param name="ssid">SSID</param>
        /// <param name="pmk_len">Length of PMK to generate</param>
        /// <returns>Pairwise master key</returns>
        public static byte[] CalculatePMK(byte[] psk, string ssid, int pmk_len = 32)
        {
            return CalculatePMK(psk, Encoding.UTF8.GetBytes(ssid), pmk_len);
        }

        /// <summary>
        /// Calculates a pairwise master key
        /// </summary>
        /// <param name="psk">Pre-shared key</param>
        /// <param name="ssid">SSID</param>
        /// <param name="pmk_len">Length of PMK to generate</param>
        /// <returns>Pairwise master key</returns>
        public static byte[] CalculatePMK(byte[] psk, byte[] ssid, int pmk_len = 32)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(psk, /*ssid*/new byte[8], 4096);
            // This reflection crap is required because there's an arbitrary restriction that the salt must be at least 8 bytes
            var saltProp = pbkdf2.GetType().GetField("m_salt", BindingFlags.NonPublic | BindingFlags.Instance);

            saltProp.SetValue(pbkdf2, ssid);

            // pbkdf2.Reset(); //To officially complete the reflection trick, the private method Initialize() should be called. That's all Reset() does. 
            // But I don't think it's needed because we haven't hashed anything yet.

            return pbkdf2.GetBytes(pmk_len);
        }

        /// <summary>
        /// Calculate a pairwise transit key
        /// </summary>
        /// <param name="pmk">Pairwise master key</param>
        /// <param name="stmac">Station MAC address</param>
        /// <param name="bssid">BSSID</param>
        /// <param name="snonce">S-nonce from EAPOL</param>
        /// <param name="anonce">A-nonce from EAPOL</param>
        /// <returns>Pairwise transit key</returns>
        public static byte[] CalculatePTK(
            byte[] pmk,
            byte[] stmac,
            byte[] bssid,
            byte[] snonce,
            byte[] anonce /*, int keyVer*/)
        {
            var pke = new byte[100];
            var ptk = new byte[80];

            using (var ms = new MemoryStream(pke))
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(
                        new byte[]
                            {
                                0x50, 0x61, 0x69, 0x72, 0x77, 0x69, 0x73, 0x65, 0x20, 0x6b, 0x65, 0x79, 0x20, 0x65, 0x78,
                                0x70, 0x61, 0x6e, 0x73, 0x69, 0x6f, 0x6e, 0
                            });
                    /* Literally the string Pairwise key expansion, with a trailing 0*/

                    if (CFunctions.memcmp(stmac, bssid) < 0)
                    {
                        bw.Write(stmac);
                        bw.Write(bssid);
                    }
                    else
                    {
                        bw.Write(bssid);
                        bw.Write(stmac);
                    }

                    if (CFunctions.memcmp(snonce, anonce) < 0)
                    {
                        bw.Write(snonce);
                        bw.Write(anonce);
                    }
                    else
                    {
                        bw.Write(anonce);
                        bw.Write(snonce);
                    }

                    bw.Write((byte)0); // Will be swapped out on each round in the loop below
                }
            }

            for (byte i = 0; i < 4; i++)
            {
                pke[99] = i;
                var hmacsha1 = new HMACSHA1(pmk);
                var hash = hmacsha1.ComputeHash(pke);
                hash.CopyTo(ptk, i * 20);
            }

            return ptk;
        }

        /// <summary>
        /// Decrypt a WEP-encrypted packet
        /// </summary>
        /// <param name="encryptedPacket">WEP encrypted packet</param>
        /// <param name="key">Decryption key</param>
        /// <param name="decrypted">Decrypted packet</param>
        /// <returns>True if successful, false if not</returns>
        public static bool TryDecryptWEP(IEEE802_11 encryptedPacket, byte[] key, out IPacket decrypted)
        {
            if (key.Length != 5 && key.Length != 13 && key.Length != 16 && key.Length != 29 && key.Length != 61)
            {
                //throw new ArgumentException("Invalid WEP key length. [5,13,16,29,61]");
                decrypted = null;
                return false;
            }

            if (encryptedPacket.FrameType != (int)FrameTypes.Data || !encryptedPacket.IsWep)
            {
                decrypted = null;
                return false;
            }

            var keyWithIV = new byte[key.Length + 3];
            Array.Copy(encryptedPacket.CCMP_WEP_Data, keyWithIV, 3);
            Array.Copy(key, 0, keyWithIV, 3, key.Length);

            var rc4 = new RC4(keyWithIV);
            var encryptedPayload =
                encryptedPacket.Payload.ToArray()
                    .Concat(BitConverter.GetBytes(ByteOrder.NetworkToHostOrder(encryptedPacket.WEP_ICV)))
                    .ToArray();
            var dec = new byte[encryptedPayload.Length];
            rc4.decrypt(encryptedPayload, encryptedPayload.Length, dec);

            var expectedCRC = BitConverter.ToUInt32(dec, dec.Length - 4);
            dec = dec.Take(dec.Length - 4).ToArray();

            if (Crc32Algorithm.Compute(dec) == expectedCRC)
            {
                var decryptedBytes =
                    encryptedPacket.ToArray().Take(24 + (encryptedPacket.QosControl?.Length ?? 0)).Concat(dec).ToArray();
                decryptedBytes[1] &= 191; //Everything except protected
                return IEEE802_11Factory.Instance.TryParse(decryptedBytes, out decrypted);
            }
            decrypted = null;
            return false;
        }

        /// <summary>
        ///     Checks whether the specified pairwise transit key is valid for the EAPoL key
        /// </summary>
        /// <param name="eapolKey">The EAPoL key to check against</param>
        /// <param name="ptk">The pairwise transit key to check</param>
        /// <returns>True if the key is valid, false if it is not</returns>
        public static bool PtkIsValid(IEEE802_1x<EapolKey> eapolKey, byte[] ptk)
        {
            var tkip = (eapolKey.Payload.KeyInformation & 7) == 1;
            bool isValid;
            var MIC = eapolKey.Payload.MIC;
            eapolKey.Payload.MIC = new byte[MIC.Length];

            try
            {
                HMAC hmac;
                var key = ptk.Take(16).ToArray();

                if (tkip)
                {
#if DOTNET5_4
                    throw new System.Exception("The current version of System.Security.Cryptography.Algorithms is missing HMACMD5.");
#else
                    hmac = new HMACMD5(key);
#endif
                }
                else
                {
                    hmac = new HMACSHA1(key);
                }

                var computedHash = hmac.ComputeHash(eapolKey.ToArray());
                isValid = computedHash.Take(16).SequenceEqual(MIC.Take(16));
            }
            finally
            {
                eapolKey.Payload.MIC = MIC;
            }

            return isValid;
        }
    }
}