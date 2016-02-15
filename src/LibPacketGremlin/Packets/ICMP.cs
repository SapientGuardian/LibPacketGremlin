// -----------------------------------------------------------------------
//  <copyright file="ICMP.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.IO;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Extensions;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    ///     Internet Control Message Protocol
    /// </summary>
    public class ICMP : IPacket
    {
        /// <summary>
        ///     The minimum number of bytes required for a successful parse
        /// </summary>
        private const int MinimumParseableBytes = 8;

        /// <summary>
        ///     Constructs an uninitialized packet
        /// </summary>
        internal ICMP()
        {
        }

        /// <summary>
        ///     Gets or sets the ICMP type
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        ///     Gets or sets the subtype to the given type.
        /// </summary>
        public byte Code { get; set; }

        /// <summary>
        ///     Gets or sets error checking data. Calculated from the ICMP header+data, with value 0 for this field. The algorithm
        ///     is the same as the header checksum for IPv4.
        /// </summary>
        public UInt16 Checksum { get; set; }

        public UInt16 ID { get; set; }

        /// <summary>
        ///     Gets or sets a value that specifies the operation that the sender is performing: 1 for request, 2 for reply.
        /// </summary>
        public UInt16 Sequence { get; set; }

        /// <summary>
        ///     Gets or sets the data specific to the message type indicated by the Type and Code fields.
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
            return 8 + (this.Data?.Length ?? 0);
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write(this.Type);
                bw.Write(this.Code);

                bw.Write(ByteOrder.HostToNetworkOrder(this.Checksum));
                bw.Write(ByteOrder.HostToNetworkOrder(this.ID));
                bw.Write(ByteOrder.HostToNetworkOrder(this.Sequence));

                bw.Write(this.Data);
            }
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
            this.Checksum = 0;
            var thisAsBytes = this.ToArray();
            this.Checksum = IPv4.ComputeChecksum(thisAsBytes, 0, thisAsBytes.Length);
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(byte[] buffer, int index, int count, out ICMP packet)
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
                        packet = new ICMP();
                        packet.Type = br.ReadByte();
                        packet.Code = br.ReadByte();
                        packet.Checksum = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        packet.ID = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        packet.Sequence = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        packet.Data = br.ReadBytes(count - 8);

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