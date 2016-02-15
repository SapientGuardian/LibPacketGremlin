// -----------------------------------------------------------------------
//  <copyright file="TCP.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.DataTypes;
    using OutbreakLabs.LibPacketGremlin.Extensions;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    ///     Transmission Control Protocol
    /// </summary>
    public abstract class TCP : IPacket
    {
        private const int MinimumParseableBytes = 20;
        
        protected byte[] _OptionsAndPadding;
        
        private IPacket container;

        protected BitVector32 Flags;

        private IPacket payload;

        /// <summary>
        ///     Gets or sets the source port number
        /// </summary>
        public UInt16 SourcePort { get; set; }

        /// <summary>
        ///     Gets or sets the destination port number
        /// </summary>
        public UInt16 DestPort { get; set; }

        /// <summary>
        ///     Gets or sets the sequence number of the first data octet in this segment (except when SYN is present). If SYN is
        ///     present the sequence number is the initial sequence number (ISN) and the first data octet is ISN+1.
        /// </summary>
        public UInt32 SeqNumber { get; set; }

        /// <summary>
        ///     If the ACK control bit is set this field contains the value of the next sequence number the sender of the segment
        ///     is expecting to receive. Once a connection is established this is always sent.
        /// </summary>
        public UInt32 AckNumber { get; set; }

        /// <summary>
        ///     This field contains the number of 32 bit words in the TCP Header. This indicates where the data begins. The TCP
        ///     header (even one including options) is an integral number of 32 bits long.
        /// </summary>
        public Nibble DataOffset { get; set; }

        /// <summary>
        ///     This field is reserved for future use, and must be zero.
        /// </summary>
        public Nibble Reserved { get; set; }

        /// <summary>
        ///     Congestion Window Reduced (CWR) flag is set by the sending host to indicate that it received a TCP segment with the
        ///     ECE flag set and had responded in congestion control mechanism (added to header by RFC 3168).
        /// </summary>
        public bool CongestionWindowReduced
        {
            get
            {
                return this.Flags[128];
            }
            set
            {
                this.Flags[128] = value;
            }
        }

        /// <summary>
        ///     ECN-Echo indicates: If the SYN flag is set (1), that the TCP peer is ECN capable. If the SYN flag is clear (0),
        ///     that a packet with Congestion Experienced flag in IP header set is received during normal transmission (added to
        ///     header by RFC 3168).
        /// </summary>
        public bool ECN_Echo
        {
            get
            {
                return this.Flags[64];
            }
            set
            {
                this.Flags[64] = value;
            }
        }

        /// <summary>
        ///     Indicates that the Urgent pointer field is significant.
        /// </summary>
        public bool Urgent
        {
            get
            {
                return this.Flags[32];
            }
            set
            {
                this.Flags[32] = value;
            }
        }

        /// <summary>
        ///     Indicates that the Acknowledgment field is significant. All packets after the initial SYN packet sent by the client
        ///     should have this flag set.
        /// </summary>
        public bool Ack
        {
            get
            {
                return this.Flags[16];
            }
            set
            {
                this.Flags[16] = value;
            }
        }

        /// <summary>
        ///     Push function. Asks to push the buffered data to the receiving application.
        /// </summary>
        public bool Push
        {
            get
            {
                return this.Flags[8];
            }
            set
            {
                this.Flags[8] = value;
            }
        }

        /// <summary>
        ///     Reset the connection.
        /// </summary>
        public bool Reset
        {
            get
            {
                return this.Flags[4];
            }
            set
            {
                this.Flags[4] = value;
            }
        }

        /// <summary>
        ///     Synchronize sequence numbers. Only the first packet sent from each end should have this flag set. Some other flags
        ///     change meaning based on this flag, and some are only valid for when it is set, and others when it is clear.
        /// </summary>
        public bool Syn
        {
            get
            {
                return this.Flags[2];
            }
            set
            {
                this.Flags[2] = value;
            }
        }

        /// <summary>
        ///     Indicates no more data from sender.
        /// </summary>
        public bool Fin
        {
            get
            {
                return this.Flags[1];
            }
            set
            {
                this.Flags[1] = value;
            }
        }

        /// <summary>
        ///     Indicates the size of the receive window, which specifies the number of bytes (beyond the sequence number in the
        ///     acknowledgment field) that the receiver is currently willing to receive.
        /// </summary>
        public UInt16 WindowSize { get; set; }

        /// <summary>
        ///     The 16-bit checksum field is used for error-checking of the header and data.
        /// </summary>
        public UInt16 Checksum { get; set; }

        /// <summary>
        ///     If the URG control bit is set, then this 16-bit field is an offset from the sequence number indicating the last
        ///     urgent data byte.
        /// </summary>
        public UInt16 UrgentPointer { get; set; }

        /// <summary>
        ///     The length of this field is determined by the data offset field. Options 0 and 1 are a single byte in length. The
        ///     remaining options indicate the total length of the option in the second byte.
        /// </summary>
        public byte[] OptionsAndPadding
        {
            get
            {
                return this._OptionsAndPadding;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                this._OptionsAndPadding = value;
            }
        }

        /// <summary>
        ///     Gets the payload contained within this packet.
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
            return 20 + (this.OptionsAndPadding?.Length ?? 0) + this.Payload.Length();
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
                bw.Write(ByteOrder.HostToNetworkOrder(this.SeqNumber));
                bw.Write(ByteOrder.HostToNetworkOrder(this.AckNumber));
                bw.Write((byte)(this.DataOffset << 4 | this.Reserved));
                bw.Write((byte)this.Flags.Data);
                bw.Write(ByteOrder.HostToNetworkOrder(this.WindowSize));
                bw.Write(ByteOrder.HostToNetworkOrder(this.Checksum));
                bw.Write(ByteOrder.HostToNetworkOrder(this.UrgentPointer));
                bw.Write(this.OptionsAndPadding);
            }
            this.Payload.WriteToStream(stream);
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
            if (this.OptionsAndPadding.Length % 4 != 0)
            {
                var OnPtemp = new byte[(this.OptionsAndPadding.Length + 3) & ~3];
                Array.Copy(this.OptionsAndPadding, OnPtemp, OnPtemp.Length);
                this.OptionsAndPadding = OnPtemp;
            }
            this.DataOffset = 5 + this.OptionsAndPadding.Length / 4;
            var TotalLength = (ushort)(this.DataOffset * 4 + this.Payload.ToArray().Length);

            if (this.container is IPv4)
            {
                this.Checksum = 0;
                var IPv4PseudoHeaderPlusUDP = new byte[12 + TotalLength];
                var typedContainer = (IPv4<TCP>)this.container;
                Array.Copy(typedContainer.SourceAddress.GetAddressBytes(), 0, IPv4PseudoHeaderPlusUDP, 0, 4);
                Array.Copy(typedContainer.DestAddress.GetAddressBytes(), 0, IPv4PseudoHeaderPlusUDP, 4, 4);
                IPv4PseudoHeaderPlusUDP[9] = typedContainer.Protocol;
                Array.Copy(
                    BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16)TotalLength)),
                    0,
                    IPv4PseudoHeaderPlusUDP,
                    10,
                    2);
                Array.Copy(this.ToArray(), 0, IPv4PseudoHeaderPlusUDP, 12, TotalLength);
                this.Checksum = ComputeChecksum(IPv4PseudoHeaderPlusUDP, 0, IPv4PseudoHeaderPlusUDP.Length);
            }

            this.Payload.CorrectFields();
        }

        /// <summary>
        ///     Calculates a checksum used for error-checking of the header and data.
        /// </summary>
        /// <param name="header">Byte array containing the header</param>
        /// <param name="start">Starting position in the header array that actually begins the header</param>
        /// <param name="length">Length of the header</param>
        /// <returns>16-bit checksum</returns>
        internal static ushort ComputeChecksum(byte[] header, int start, int length)
        {
            ushort word16;
            long sum = 0;
            for (var i = start; i < length + start; i += 2)
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
        internal static bool TryParse(byte[] buffer, int index, int count, out TCP packet)
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
                        var seqNumber = ByteOrder.NetworkToHostOrder(br.ReadUInt32());
                        var ackNumber = ByteOrder.NetworkToHostOrder(br.ReadUInt32());
                        var dataOffsetAndReserved = br.ReadByte();
                        var dataOffset = dataOffsetAndReserved >> 4;
                        var reserved = dataOffsetAndReserved & 0x0F;
                        var flags = new BitVector32(br.ReadByte());
                        var windowSize = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        var checksum = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        var urgentPointer = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        byte[] optionsAndPadding = null;

                        if (dataOffset > 5)
                        {
                            if (br.BaseStream.Position + (dataOffset - 5) * 4 > count)
                            {
                                // throw new ArgumentException("Header specifies more bytes than available");
                                packet = null;
                                return false;
                            }
                            optionsAndPadding = br.ReadBytes((dataOffset - 5) * 4);
                        }

                        Generic payload;
                        Generic.TryParse(
                            buffer,
                            index + (int)br.BaseStream.Position,
                            count - dataOffset * 32 / 8,
                            out payload);
                        // This can never fail, so I'm not checking the output
                        var newPacket = new TCP<Generic>();
                        newPacket.Payload = payload;
                        packet = newPacket;

                        packet.SourcePort = sourcePort;
                        packet.DestPort = destPort;
                        packet.SeqNumber = seqNumber;
                        packet.AckNumber = ackNumber;
                        packet.DataOffset = dataOffset;
                        packet.Reserved = reserved;
                        packet.Flags = flags;
                        packet.WindowSize = windowSize;
                        packet.Checksum = checksum;
                        packet.UrgentPointer = urgentPointer;
                        packet.OptionsAndPadding = optionsAndPadding;

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
    ///     Transmission Control Protocol
    /// </summary>
    public class TCP<PayloadType> : TCP
        where PayloadType : class, IPacket
    {
        /// <summary>
        ///     Constructs a packet with no defaults. All fields must be set, or things will explode.
        /// </summary>
        internal TCP()
        {
        }

        /// <summary>
        ///     Gets or sets the payload contained within this packet
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