// -----------------------------------------------------------------------
//  <copyright file="Generic.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.IO;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;

    /// <summary>
    ///     A generic packet for storing a blob of data
    /// </summary>
    public class Generic : IPacket
    {
        private byte[] buffer;

        /// <summary>
        ///     Constructs a generic unstructured packet
        /// </summary>
        internal Generic()
        {
        }

        /// <summary>
        ///     Gets or sets the data of this packet
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                return this.buffer;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                this.buffer = value;
            }
        }

        /// <summary>
        ///     Get the payload, if applicable
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
            return this.Buffer.Length;
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write(this.Buffer);
            }
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(byte[] buffer, int index, int count, out Generic packet)
        {
            try
            {
                var data = new byte[count];
                Array.Copy(buffer, index, data, 0, count);
                packet = new Generic { Buffer = data };
                return true;
            }
            catch (Exception)
            {
                packet = null;
                return false;
            }
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(ReadOnlySpan<byte> buffer, out Generic packet)
        {
            try
            {
                var data = buffer.ToArray();
                packet = new Generic { Buffer = data };
                return true;
            }
            catch (Exception)
            {
                packet = null;
                return false;
            }
        }
    }
}