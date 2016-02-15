// -----------------------------------------------------------------------
//  <copyright file="MSSubTag.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.Beacon802_11Support
{
    using System;
    using System.IO;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;

    /// <summary>
    /// Microsoft-specific sub-tags that can be embedded in MSTags
    /// </summary>
    public class MSSubTag : IPacket
    {
        /// <summary>
        ///     The minimum number of bytes required for a successful parse
        /// </summary>
        private const int MinimumParseableBytes = 3;

        private byte[] tagData;

        /// <summary>
        ///     Constructs an uninitialized tag
        /// </summary>
        internal MSSubTag()
        {
        }

        /// <summary>
        /// Gets or sets the length of the tag
        /// </summary>
        public byte TagLength { get; set; }

        /// <summary>
        /// Gets or sets the unstructured tag data
        /// </summary>
        public byte[] TagData
        {
            get
            {
                return this.tagData;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                this.tagData = value;
            }
        }

        /// <summary>
        /// Gets or sets the Microsoft-specific tag type
        /// </summary>
        public UInt16 TagType { get; set; }

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
            return 3 + (this.TagData?.Length ?? 0);
        }

        public IPacket Payload => null;

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write(this.TagType);
                bw.Write(this.TagLength);
                bw.Write(this.TagData);
            }
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
            this.TagLength = (byte)this.TagData.Length;
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="tag">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(byte[] buffer, int index, int count, out MSSubTag tag)
        {
            try
            {
                if (count < MinimumParseableBytes)
                {
                    tag = null;
                    return false;
                }

                using (var ms = new MemoryStream(buffer, index, count, false))
                {
                    using (var br = new BinaryReader(ms))
                    {
                        tag = new MSSubTag();
                        tag.TagType = br.ReadUInt16();
                        tag.TagLength = br.ReadByte();
                        tag.TagData = br.ReadBytes(Math.Min(tag.TagLength, count - 3));
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                tag = null;
                return false;
            }
        }
    }
}