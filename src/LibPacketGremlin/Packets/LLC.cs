// -----------------------------------------------------------------------
//  <copyright file="LLC.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.IO;
    using System.Text;
    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    /// Logical Link Control
    /// </summary>
    public abstract class LLC : IPacket
    {
        private const int MinimumParseableBytes = 3;

        private IPacket payload;

        /// <summary>
        /// Gets or sets the destination service access point
        /// </summary>
        public byte DSAP { get; set; }

        /// <summary>
        /// Gets or sets the source service access point
        /// </summary>
        public byte SSAP { get; set; }

        /// <summary>
        /// Gets or sets control data
        /// </summary>
        public byte CommandControl { get; set; }

        /// <summary>
        /// Gets or sets control data
        /// </summary>
        public ushort ResponseControl { get; set; }

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
            return 2 + (this.DSAP % 2 == 0 ? 1 : 2) + this.Payload.Length();
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write(this.DSAP);
                bw.Write(this.SSAP);
                if (this.DSAP % 2 == 0)
                {
                    bw.Write(this.CommandControl);
                }
                else
                {
                    bw.Write(this.ResponseControl);
                }
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
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(ReadOnlySpan<byte> buffer, out LLC packet)
        {
            try
            {
                if (buffer.Length < MinimumParseableBytes)
                {
                    packet = null;
                    return false;
                }

                var br = new SpanReader(buffer);
                var dsap = br.ReadByte();
                var ssap = br.ReadByte();

                byte commandControl = 0;
                UInt16 responseControl = 0;

                if (dsap % 2 == 0)
                {
                    commandControl = br.ReadByte();
                }
                else if (buffer.Length < 4)
                {
                    packet = null;
                    return false;
                }
                else
                {
                    responseControl = br.ReadUInt16LittleEndian();
                }

                var payloadBytes = br.Slice();
                SNAP payloadSNAP;
                if (SNAP.TryParse(payloadBytes, out payloadSNAP))
                {
                    packet = new LLC<SNAP> { Payload = payloadSNAP };
                }
                else
                {
                    Generic payloadGeneric;
                    Generic.TryParse(payloadBytes, out payloadGeneric);

                    // This can never fail, so I'm not checking the output
                    packet = new LLC<Generic> { Payload = payloadGeneric };
                }

                packet.DSAP = dsap;
                packet.SSAP = ssap;
                packet.CommandControl = commandControl;
                packet.ResponseControl = responseControl;

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
    /// Logical Link Control
    /// </summary>
    public class LLC<PayloadType> : LLC
        where PayloadType : class, IPacket
    {
        /// <summary>
        ///     Constructs an uninitialized packet.
        /// </summary>
        internal LLC()
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