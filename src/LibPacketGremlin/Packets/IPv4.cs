﻿// -----------------------------------------------------------------------
//  <copyright file="IPv4.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.DataTypes;
    using OutbreakLabs.LibPacketGremlin.Extensions;
    using OutbreakLabs.LibPacketGremlin.Packets.IPv4Support;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    /// IPv4 is a connectionless protocol for use on packet-switched networks
    /// </summary>
    public abstract class IPv4 : IPacket
    {
        private const int MinimumParseableBytes = 20;

        private readonly BitVector32.Section FragOffSection = BitVector32.CreateSection(8191);

        protected BitVector32 flagsAndFragOff;

        private IPacket payload;

        /// <summary>
        /// Gets or sets the version of IP. This should always be 4.
        /// </summary>
        public Nibble Version { get; set; }

        /// <summary>
        /// Gets or sets the number of 32-bit words in the header.
        /// </summary>
        public Nibble HeaderLength { get; set; }

        /// <summary>
        /// Gets or sets a value used for packet classification purposes.
        /// </summary>
        public byte DifferentiatedServices { get; set; }

        /// <summary>
        /// Gets or sets the entire packet size, including header and data, in bytes.
        /// </summary>
        public UInt16 TotalLength { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier, primarily used for fragmentation.
        /// </summary>
        public UInt16 ID { get; set; }

        private BitVector32 FlagsAndFragOff
        {
            get
            {
                return this.flagsAndFragOff;
            }
            set
            {
                this.flagsAndFragOff = value;
            }
        }

        /// <summary>
        /// Gets or sets the reserved flag, which should always be zero.
        /// </summary>
        public bool ReservedFlag
        {
            get
            {
                return this.flagsAndFragOff[32768];
            }
            set
            {
                this.flagsAndFragOff[32768] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Don't Fragment flag. False is "May Fragment", True is "Don't Fragment".
        /// </summary>
        public bool DontFragmentFlag 
        {
            get
            {
                return this.flagsAndFragOff[16384];
            }
            set
            {
                this.flagsAndFragOff[16384] = value;
            }
        }

        /// <summary>
        /// Gets or sets the More Fragments flag. False is "Last Fragment", True is "More Fragments".
        /// </summary>
        public bool MoreFragmentsFlag
        {
            get
            {
                return this.flagsAndFragOff[8192];
            }
            set
            {
                this.flagsAndFragOff[8192] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the offset of a particular fragment 
        /// relative to the beginning of the original unfragmented IP datagram,
        /// in units of eight-byte blocks.
        /// </summary>
        public int FragOff
        {
            get
            {
                return this.flagsAndFragOff[this.FragOffSection];
            }
            set
            {
                this.flagsAndFragOff[this.FragOffSection] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value limiting a datagram's lifetime.
        /// </summary>
        public byte TTL { get; set; }

        /// <summary>
        /// Gets or sets a value defining the protocol used in the data portion of the IP datagram.
        /// </summary>
        public byte Protocol { get; set; }

        /// <summary>
        /// Gets or sets a 16-bit checksum used for error-checking of the header
        /// </summary>
        public UInt16 HeaderChecksum { get; set; }

        /// <summary>
        /// Gets or sets the sender of the packet
        /// </summary>
        public IPv4Address SourceAddress { get; set; }

        /// <summary>
        /// Gets or sets the destination of the packet
        /// </summary>
        public IPv4Address DestAddress { get; set; }

        /// <summary>
        /// Gets or sets a byte array containing rarely-used options, and padding needed to ensure the header contains an integer number of 32-bit words.
        /// </summary>
        public byte[] OptionsAndPadding { get; set; }

        /// <summary>
        /// Gets the payload contained within this packet
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
        }

        /// <summary>
        ///     Gets the length of the packet
        /// </summary>
        /// <returns>Length of the packet</returns>
        public long Length()
        {
            return 20 + this.OptionsAndPadding.Length + this.Payload.Length();
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write((byte)((this.Version << 4) | this.HeaderLength));
                bw.Write(this.DifferentiatedServices);
                bw.Write(ByteOrder.HostToNetworkOrder(this.TotalLength));
                bw.Write(ByteOrder.HostToNetworkOrder(this.ID));
                bw.Write(ByteOrder.HostToNetworkOrder((UInt16)this.flagsAndFragOff.Data)); //Highly suspect
                bw.Write(this.TTL);
                bw.Write(this.Protocol);
                bw.Write(ByteOrder.HostToNetworkOrder(this.HeaderChecksum));
                bw.Write(this.SourceAddress.GetAddressBytes()); //Is the endianness proper?
                bw.Write(this.DestAddress.GetAddressBytes()); //Is the endianness proper?
                bw.Write(this.OptionsAndPadding);
            }
            this.Payload.WriteToStream(stream);
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
            this.Version = 4;
            if (this.OptionsAndPadding.Length % 4 != 0)
            {
                var OnPtemp = new byte[(this.OptionsAndPadding.Length + 3) & ~3];
                Array.Copy(this.OptionsAndPadding, OnPtemp, OnPtemp.Length);
                this.OptionsAndPadding = OnPtemp;
            }
            this.HeaderLength = 5 + this.OptionsAndPadding.Length / 4;
            this.TotalLength = (ushort)(this.HeaderLength * 4 + this.Payload.Length());

            // TODO: Add ICMP when ported over
            // TODO: Add TCP when ported over
            if (this.Payload is IPv4)
            {
                this.Protocol = (byte)Protocols.IP;
            }
            else if (this.Payload is UDP)
            {
                this.Protocol = (byte)Protocols.UDP;
            }

            this.HeaderChecksum = 0;
            this.HeaderChecksum = ComputeChecksum(this.ToArray(), 0, this.HeaderLength * 4);
            this.Payload.CorrectFields();
        }

        /// <summary>
        /// Compute the checksum of an IPv4 Header
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
                word16 = (ushort)(((evenLength[i] << 8) & 0xFF00) + (evenLength[i + 1] & 0xFF));
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
        /// <param name="data">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public static bool TryParse(byte[] data, out IPv4 packet)
        {
            try
            {
                if (data.Length < MinimumParseableBytes)
                {
                    packet = null;
                    return false;
                }

                using (var ms = new MemoryStream(data))
                {
                    using (var br = new BinaryReader(ms))
                    {
                        var versionAndHeaderLen = br.ReadByte();
                        var version = versionAndHeaderLen >> 4;
                        var headerLength = versionAndHeaderLen & 0x0F;

                        var differentiatedServices = br.ReadByte();
                        var totalLength = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        var id = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        var flagsAndFragOff = new BitVector32(ByteOrder.NetworkToHostOrder(br.ReadUInt16()));
                        var ttl = br.ReadByte();
                        var protocol = br.ReadByte();
                        var headerChecksum = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        var sourceAddress = new IPv4Address(br.ReadBytes(4));
                        var destAddress = new IPv4Address(br.ReadBytes(4));

                        if (headerLength == 0 || (headerLength * 32 / 8 > data.Length))
                        {
                            // Specified header length is larger than available bytes
                            packet = null;
                            return false;
                        }

                        byte[] optionsAndPadding;
                        if (headerLength * 32 / 8 < br.BaseStream.Position)
                        {
                            optionsAndPadding = br.ReadBytes(headerLength * 32 / 8 - (int)br.BaseStream.Position);
                        }
                        else
                        {
                            optionsAndPadding = Array.Empty<byte>();
                        }

                        br.BaseStream.Seek(headerLength * 32 / 8, SeekOrigin.Begin);

                        byte[] payloadBytes;
                        
                        // TODO: Accept option for IgnoreLength
                        if (true /*IgnoreLength*/)
                        {
                            payloadBytes = br.ReadBytes(data.Length - (int)br.BaseStream.Position);
                        }
                        /*else
                        {
                            payloadBytes = br.ReadBytes(totalLength - (headerLength * 32 / 8));
                        }*/

                        switch (protocol)
                        {
                            // TODO: Add ICMP when ported over
                            case (byte)Protocols.UDP:
                                {
                                    UDP payload;
                                    if (UDP.TryParse(payloadBytes, out payload))
                                    {
                                        var newPacket = new IPv4<UDP>();
                                        newPacket.Payload = payload;
                                        packet = newPacket;
                                    }
                                    else
                                    {
                                        Generic fallbackPayload;
                                        Generic.TryParse(payloadBytes, out fallbackPayload);
                                        // This can never fail, so I'm not checking the output

                                        var newPacket = new IPv4<Generic>();
                                        newPacket.Payload = fallbackPayload;
                                        packet = newPacket;
                                    }
                                }

                                break;
                            // TODO: Add TCP when ported over

                            default:
                                {
                                    Generic payload;
                                    Generic.TryParse(payloadBytes, out payload);
                                    // This can never fail, so I'm not checking the output
                                    var newPacket = new IPv4<Generic>();
                                    newPacket.Payload = payload;
                                    packet = newPacket;
                                }
                                break;
                        }

                        packet.Version = version;
                        packet.HeaderLength = headerLength;
                        packet.DifferentiatedServices = differentiatedServices;
                        packet.TotalLength = totalLength;
                        packet.ID = id;
                        packet.FlagsAndFragOff = flagsAndFragOff;
                        packet.TTL = ttl;
                        packet.Protocol = protocol;
                        packet.HeaderChecksum = headerChecksum;
                        packet.SourceAddress = sourceAddress;
                        packet.DestAddress = destAddress;
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
    /// IPv4 is a connectionless protocol for use on packet-switched networks
    /// </summary>
    /// <typeparam name="PayloadType"></typeparam>
    public class IPv4<PayloadType> : IPv4
        where PayloadType : class, IPacket
    {
        /// <summary>
        ///     Constructs an IPv4 packet with default values, and the specified payload
        /// </summary>
        /// <param name="payload">Packet payload</param>
        public IPv4(PayloadType payload)
        {
            this.Version = 4;
            this.HeaderLength = 5;
            this.DifferentiatedServices = 0;
            this.TotalLength = 0;
            this.ID = 0;
            this.flagsAndFragOff = new BitVector32(0);
            this.TTL = 0;
            this.Protocol = 0;
            this.HeaderChecksum = 0;
            this.OptionsAndPadding = new byte[0];
            this.SourceAddress = new IPv4Address(0);
            this.DestAddress = new IPv4Address(0);
            this.Payload = payload;
        }

        /// <summary>
        ///     Constructs an IPv4 packet with no defaults. All fields must be set, or things will explode.
        /// </summary>
        internal IPv4()
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