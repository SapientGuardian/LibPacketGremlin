// -----------------------------------------------------------------------
//  <copyright file="UDP.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Extensions;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    /// User Datagram Protocol
    /// </summary>
    public abstract class UDP : IPacket
    {
        private const int MinimumParseableBytes = 8;

        private IPacket container;

        private IPacket payload;

        /// <summary>
        /// Gets or sets the sender's port.
        /// </summary>
        public UInt16 SourcePort { get; set; }

        /// <summary>
        /// Gets or sets the receiver's port.
        /// </summary>
        public UInt16 DestPort { get; set; }

        /// <summary>
        /// Gets or sets the length in bytes of the UDP header and UDP data.
        /// </summary>
        public UInt16 TotalLength { get; set; }

        /// <summary>
        /// Gets or sets a checksum used for error-checking of the header and data. This field is optional in IPv4, and mandatory in IPv6.
        /// </summary>
        public UInt16 Checksum { get; set; }

        /// <summary>
        /// Gets the payload contained within this packet.
        /// </summary>
        public IPacket Payload
        {
            get
            {
                return this.payload;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                this.payload = value;
                value.SetContainer(this);
            }
        }

        /// <summary>
        ///     Set the enclosing packet
        /// </summary>
        /// <param name="container">Packet that contains this one</param>
        public void SetContainer(IPacket container)
        {
            this.container = container;
        }

        /// <summary>
        ///     Gets the length of the packet
        /// </summary>
        /// <returns>Length of the packet</returns>
        public long Length()
        {
            return 8 + this.Payload.Length();
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write(ByteOrder.HostToNetworkOrder(this.SourcePort));
                bw.Write(ByteOrder.HostToNetworkOrder(this.DestPort));
                bw.Write(ByteOrder.HostToNetworkOrder(this.TotalLength));
                bw.Write(ByteOrder.HostToNetworkOrder(this.Checksum));
            }
            this.Payload.WriteToStream(stream);
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
            this.TotalLength = (UInt16)(8 + (UInt16)this.Payload.Length());

            var ipv4container = this.container as IPv4;
            if (ipv4container != null)
            {
                this.Checksum = 0;
                var IPv4PseudoHeaderPlusUDP = new byte[12 + this.TotalLength + (this.TotalLength % 2 == 0 ? 0 : 1)];

                Array.Copy(ipv4container.SourceAddress.GetAddressBytes(), 0, IPv4PseudoHeaderPlusUDP, 0, 4);
                Array.Copy(ipv4container.DestAddress.GetAddressBytes(), 0, IPv4PseudoHeaderPlusUDP, 4, 4);
                //[8] should be 0
                IPv4PseudoHeaderPlusUDP[9] = ipv4container.Protocol;
                Array.Copy(
                    BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16)this.TotalLength)),
                    0,
                    IPv4PseudoHeaderPlusUDP,
                    10,
                    2);
                Array.Copy(this.ToArray(), 0, IPv4PseudoHeaderPlusUDP, 12, this.TotalLength);
                this.Checksum = ComputeChecksum(IPv4PseudoHeaderPlusUDP, 0, IPv4PseudoHeaderPlusUDP.Length);
            }
            this.Payload.CorrectFields();
        }

        /// <summary>
        /// Calculates a checksum used for error-checking of the header and data.
        /// </summary>
        /// <param name="header">Byte array containing the header</param>
        /// <param name="start">Starting position in the header array that actually begins the header</param>
        /// <param name="length">Length of the header</param>
        /// <returns>16-bit checksum</returns>
        public static ushort ComputeChecksum(byte[] header, int start, int length)
        {
            var evenLength = new int[length + length % 2];
            Array.Copy(header, start, evenLength, 0, length);

            ushort word16;
            long sum = 0;
            for (var i = 0; i < length - 1; i += 2)
            {
                word16 = (ushort)(((header[i] << 8) & 0xFF00) + (header[i + 1] & 0xFF));
                sum += word16;
            }

            while (sum >> 16 != 0)
            {
                sum = (sum & 0xFFFF) + (sum >> 16);
            }

            sum = ~sum;

            return (ushort)sum;
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>        
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public static bool TryParse(byte[] buffer, int index, int count, out UDP packet)
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
                        var sourcePort = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        var destPort = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        var totalLength = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        var checksum = ByteOrder.NetworkToHostOrder(br.ReadUInt16());

                        Generic payload;
                        Generic.TryParse(buffer, index + (int)br.BaseStream.Position, count - 8, out payload);
                        // This can never fail, so I'm not checking the output
                        var newPacket = new UDP<Generic>();
                        newPacket.Payload = payload;
                        packet = newPacket;

                        packet.SourcePort = sourcePort;
                        packet.DestPort = destPort;
                        packet.TotalLength = totalLength;
                        packet.Checksum = checksum;

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

    /// <summary>
    /// User Datagram Protocol
    /// </summary>
    public class UDP<PayloadType> : UDP
        where PayloadType : class, IPacket
    {
        /// <summary>
        ///     Constructs a packet with no defaults. All fields must be set, or things will explode.
        /// </summary>
        internal UDP()
        {
        }

        /// <summary>
        /// Gets or sets the payload contained within this packet
        /// </summary>
        public new PayloadType Payload
        {
            get
            {

                // Should never be null or of the wrong type, because the base Setter makes it so.
                return (PayloadType)base.Payload;
            }

            set
            {
                base.Payload = value;
            }
        }
    }
}