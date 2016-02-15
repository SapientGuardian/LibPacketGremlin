// -----------------------------------------------------------------------
//  <copyright file="Radiotap.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.IO;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;

    /// <summary>
    ///     A Radiotap-encapsulated frame
    /// </summary>
    public abstract class Radiotap : IPacket
    {
        private const int MinimumParseableBytes = 8;

        private byte[] fieldData;

        private IPacket payload;

        public byte Version { get; set; }

        public byte Pad { get; set; }

        public ushort LengthRadiotap { get; set; }

        public uint Present { get; set; }

        public byte[] FieldData
        {
            get
            {
                return this.fieldData;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                this.fieldData = value;
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
            return 8 + this.FieldData.Length + this.Payload.Length();
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
                bw.Write(this.Pad);
                bw.Write(this.LengthRadiotap);
                bw.Write(this.Present);
                bw.Write(this.FieldData);
            }

            this.Payload.WriteToStream(stream);
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
            this.LengthRadiotap = (ushort)(this.FieldData.Length + 8);
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
        internal static bool TryParse(byte[] buffer, int index, int count, out Radiotap packet)
        {
            try
            {
                using (var ms = new MemoryStream(buffer, index, count, false))
                {
                    using (var br = new BinaryReader(ms))
                    {
                        var version = br.ReadByte();
                        var pad = br.ReadByte();
                        var length = br.ReadUInt16();
                        var present = br.ReadUInt32();

                        if (count - br.BaseStream.Position < length - 8)
                        {
                            packet = null;
                            return false;
                        }
                        var fieldData = br.ReadBytes(length - 8);

                        packet = null;

                        IEEE802_11 payload80211;
                        if (IEEE802_11.TryParse(
                            buffer,
                            index + (int)br.BaseStream.Position,
                            (int)(count - br.BaseStream.Position),
                            out payload80211))
                        {
                            packet = new Radiotap<IEEE802_11> { Payload = payload80211 };
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
                            packet = new Radiotap<Generic> { Payload = payload };
                        }

                        packet.Version = version;
                        packet.Pad = pad;
                        packet.LengthRadiotap = length;
                        packet.Present = present;
                        packet.FieldData = fieldData;

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
    ///     A Radiotap-encapsulated frame
    /// </summary>
    public class Radiotap<PayloadType> : Radiotap
        where PayloadType : class, IPacket
    {
        /// <summary>
        ///     Constructs an uninitialized packet.
        /// </summary>
        internal Radiotap()
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