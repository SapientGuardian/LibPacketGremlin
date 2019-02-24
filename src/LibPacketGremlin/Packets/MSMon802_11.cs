// -----------------------------------------------------------------------
//  <copyright file="MSMon802_11.cs" company="Outbreak Labs">
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
    ///     Wireless header added by Microsoft Network Monitor
    /// </summary>
    public abstract class MSMon802_11 : IPacket
    {
        private const int MinimumParseableBytes = 32;

        private IPacket payload;

        public byte Version { get; set; }

        // protected byte revision;
        public ushort LengthMS { get; set; }

        public uint OpMode { get; set; }

        public uint ReceiveFlags { get; set; }

        public uint PhyID { get; set; }

        public uint CenterFrequency { get; set; }

        //protected ushort numberOfMPDUsReceived;
        public int RSSI { get; set; }

        public byte DataRate { get; set; }

        public ulong Timestamp { get; set; }

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
            return 32 + this.Payload.Length();
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
                bw.Write(this.LengthMS);
                bw.Write(this.OpMode);
                bw.Write(this.ReceiveFlags);
                bw.Write(this.PhyID);
                bw.Write(this.CenterFrequency);
                bw.Write(this.RSSI);
                bw.Write(this.DataRate);

                bw.Write(this.Timestamp);
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
        internal static bool TryParse(ReadOnlySpan<byte> buffer, out MSMon802_11 packet)
        {
            try
            {
                if (buffer.Length < MinimumParseableBytes)
                {
                    packet = null;
                    return false;
                }

                var br = new SpanReader(buffer);

                var version = br.ReadByte();
                var length = br.ReadUInt16LittleEndian();
                var opMode = br.ReadUInt32LittleEndian();
                var receiveFlags = br.ReadUInt32LittleEndian();
                var phyID = br.ReadUInt32LittleEndian();
                var chCenterFrequency = br.ReadUInt32LittleEndian();

                var RSSI = br.ReadInt32LittleEndian();
                var dataRate = br.ReadByte();

                var timestamp = br.ReadUInt64LittleEndian();

                packet = null;

                // TODO: Consider parsed length
                var payloadBytes = br.Slice();
                IEEE802_11 payload80211;
                if (IEEE802_11.TryParse(payloadBytes, out payload80211))
                {
                    packet = new MSMon802_11<IEEE802_11> { Payload = payload80211 };
                }

                if (packet == null)
                {
                    Generic payload;
                    Generic.TryParse(payloadBytes, out payload);

                    // This can never fail, so I'm not checking the output
                    packet = new MSMon802_11<Generic> { Payload = payload };
                }

                packet.Version = version;
                packet.LengthMS = length;
                packet.OpMode = opMode;
                packet.ReceiveFlags = receiveFlags;
                packet.PhyID = phyID;
                packet.CenterFrequency = chCenterFrequency;
                packet.RSSI = RSSI;
                packet.DataRate = dataRate;
                packet.Timestamp = timestamp;

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
    ///     Wireless header added by Microsoft Network Monitor
    /// </summary>
    public class MSMon802_11<PayloadType> : MSMon802_11
        where PayloadType : class, IPacket
    {
        /// <summary>
        ///     Constructs an uninitialized packet.
        /// </summary>
        internal MSMon802_11()
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