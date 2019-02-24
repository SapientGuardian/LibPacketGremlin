// -----------------------------------------------------------------------
//  <copyright file="SNAP.cs" company="Outbreak Labs">
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
    /// Subnetwork Access Protocol, an extension of LLC
    /// </summary>
    public abstract class SNAP : IPacket
    {
        private const int MinimumParseableBytes = 5;

        private byte[] organizationCode;

        private IPacket payload;

        /// <summary>
        /// Gets or sets a vendor identifier
        /// </summary>
        public byte[] OrganizationCode
        {
            get
            {
                return this.organizationCode;
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
                this.organizationCode = value;
            }
        }

        /// <summary>
        /// Vendor specific protocol, or ethertype if OrganizationCode is 0
        /// </summary>
        public ushort ProtocolID { get; set; }

        /// <summary>
        ///     Gets the payload contained within this packet
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
            return 5 + this.Payload.Length();
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write(this.OrganizationCode);
                bw.Write(ByteOrder.HostToNetworkOrder(this.ProtocolID));
            }

            this.Payload.WriteToStream(stream);
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
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
        internal static bool TryParse(ReadOnlySpan<byte> buffer, out SNAP packet)
        {
            try
            {
                if (buffer.Length < MinimumParseableBytes)
                {
                    packet = null;
                    return false;
                }

                var br = new SpanReader(buffer);
                var organizationCode = br.ReadBytes(3);
                var etherType = br.ReadUInt16BigEndian();

                packet = null;
                var payloadBytes = br.Slice();

                if (organizationCode[0] == 0 && organizationCode[1] == 0 && organizationCode[2] == 0)
                {
                    switch (etherType)
                    {
                        case (ushort)EtherTypes.IPv4:
                            {
                                IPv4 payload;
                                if (IPv4.TryParse(payloadBytes, out payload))
                                {
                                    packet = new SNAP<IPv4> { Payload = payload };
                                }
                            }
                            break;
                        case (ushort)EtherTypes.ARP:
                            {
                                ARP payload;
                                if (ARP.TryParse(payloadBytes, out payload))
                                {
                                    packet = new SNAP<ARP> { Payload = payload };
                                }
                            }
                            break;
                        case (ushort)EtherTypes.EAPoLAN:
                            {
                                IEEE802_1x payload;
                                if (IEEE802_1x.TryParse(payloadBytes, out payload))
                                {
                                    packet = new SNAP<IEEE802_1x> { Payload = payload };
                                }
                            }
                            break;
                    }
                }

                if (packet == null)
                {
                    Generic payload;
                    Generic.TryParse(payloadBytes, out payload);

                    // This can never fail, so I'm not checking the output
                    packet = new SNAP<Generic> { Payload = payload };
                }

                packet.OrganizationCode = organizationCode;
                packet.ProtocolID = etherType;

                return true;

            }
            catch (Exception)
            {
                packet = null;
                return false;
            }
        }
    }

    /// <summary>
    /// Subnetwork Access Protocol, an extension of LLC
    /// </summary>
    public class SNAP<PayloadType> : SNAP
        where PayloadType : class, IPacket
    {
        /// <summary>
        ///     Constructs an uninitialized packet.
        /// </summary>
        internal SNAP()
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