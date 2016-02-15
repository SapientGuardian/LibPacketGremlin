// -----------------------------------------------------------------------
//  <copyright file="RC4.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Utilities
{
    using System;

    /// <summary>
    /// Implementation of RC4 encryption
    /// </summary>
    /// <remarks>Taken from http://www.idrix.fr/Root/Samples/rc4_derive_dotnet.cs</remarks>
    public class RC4
    {
        protected byte[] m_key = new byte[256];

        protected byte m_x;

        protected byte m_y;

        public RC4(byte[] keyData)
        {
            this.Reset(keyData, 0, keyData.Length);
        }

        public RC4(byte[] keyData, int offset, int length)
        {
            this.Reset(keyData, offset, length);
        }

        ~RC4()
        {
            // Clear sensitive information
            Array.Clear(this.m_key, 0, 256);
        }

        public void Reset(byte[] keyData, int offset, int length)
        {
            var i = 0;
            byte indx1 = 0, indx2 = 0;
            for (i = 0; i <= 255; i++)
            {
                this.m_key[i] = (byte)i;
            }

            for (i = 0; i <= 255; i++)
            {
                var tmp = this.m_key[i];
                indx2 += (byte)(keyData[indx1] + tmp);
                this.m_key[i] = this.m_key[indx2];
                this.m_key[indx2] = tmp;

                if (++indx1 == length)
                {
                    indx1 = 0;
                }
            }

            this.m_x = 0;
            this.m_y = 0;
        }

        public void encrypt(byte[] inData, int length, byte[] outData)
        {
            for (var index = 0; index < length; index++)
            {
                byte sx, sy;
                sx = this.m_key[++this.m_x];
                this.m_y += sx;
                sy = this.m_key[this.m_y];
                this.m_key[this.m_y] = sx;
                this.m_key[this.m_x] = sy;

                outData[index] = (byte)(inData[index] ^ this.m_key[(sx + sy) & 0xFF]);
            }
        }

        public void decrypt(byte[] inData, int length, byte[] outData)
        {
            this.encrypt(inData, length, outData);
        }
    }
}