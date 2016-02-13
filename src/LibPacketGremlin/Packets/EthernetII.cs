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
            // TODO: Uncomment when these types are ported over
            /*else if (Payload is OutbreakLabs.PacketGremlin.PacketGremlinCore.Packets.ApplicationLayer.WakeOnLAN)
            {
                EtherType = (UInt16)EtherTypes.WakeOnLAN;
            }*/
            this.Payload.CorrectFields();
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="data">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public static bool TryParse(byte[] data, out EthernetII packet)
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
                        var dstMac = br.ReadBytes(6);
                        var srcMac = br.ReadBytes(6);
                        var etherType = ByteOrder.NetworkToHostOrder(br.ReadUInt16());

                        var payloadBytes = br.ReadBytes((int)(data.Length - br.BaseStream.Position));

                        switch (etherType)
                        {
                            
                            case (ushort)EtherTypes.IPv4:
                                {
                                    IPv4 payload;
                                    if (IPv4.TryParse(payloadBytes, out payload))
                                    {
                                        var newPacket = new EthernetII<IPv4>();
                                        newPacket.Payload = payload;
                                        packet = newPacket;
                                    }
                                    else
                                    {
                                        Generic fallbackPayload;
                                        Generic.TryParse(payloadBytes, out fallbackPayload);
                                        // This can never fail, so I'm not checking the output

                                        var newPacket = new EthernetII<Generic>();
                                        newPacket.Payload = fallbackPayload;
                                        packet = newPacket;
                                    }
                                }
                                break;
                            case (ushort)EtherTypes.ARP:
                                {
                                    ARP payload;
                                    if (ARP.TryParse(payloadBytes, out payload))
                                    {
                                        var newPacket = new EthernetII<ARP>();
                                        newPacket.Payload = payload;
                                        packet = newPacket;
                                    }
                                    else
                                    {
                                        Generic fallbackPayload;
                                        Generic.TryParse(payloadBytes, out fallbackPayload);
                                        // This can never fail, so I'm not checking the output

                                        var newPacket = new EthernetII<Generic>();
                                        newPacket.Payload = fallbackPayload;
                                        packet = newPacket;
                                    }
                                }
                                break;

                            default:
                                {
                                    Generic payload;
                                    Generic.TryParse(payloadBytes, out payload);
                                    // This can never fail, so I'm not checking the output

                                    var newPacket = new EthernetII<Generic>();
                                    newPacket.Payload = payload;
                                    packet = newPacket;
                                }
                                break;
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
        ///     Constructs an EthernetII packet with default values, and the specified payload
        /// </summary>
        /// <param name="payload">Packet payload</param>
        public EthernetII(PayloadType payload)
        {
            this.SrcMac = new byte[6];
            this.DstMac = new byte[6];
            this.Payload = payload;
        }

        /// <summary>
        ///     Constructs an EthernetII packet with no defaults. All fields must be set, or things will explode.
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