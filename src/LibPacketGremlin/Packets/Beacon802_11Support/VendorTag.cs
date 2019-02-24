// -----------------------------------------------------------------------
//  <copyright file="VendorTag.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.Beacon802_11Support
{
    using System;
    using System.IO;
    using System.Text;
    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    /// Placeholder for unsupported vendor tags
    /// </summary>
    public class VendorTag : IManagementTag
    {
        /// <summary>
        ///     The minimum number of bytes required for a successful parse
        /// </summary>
        private const int MinimumParseableBytes = 5;

        private byte[] oui;

        private byte[] tagData;

        /// <summary>
        ///     Constructs an uninitialized tag
        /// </summary>
        internal VendorTag()
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
        /// Gets or sets the vendor identifier
        /// </summary>
        public byte[] OUI
        {
            get
            {
                return this.oui;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (value.Length != 3)
                {
                    throw new ArgumentException("This field must be exactly 3 bytes in length", nameof(value));
                }
                this.oui = value;
            }
        }

        /// <summary>
        /// The vendor tag type
        /// </summary>
        public byte TagType => 0xdd;

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
            return 5 + (this.TagData?.Length ?? 0);
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
                bw.Write(this.OUI);
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
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(ReadOnlySpan<byte> buffer, out VendorTag tag)
        {
            try
            {
                if (buffer.Length < MinimumParseableBytes)
                {
                    tag = null;
                    return false;
                }

                var br = new SpanReader(buffer);
                tag = new VendorTag();
                var tagType = br.ReadByte();
                if (tagType != 0xdd)
                {
                    tag = null;
                    return false;
                }
                tag.TagLength = br.ReadByte();
                tag.OUI = br.ReadBytes(3);
                tag.TagData = br.ReadBytes(Math.Min(tag.TagLength, buffer.Length - 5));
                return true;

            }
            catch (Exception)
            {
                tag = null;
                return false;
            }
        }
    }
}