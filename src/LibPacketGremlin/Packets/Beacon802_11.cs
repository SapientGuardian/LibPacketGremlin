// -----------------------------------------------------------------------
//  <copyright file="Beacon802_11.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets.Beacon802_11Support;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    /// 802.11 beacon, containing information about a network
    /// </summary>
    public class Beacon802_11 : IPacket
    {
        /// <summary>
        ///     The minimum number of bytes required for a successful parse
        /// </summary>
        private const int MinimumParseableBytes = 12;

        private byte[] beaconInterval;

        /// <summary>
        ///     Constructs an uninitialized packet
        /// </summary>
        internal Beacon802_11()
        {
            this.Tags = new List<IManagementTag>();
        }

        /// <summary>
        /// Gets or sets the time used for synchronization
        /// </summary>
        public UInt64 Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the time interval between beacon transmissions
        /// </summary>
        public byte[] BeaconInterval
        {
            get
            {
                return this.beaconInterval;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (value.Length != 2)
                {
                    throw new ArgumentException("This field must be exactly 2 bytes in length", nameof(value));
                }
                this.beaconInterval = value;
            }
        }

        /// <summary>
        /// Gets or sets a value announcing support for polling, encryption details, and type of network
        /// </summary>
        public UInt16 Capabilities { get; set; }

        /// <summary>
        /// Gets or sets tags bearing additional information about the network
        /// </summary>
        public List<IManagementTag> Tags { get; set; }

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
            return 12 + this.Tags.Sum(tag => tag.Length());
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write(this.Timestamp);
                bw.Write(this.BeaconInterval);
                bw.Write(this.Capabilities);
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
            foreach (var tag in this.Tags)
            {
                tag.CorrectFields();
            }
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(byte[] buffer, int index, int count, out Beacon802_11 packet)
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
                        packet = new Beacon802_11();

                        packet.Timestamp = br.ReadUInt64();
                        packet.BeaconInterval = br.ReadBytes(2);
                        packet.Capabilities = br.ReadUInt16();

                        while (br.BaseStream.Position <= count - 2)
                        {
                            IManagementTag newTag = null;
                            var tagType = br.ReadByte();
                            var tagLength = br.ReadByte();
                            var safeLength = Math.Min(tagLength, count - (int)br.BaseStream.Position);
                            switch (tagType)
                            {
                                case (byte)ManagementTagTypes.SSID:
                                    {
                                        SSIDTag tag;
                                        if (SSIDTag.TryParse(
                                            buffer,
                                            index + (int)br.BaseStream.Position - 2,
                                            2 + safeLength,
                                            out tag))
                                        {
                                            newTag = tag;
                                        }
                                    }
                                    break;
                                case (byte)ManagementTagTypes.VendorSpecific:
                                    {
                                        /* We'll ask each vendor to parse. Not the most efficient solution.
                                        MSTag msTag;
                                        if (MSTag.TryParse(
                                            buffer,
                                            index + (int)br.BaseStream.Position - 2,
                                            2 + safeLength,
                                            out msTag))
                                        {
                                            newTag = msTag;
                                        }
                                        else
                                        {*/
                                        VendorTag tag;
                                        if (VendorTag.TryParse(
                                            buffer,
                                            index + (int)br.BaseStream.Position - 2,
                                            2 + safeLength,
                                            out tag))
                                        {
                                            newTag = tag;
                                        }
                                        /*}*/
                                    }
                                    break;
                            }

                            if (newTag == null)
                            {
                                UnsupportedTag tag;
                                UnsupportedTag.TryParse(
                                    buffer,
                                    index + (int)br.BaseStream.Position - 2,
                                    2 + safeLength,
                                    out tag);
                                newTag = tag;
                            }
                            br.BaseStream.Seek(safeLength, SeekOrigin.Current);
                            packet.Tags.Add(newTag);
                        }

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