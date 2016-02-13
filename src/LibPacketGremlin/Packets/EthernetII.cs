// -----------------------------------------------------------------------
//  <copyright file="EthernetII.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.IO;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets.EthernetIISupport;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    /// A frame on an Ethernet link
    /// </summary>
    public abstract class EthernetII : IPacket
    {
        private const int MinimumParseableBytes = 14;

        private byte[] dstMac;

        private IPacket payload;

        private byte[] srcMac;

        /// <summary>
        ///     Hardware (MAC) address of the destination.
        /// </summary>
        public byte[] DstMac
        {
            get
            {
                return this.dstMac;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (value.Length != 6)
                {
                    throw new ArgumentException("MAC addresses must be exactly 6 bytes in length", nameof(value));
                }
                this.dstMac = value;
            }
        }

        /// <summary>
        ///     Hardware (MAC) address of the source.
        /// </summary>
        public byte[] SrcMac
        {
            get
            {
                return this.srcMac;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (value.Length != 6)
                {
                    throw new ArgumentException("MAC addresses must be exactly 6 bytes in length", nameof(value));
                }
                this.srcMac = value;
            }
        }

        /// <summary>
        /// EtherType is a two-octet field in an Ethernet frame. It is used to indicate which protocol is encapsulated in the payload of an Ethernet Frame. EtherType numbering generally starts from 0x0800.
        /// </summary>
        public UInt16 EtherType { get; set; }

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
            return 14 + this.Payload.Length();
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write(this.DstMac, 0, 6);
                bw.Write(this.SrcMac, 0, 6);
                bw.Write(ByteOrder.HostToNetworkOrder(this.EtherType));
            }

            this.Payload.WriteToStream(stream);
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
            if (this.Payload is IPv4)
            {
                this.EtherType = (UInt16)EtherTypes.IPv4;
            }
            else if (Payload is ARP)
            {
                this.EtherType = (UInt16)EtherTypes.ARP;
            }
            else if (Payload is WakeOnLan)
            {
                EtherType = (UInt16)EtherTypes.WakeOnLAN;
            }
            
            this.Payload.CorrectFields();
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>        
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(byte[] buffer, int index, int count, out EthernetII packet)
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
                        var dstMac = br.ReadBytes(6);
                        var srcMac = br.ReadBytes(6);
                        var etherType = ByteOrder.NetworkToHostOrder(br.ReadUInt16());

                        packet = null;
                        switch (etherType)
                        {

                            case (ushort)EtherTypes.IPv4:
                                {
                                    IPv4 payload;
                                    if (IPv4.TryParse(buffer, index + (int)br.BaseStream.Position, (int)(count - br.BaseStream.Position), out payload))
                                    {
                                        packet = new EthernetII<IPv4> { Payload = payload };                                        
                                    }
                                }
                                break;
                            case (ushort)EtherTypes.ARP:
                                {
                                    ARP payload;
                                    if (ARP.TryParse(buffer, index + (int)br.BaseStream.Position, (int)(count - br.BaseStream.Position), out payload))
                                    {
                                        packet = new EthernetII<ARP> { Payload = payload };                                        
                                    }
                                }
                                break;

                        }

                        if (packet == null)
                        {
                            Generic payload;
                            Generic.TryParse(buffer, index + (int)br.BaseStream.Position, (int)(count - br.BaseStream.Position), out payload);
                            
                            // This can never fail, so I'm not checking the output
                            packet = new EthernetII<Generic> { Payload = payload };                            
                        }

                        packet.DstMac = dstMac;
                        packet.EtherType = etherType;
                        packet.SrcMac = srcMac;

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
    /// A frame on an Ethernet link
    /// </summary>
    public class EthernetII<PayloadType> : EthernetII
        where PayloadType : class, IPacket
    {


        /// <summary>
        ///     Constructs an uninitialized packet.
        /// </summary>
        internal EthernetII()
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