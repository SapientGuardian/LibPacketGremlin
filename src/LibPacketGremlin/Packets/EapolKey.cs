// -----------------------------------------------------------------------
//  <copyright file="EapolKey.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.IO;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    /// 802.1x key exchange information
    /// </summary>
    public class EapolKey : IPacket
    {
        /// <summary>
        ///     The minimum number of bytes required for a successful parse
        /// </summary>
        private const int MinimumParseableBytes = 95;

        private byte[] _ID; //8

        private byte[] _IV; //16

        private byte[] _MIC; //16

        private byte[] _Nonce; //32

        private byte[] _ReplayCounter; //8

        private byte[] _RSC; //8

        /// <summary>
        ///     Constructs an uninitialized packet
        /// </summary>
        internal EapolKey()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating the flavor of key descriptor (e.g. WPA)
        /// </summary>
        public byte KeyDescriptorType { get; set; }

        /// <summary>
        /// Gets or sets information about the key type
        /// </summary>
        public UInt16 KeyInformation { get; set; }

        /// <summary>
        /// Gets or sets the length of the key in bytes
        /// </summary>
        public UInt16 KeyLength { get; set; }

        /// <summary>
        /// Gets or sets a value used to detect replay attacks
        /// </summary>
        public byte[] ReplayCounter
        {
            get
            {
                return this._ReplayCounter;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (value.Length != 8)
                {
                    throw new ArgumentException("This field must be exactly 8 bytes in length", nameof(value));
                }
                this._ReplayCounter = value;
            }
        }

        /// <summary>
        /// Gets or sets a value used to derive keys
        /// </summary>
        public byte[] Nonce
        {
            get
            {
                return this._Nonce;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (value.Length != 32)
                {
                    throw new ArgumentException("This field must be exactly 32 bytes in length", nameof(value));
                }
                this._Nonce = value;
            }
        }

        /// <summary>
        /// Gets or sets the initialization vector
        /// </summary>
        public byte[] IV
        {
            get
            {
                return this._IV;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (value.Length != 16)
                {
                    throw new ArgumentException("This field must be exactly 16 bytes in length", nameof(value));
                }
                this._IV = value;
            }
        }

        /// <summary>
        /// Gets or sets the expected sequence number
        /// </summary>
        public byte[] RSC
        {
            get
            {
                return this._RSC;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (value.Length != 8)
                {
                    throw new ArgumentException("This field must be exactly 8 bytes in length", nameof(value));
                }
                this._RSC = value;
            }
        }

        /// <summary>
        /// Gets or sets the key identifier
        /// </summary>
        public byte[] ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (value.Length != 8)
                {
                    throw new ArgumentException("This field must be exactly 8 bytes in length", nameof(value));
                }
                this._ID = value;
            }
        }

        /// <summary>
        /// Gets or sets the material integrity check value
        /// </summary>
        public byte[] MIC
        {
            get
            {
                return this._MIC;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (value.Length != 16)
                {
                    throw new ArgumentException("This field must be exactly 16 bytes in length", nameof(value));
                }
                this._MIC = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of bytes in the Data field
        /// </summary>
        public UInt16 DataLen { get; set; }

        /// <summary>
        /// Gets or sets the key data
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        ///     Gets the payload contained within this packet
        /// </summary>
        public IPacket Payload => null;

        /// <summary>
        ///     Set the enclosing packet
        /// </summary>
        /// <param name="container">Packet that contains this one</param>
        public void SetContainer(IPacket container)
        {
        }

        /// <summary>
        ///     Gets the length of the packet
        /// </summary>
        /// <returns>Length of the packet</returns>
        public long Length()
        {
            return 5 + 8 + 32 + 16 + 8 + 8 + 16 + 2 + (this.Data?.Length ?? 0);
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write(this.KeyDescriptorType);
                bw.Write(ByteOrder.HostToNetworkOrder(this.KeyInformation));
                bw.Write(ByteOrder.HostToNetworkOrder(this.KeyLength));
                bw.Write(this.ReplayCounter);
                bw.Write(this.Nonce);
                bw.Write(this.IV);
                bw.Write(this.RSC);
                bw.Write(this.ID);
                bw.Write(this.MIC);
                bw.Write(ByteOrder.HostToNetworkOrder(this.DataLen));
                bw.Write(this.Data);
            }
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
            this.DataLen = (ushort)(this.Data?.Length ?? 0);
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(byte[] buffer, int index, int count, out EapolKey packet)
        {
            try
            {
                if (count < MinimumParseableBytes)
                {
                    packet = null;
                    return false;
                }

                using (var ms = new MemoryStream(buffer, index, count, false))
                {
                    using (var br = new BinaryReader(ms))
                    {
                        packet = new EapolKey();
                        packet.KeyDescriptorType = br.ReadByte();
                        packet.KeyInformation = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        packet.KeyLength = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        packet.ReplayCounter = br.ReadBytes(8);
                        packet.Nonce = br.ReadBytes(32);
                        packet.IV = br.ReadBytes(16);
                        packet.RSC = br.ReadBytes(8);
                        packet.ID = br.ReadBytes(8);
                        packet.MIC = br.ReadBytes(16);
                        packet.DataLen = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        packet.Data = br.ReadBytes((int)(count - br.BaseStream.Position));

                        return true;
                    }
                }
            }
            catch (Exception)
            {
                packet = null;
                return false;
            }
        }
    }
}