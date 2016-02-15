// -----------------------------------------------------------------------
//  <copyright file="IEEE802_11.cs" company="Outbreak Labs">
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
    using OutbreakLabs.LibPacketGremlin.Extensions;
    using OutbreakLabs.LibPacketGremlin.Packets.IEEE802_11Support;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    ///     Wireless LAN protocol
    /// </summary>
    public abstract class IEEE802_11 : IPacket
    {
        private const int MinimumParseableBytes = 24;

        private byte[] bssid;

        private byte[] ccmpWepData;

        private byte[] destination;

        /// <summary>
        ///     This flag is one bit. It indicates whether a data frame is headed for a distributed system.
        /// </summary>
        protected BitVector32 FrameControlFlags;

        protected BitVector32 FrameControlVersionTypeAndSubtype;

        private IPacket payload;

        private byte[] qosControl;

        private byte[] receiver;

        private byte[] source;

        private byte[] transmitter;

        /// <summary>
        ///     Two bits identifying the type of WLAN frame. Control, Data and Management are various frame types defined in IEEE
        ///     802.11
        /// </summary>
        public int SubType
        {
            get
            {
                return
                    this.FrameControlVersionTypeAndSubtype[BitVector32.CreateSection(15, BitVector32.CreateSection(15))];
            }
            set
            {
                this.FrameControlVersionTypeAndSubtype[BitVector32.CreateSection(15, BitVector32.CreateSection(15))] =
                    value;
            }
        }

        /// <summary>
        ///     Four bits providing addition discrimination between frames. Type and Sub type go together to identify the exact
        ///     frame.
        /// </summary>
        public int FrameType
        {
            get
            {
                return
                    this.FrameControlVersionTypeAndSubtype[BitVector32.CreateSection(3, BitVector32.CreateSection(3))];
            }
            set
            {
                this.FrameControlVersionTypeAndSubtype[BitVector32.CreateSection(3, BitVector32.CreateSection(3))] =
                    value;
            }
        }

        /// <summary>
        ///     This field is two bits representing the protocol version. Currently used protocol version is zero. Other values are
        ///     reserved for future use.
        /// </summary>
        public int ProtocolVersion
        {
            get
            {
                return this.FrameControlVersionTypeAndSubtype[BitVector32.CreateSection(3)];
            }
            set
            {
                this.FrameControlVersionTypeAndSubtype[BitVector32.CreateSection(3)] = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether a data frame was sent to a station
        /// </summary>
        public bool ToDS
        {
            get
            {
                return this.FrameControlFlags[1];
            }
            set
            {
                this.FrameControlFlags[1] = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether a data frame was sent from a station
        /// </summary>
        public bool FromDS
        {
            get
            {
                return this.FrameControlFlags[2];
            }
            set
            {
                this.FrameControlFlags[2] = value;
            }
        }

        /// <summary>
        ///     The More Fragments bit is set when a packet is divided into multiple frames for transmission. Every frame except
        ///     the last frame of a packet will have this bit set.
        /// </summary>
        public bool MoreFrag
        {
            get
            {
                return this.FrameControlFlags[4];
            }
            set
            {
                this.FrameControlFlags[4] = value;
            }
        }

        /// <summary>
        ///     The Retry bit is set to one when a frame is resent. This aids in the elimination of duplicate frames.
        /// </summary>
        public bool Retry
        {
            get
            {
                return this.FrameControlFlags[8];
            }
            set
            {
                this.FrameControlFlags[8] = value;
            }
        }

        /// <summary>
        ///     This bit indicates the power management state of the sender after the completion of a frame exchange.
        /// </summary>
        public bool PowerMgt
        {
            get
            {
                return this.FrameControlFlags[16];
            }
            set
            {
                this.FrameControlFlags[16] = value;
            }
        }

        /// <summary>
        ///     The More Data bit is used to buffer frames received in a distributed system. It indicates that at least one frame
        ///     is available and addresses all stations connected.
        /// </summary>
        public bool MoreData
        {
            get
            {
                return this.FrameControlFlags[32];
            }
            set
            {
                this.FrameControlFlags[32] = value;
            }
        }

        /// <summary>
        ///     The WEP bit is modified after processing a frame. It is toggled to one after a frame has been decrypted or if no
        ///     encryption is set it will have already been one.
        /// </summary>
        public bool Protected
        {
            get
            {
                return this.FrameControlFlags[64];
            }
            set
            {
                this.FrameControlFlags[64] = value;
            }
        }

        /// <summary>
        ///     This bit is only set when the 'strict ordering' delivery method is employed. Frames and fragments are not always
        ///     sent in order as it causes a transmission performance penalty.
        /// </summary>
        public bool Order
        {
            get
            {
                return this.FrameControlFlags[128];
            }
            set
            {
                this.FrameControlFlags[128] = value;
            }
        }

        /// <summary>
        ///     This field can take one of three forms: Duration, Contention-Free Period (CFP), and Association ID (AID).
        /// </summary>
        public UInt16 DurationID { get; set; }

        /// <summary>
        ///     Gets or sets the original source of the frame
        /// </summary>
        public byte[] Source
        {
            get
            {
                return this.source;
            }
            set
            {
                if (value != null && value.Length != 6)
                {
                    throw new ArgumentException(
                        "If this field is set, it must be exactly 6 bytes in length",
                        nameof(value));
                }
                this.source = value;
            }
        }

        /// <summary>
        ///     Gets or sets the final recipient of the frame
        /// </summary>
        public byte[] Destination
        {
            get
            {
                return this.destination;
            }
            set
            {
                if (value != null && value.Length != 6)
                {
                    throw new ArgumentException(
                        "If this field is set, it must be exactly 6 bytes in length",
                        nameof(value));
                }
                this.destination = value;
            }
        }

        /// <summary>
        ///     Gets or sets the MAC address of the access point
        /// </summary>
        public byte[] BSSID
        {
            get
            {
                return this.bssid;
            }
            set
            {
                if (value != null && value.Length != 6)
                {
                    throw new ArgumentException(
                        "If this field is set, it must be exactly 6 bytes in length",
                        nameof(value));
                }
                this.bssid = value;
            }
        }

        /// <summary>
        ///     Gets or sets the immediate sender of the frame
        /// </summary>
        public byte[] Transmitter
        {
            get
            {
                return this.transmitter;
            }
            set
            {
                if (value != null && value.Length != 6)
                {
                    throw new ArgumentException(
                        "If this field is set, it must be exactly 6 bytes in length",
                        nameof(value));
                }
                this.transmitter = value;
            }
        }

        /// <summary>
        ///     Gets or sets the immediate receiver of the frame
        /// </summary>
        public byte[] Receiver
        {
            get
            {
                return this.receiver;
            }
            set
            {
                if (value != null && value.Length != 6)
                {
                    throw new ArgumentException(
                        "If this field is set, it must be exactly 6 bytes in length",
                        nameof(value));
                }
                this.receiver = value;
            }
        }

        /// <summary>
        ///     Checksum, not always present
        /// </summary>
        public UInt32 FrameCheckSequence { get; set; }

        /// <summary>
        ///     The Sequence Control field is a two-byte section used for identifying message order as well as eliminating
        ///     duplicate frames.
        /// </summary>
        public UInt16 SequenceControl { get; set; }

        /// <summary>
        ///     Gets or sets the fragment number
        /// </summary>
        public int FragmentNumber => this.SequenceControl & 15;

        /// <summary>
        ///     Gets or sets the sequence number
        /// </summary>
        public int SequenceNumber => this.SequenceControl >> 4;

        public byte[] CCMP_WEP_Data
        {
            get
            {
                return this.ccmpWepData;
            }
            set
            {
                if (value != null && value.Length != 4 && value.Length != 8)
                {
                    throw new ArgumentException(
                        "If this field is set, it must be exactly 4 or 8 bytes in length",
                        nameof(value));
                }
                this.ccmpWepData = value;
            }
        }

        /// <summary>
        ///     Gets or sets the integrity check value for WEP
        /// </summary>
        public UInt32 WEP_ICV { get; set; }

        public byte[] QosControl
        {
            get
            {
                return this.qosControl;
            }
            set
            {
                if (value != null && value.Length != 2)
                {
                    throw new ArgumentException(
                        "If this field is set, it must be exactly 2 bytes in length",
                        nameof(value));
                }
                this.qosControl = value;
            }
        }

        public bool IsWep => this.Protected && this.CCMP_WEP_Data != null && this.CCMP_WEP_Data.Length == 4;

        public byte[] ClientMac
        {
            get
            {
                byte[] clientMac;
                if (this.ToDS == false && this.FromDS) //Destination == client/station
                {
                    clientMac = this.Destination;
                }
                else
                {
                    clientMac = this.Source;
                }
                return clientMac;
            }
        }

        public byte[] APMac
        {
            get
            {
                byte[] apMac;
                if (this.ToDS == false && this.FromDS) //Destination == client/station
                {
                    apMac = this.Source;
                }
                else
                {
                    apMac = this.Destination;
                }
                return apMac;
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
            // TODO: Calculate
            return this.ToArray().Length;
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write((byte)this.FrameControlVersionTypeAndSubtype.Data);
                bw.Write((byte)this.FrameControlFlags.Data);
                bw.Write(this.DurationID);
                if (!this.ToDS)
                {
                    bw.Write(this.Destination);

                    if (!this.FromDS)
                    {
                        bw.Write(this.Source);
                        bw.Write(this.BSSID);
                    }
                    else
                    {
                        bw.Write(this.BSSID);
                        bw.Write(this.Source);
                    }
                    bw.Write(this.SequenceControl); //Was HTNO, wireshark disagrees
                }
                else
                {
                    if (!this.FromDS)
                    {
                        bw.Write(this.BSSID);
                        bw.Write(this.Source);
                        bw.Write(this.Destination);
                        bw.Write(this.SequenceControl); //Was HTNO, wireshark disagrees
                    }
                    else
                    {
                        bw.Write(this.Receiver);
                        bw.Write(this.Transmitter);
                        bw.Write(this.Destination);
                        bw.Write(this.SequenceControl); //Was HTNO, wireshark disagrees
                        bw.Write(this.Source);
                    }
                }

                if (this.FrameType == (int)FrameTypes.Data && this.SubType == (int)DataSubTypes.QoS)
                {
                    bw.Write(this.QosControl);
                }

                if (this.FrameType == (int)FrameTypes.Data && this.Protected)
                {
                    bw.Write(this.CCMP_WEP_Data);
                }
            }

            this.Payload.WriteToStream(stream);

            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                if (this.IsWep)
                {
                    bw.Write(ByteOrder.HostToNetworkOrder(this.WEP_ICV));
                }

                if (this.FrameCheckSequence != 0)
                {
                    bw.Write(this.FrameCheckSequence);
                }
            }
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
        internal static bool TryParse(byte[] buffer, int index, int count, out IEEE802_11 packet)
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
                        var frameControlVersionTypeAndSubtype = new BitVector32(br.ReadByte());
                        var frameControlFlags = new BitVector32(br.ReadByte());
                        var durationID = br.ReadUInt16();

                        var toDS = frameControlFlags[1];
                        var fromDS = frameControlFlags[2];

                        byte[] destination;
                        byte[] source;
                        byte[] bssid;
                        byte[] receiver = null;
                        byte[] transmitter = null;
                        UInt16 sequenceControl;

                        if (!toDS)
                        {
                            destination = br.ReadBytes(6);

                            if (!fromDS)
                            {
                                source = br.ReadBytes(6);
                                bssid = br.ReadBytes(6);
                            }
                            else
                            {
                                bssid = br.ReadBytes(6);
                                source = br.ReadBytes(6);
                            }
                            sequenceControl = br.ReadUInt16(); //Was NTHO, wireshark disagrees
                        }
                        else
                        {
                            if (!fromDS)
                            {
                                bssid = br.ReadBytes(6);
                                source = br.ReadBytes(6);
                                destination = br.ReadBytes(6);
                                sequenceControl = br.ReadUInt16(); //Was NTHO, wireshark disagrees
                            }
                            else
                            {
                                receiver = br.ReadBytes(6);
                                transmitter = br.ReadBytes(6);
                                bssid = transmitter; //Per airdecap-ng
                                destination = br.ReadBytes(6);
                                sequenceControl = br.ReadUInt16(); //Was NTHO, wireshark disagrees
                                source = br.ReadBytes(6);
                            }
                        }

                        var frameType =
                            frameControlVersionTypeAndSubtype[BitVector32.CreateSection(3, BitVector32.CreateSection(3))
                                ];
                        var subType =
                            frameControlVersionTypeAndSubtype[
                                BitVector32.CreateSection(15, BitVector32.CreateSection(15))];

                        byte[] qosControl = null;
                        if (frameType == (int)FrameTypes.Data && subType == (int)DataSubTypes.QoS)
                        {
                            qosControl = br.ReadBytes(2);
                        }

                        var isProtected = frameControlFlags[64];
                        byte[] ccmp_WEP_Data = null;

                        if (isProtected)
                        {
                            if (count - br.BaseStream.Position < 4)
                            {
                                packet = null;
                                return false;
                            }
                            var firstFour = br.ReadBytes(4);
                            if (firstFour[3] == 0)
                            {
                                ccmp_WEP_Data = firstFour;
                            }
                            else
                            {
                                if (count - br.BaseStream.Position < 4)
                                {
                                    packet = null;
                                    return false;
                                }
                                ccmp_WEP_Data = new byte[8];
                                firstFour.CopyTo(ccmp_WEP_Data, 0);
                                br.ReadBytes(4).CopyTo(ccmp_WEP_Data, 4);
                            }
                        }

                        var isWep = isProtected && ccmp_WEP_Data?.Length == 4;

                        if (count - br.BaseStream.Position - (isWep ? 4 : 0) < 0)
                        {
                            packet = null;
                            return false;
                        }

                        var unsafePayloadLen = count - (int)br.BaseStream.Position - (isWep ? 4 : 0);
                        var safePayloadLen = Math.Max(0, unsafePayloadLen);

                        packet = null;

                        if (frameType == (int)FrameTypes.Data && ((subType & 4) != 1) && !isProtected)
                        {
                            LLC payload;
                            if (LLC.TryParse(buffer, index + (int)br.BaseStream.Position, safePayloadLen, out payload))
                            {
                                packet = new IEEE802_11<LLC> { Payload = payload };
                            }
                        }
                        else if (frameType == (int)FrameTypes.Management && subType == (int)ManagementSubTypes.Beacon)
                        {
                            Beacon802_11 payload;
                            if (Beacon802_11.TryParse(
                                buffer,
                                index + (int)br.BaseStream.Position,
                                safePayloadLen,
                                out payload))
                            {
                                packet = new IEEE802_11<Beacon802_11> { Payload = payload };
                            }
                        }

                        if (packet == null)
                        {
                            Generic payload;
                            Generic.TryParse(buffer, index + (int)br.BaseStream.Position, safePayloadLen, out payload);

                            // This can never fail, so I'm not checking the output
                            packet = new IEEE802_11<Generic> { Payload = payload };
                        }

                        br.BaseStream.Seek(packet.Payload.Length(), SeekOrigin.Current);

                        UInt32 wep_ICV = 0;
                        if (isWep)
                        {
                            wep_ICV = ByteOrder.NetworkToHostOrder(br.ReadUInt32());
                        }

                        packet.FrameControlVersionTypeAndSubtype = frameControlVersionTypeAndSubtype;
                        packet.FrameControlFlags = frameControlFlags;
                        packet.DurationID = durationID;
                        packet.Destination = destination;
                        packet.Source = source;
                        packet.BSSID = bssid;
                        packet.SequenceControl = sequenceControl;
                        packet.Receiver = receiver;
                        packet.Transmitter = transmitter;
                        packet.QosControl = qosControl;
                        packet.CCMP_WEP_Data = ccmp_WEP_Data;
                        packet.WEP_ICV = wep_ICV;

                        if (br.BaseStream.Position == count - 4)
                        {
                            packet.FrameCheckSequence = br.ReadUInt32();
                        }

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
    ///     Wireless LAN protocol
    /// </summary>
    public class IEEE802_11<PayloadType> : IEEE802_11
        where PayloadType : class, IPacket
    {
        /// <summary>
        ///     Constructs an uninitialized packet.
        /// </summary>
        internal IEEE802_11()
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