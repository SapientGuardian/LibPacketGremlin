// -----------------------------------------------------------------------
//  <copyright file="IEEE802_1x.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.IO;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets.IEEE802_1xSupport;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    /// Encapsulation of EAP over IEEE 802
    /// </summary>
    public abstract class IEEE802_1x : IPacket
    {
        private const int MinimumParseableBytes = 8;

        protected UInt16 _BodyLength;

        protected byte _Type;

        protected byte _Version;

        private IPacket payload;

        /// <summary>
        /// Gets or sets the version of the protocol
        /// </summary>
        public byte Version
        {
            get
            {
                return this._Version;
            }
            set
            {
                this._Version = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of data contained in the packet
        /// </summary>
        public byte Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }

        /// <summary>
        /// Gets or sets the length of the body
        /// </summary>
        public UInt16 BodyLength
        {
            get
            {
                return this._BodyLength;
            }
            set
            {
                this._BodyLength = value;
            }
        }

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
            return 4 + this.Payload.Length();
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write(this.Version);
                bw.Write(this.Type);
                bw.Write(ByteOrder.HostToNetworkOrder(this.BodyLength));
            }

            this.Payload.WriteToStream(stream);
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
            if (this.Payload is EapolKey)
            {
                this.Type = (byte)Types.EAPOL_KEY;
            }

            this.BodyLength = (ushort)this.Payload.Length();

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
        internal static bool TryParse(byte[] buffer, int index, int count, out IEEE802_1x packet)
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
                        var version = br.ReadByte();
                        var type = br.ReadByte();
                        var bodyLength = ByteOrder.NetworkToHostOrder(br.ReadUInt16());

                        var payloadLength = Math.Min(bodyLength, (int)(count - br.BaseStream.Position));

                        packet = null;

                        switch (type)
                        {
                            case (byte)Types.EAPOL_KEY:
                                {
                                    EapolKey payload;
                                    if (EapolKey.TryParse(
                                        buffer,
                                        index + (int)br.BaseStream.Position,
                                        payloadLength,
                                        out payload))
                                    {
                                        packet = new IEEE802_1x<EapolKey> { Payload = payload };
                                    }
                                }
                                break;
                        }

                        if (packet == null)
                        {
                            Generic payload;
                            Generic.TryParse(
                                buffer,
                                index + (int)br.BaseStream.Position,
                                (int)(count - br.BaseStream.Position),
                                out payload);

                            // This can never fail, so I'm not checking the output
                            packet = new IEEE802_1x<Generic> { Payload = payload };
                        }

                        packet.Version = version;
                        packet.Type = type;
                        packet.BodyLength = bodyLength;

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
    /// Encapsulation of EAP over IEEE 802
    /// </summary>
    public class IEEE802_1x<PayloadType> : IEEE802_1x
        where PayloadType : class, IPacket
    {
        /// <summary>
        ///     Constructs an uninitialized packet.
        /// </summary>
        internal IEEE802_1x()
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