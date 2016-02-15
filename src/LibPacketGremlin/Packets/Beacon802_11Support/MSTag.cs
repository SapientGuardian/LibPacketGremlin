// -----------------------------------------------------------------------
//  <copyright file="MSTag.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.Beacon802_11Support
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;

    /// <summary>
    /// Microsoft-specific management tags
    /// </summary>
    public class MSTag : IManagementTag
    {
        /// <summary>
        ///     The minimum number of bytes required for a successful parse
        /// </summary>
        private const int MinimumParseableBytes = 8;
        
        private List<MSSubTag> tags;

        /// <summary>
        ///     Constructs an uninitialized tag
        /// </summary>
        internal MSTag()
        {
            this.Tags = new List<MSSubTag>();
        }

        /// <summary>
        /// Gets or sets the length of the tag data
        /// </summary>
        public byte TagLength { get; set; }

        /// <summary>
        /// Microsoft's vendor identifier
        /// </summary>
        public byte[] OUI => new byte[] { 0x00, 0x50, 0xF2 };

        /// <summary>
        /// Gets or sets the vendor tag type
        /// </summary>
        public byte VendorType { get; set; }

        /// <summary>
        /// Gets or sets the version of the tag
        /// </summary>
        public UInt16 Version { get; set; }

        /// <summary>
        /// Gets or sets a list of sub-tags
        /// </summary>
        public List<MSSubTag> Tags
        {
            get
            {
                return this.tags;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                this.tags = value;
            }
        }

        /// <summary>
        /// Vendor tag type
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
            return 8 + (this.Tags?.Sum(tag => tag.Length()) ?? 0);
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
                bw.Write(this.VendorType);
                bw.Write(this.Version); //htons?
            }
            foreach (var tag in this.Tags)
            {
                tag.WriteToStream(stream);
            }
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
            this.TagLength = (byte)(3 + 1 + 2 + this.Tags.Sum(t => t.Length()));
            foreach (var tag in this.Tags)
            {
                tag.CorrectFields();
            }
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="tag">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(byte[] buffer, int index, int count, out MSTag tag)
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
                        tag = new MSTag();
                        var tagTypeRead = br.ReadByte();
                        if (tagTypeRead != 0xdd)
                        {
                            tag = null;
                            return false;
                        }
                        tag.TagLength = br.ReadByte();
                        var readOUI = br.ReadBytes(3);
                        if (!readOUI.SequenceEqual(new byte[] { 0x00, 0x50, 0xF2 }))
                        {
                            tag = null;
                            return false;
                        }
                        tag.VendorType = br.ReadByte();
                        tag.Version = br.ReadUInt16(); //htons?
                        while (ms.Position <= count - 3)
                        {
                            MSSubTag newTag;
                            var tagType = br.ReadUInt16();
                            var tagLength = br.ReadByte();
                            var safeLength = Math.Min(tagLength, count - (int)br.BaseStream.Position);
                            //switch (tagType)
                            //{
                            //    default:
                            //        newTag = new MSSubTag();
                            //        break;
                            //}

                            if (!MSSubTag.TryParse(buffer, (int)br.BaseStream.Position - 3, safeLength, out newTag))
                            {
                                tag = null;
                                return false;
                            }
                            br.BaseStream.Seek(safeLength, SeekOrigin.Current);

                            tag.Tags.Add(newTag);
                        }
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