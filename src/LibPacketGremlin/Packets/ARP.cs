// -----------------------------------------------------------------------
//  <copyright file="ARP.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.IO;
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets.ARPSupport;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    ///     Address Resolution Protocol, used for resolution of network layer addresses into link layer addresses
    /// </summary>
    public class ARP : IPacket
    {
        /// <summary>
        ///     The minimum number of bytes required for a successful parse
        /// </summary>
        private const int MinimumParseableBytes = 8;

        /// <summary>
        ///     The parent layer of this packet
        /// </summary>
        private IPacket container;

        /// <summary>
        ///     Constructs an uninitialized packet
        /// </summary>
        internal ARP()
        {
        }

        /// <summary>
        ///     Gets or sets the Link Layer protocol type. Example: Ethernet is 1.
        /// </summary>
        public UInt16 HType { get; set; }

        /// <summary>
        ///     Gets or sets the upper layer protocol for which the ARP request is intended. For IPv4, this has the value 0x0800.
        ///     The permitted PTYPE values share a numbering space with those for Ethertype.
        /// </summary>
        public UInt16 PType { get; set; }

        /// <summary>
        ///     Gets or sets the length (in octets) of a hardware address. Ethernet addresses size is 6.
        /// </summary>
        public byte HLen { get; set; }

        /// <summary>
        ///     Gets or sets the length (in octets) of addresses used in the upper layer protocol. (The upper layer protocol
        ///     specified in PTYPE.) IPv4 address size is 4.
        /// </summary>
        public byte PLen { get; set; }

        /// <summary>
        ///     Gets or sets a value that specifies the operation that the sender is performing: 1 for request, 2 for reply.
        /// </summary>
        public UInt16 Operation { get; set; }

        /// <summary>
        ///     Gets or sets the hardware (MAC) address of the sender.
        /// </summary>
        public byte[] SenderHardwareAddress { get; set; }

        /// <summary>
        ///     Gets or sets the upper layer protocol address of the sender.
        /// </summary>
        public byte[] SenderProtocolAddress { get; set; }

        /// <summary>
        ///     Gets or sets the hardware address of the intended receiver. This field is ignored in requests.
        /// </summary>
        public byte[] TargetHardwareAddress { get; set; }

        /// <summary>
        ///     Gets or sets the upper layer protocol address of the intended receiver.
        /// </summary>
        public byte[] TargetProtocolAddress { get; set; }

        /// <summary>
        ///     Gets the payload contained within this packet
        /// </summary>
        public IPacket Payload => null;

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
            return 8 + (this.SenderHardwareAddress?.Length ?? 0) + (this.SenderProtocolAddress?.Length ?? 0)
                   + (this.TargetHardwareAddress?.Length ?? 0) + (this.TargetProtocolAddress?.Length ?? 0);
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write(ByteOrder.HostToNetworkOrder(this.HType));
                bw.Write(ByteOrder.HostToNetworkOrder(this.PType));
                bw.Write(this.HLen);
                bw.Write(this.PLen);

                bw.Write(ByteOrder.HostToNetworkOrder(this.Operation));

                bw.Write(this.SenderHardwareAddress);

                bw.Write(this.SenderProtocolAddress);

                bw.Write(this.TargetHardwareAddress);

                bw.Write(this.TargetProtocolAddress);
            }
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
            if (this.container is EthernetII)
            {
                this.HType = (UInt16)HardwareTypes.EthernetII;
            }

            /*
                else if (container is IEEE_802_11) // Not sure if this is what was intended
                {
                    HType = (UInt16)HardwareTypes.IEEE802;
                }*/

            this.HLen = (byte)this.SenderHardwareAddress.Length;
            this.PLen = (byte)this.SenderProtocolAddress.Length;
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(byte[] buffer, int index, int count, out ARP packet)
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
                        packet = new ARP();
                        packet.HType = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        packet.PType = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        packet.HLen = br.ReadByte();
                        packet.PLen = br.ReadByte();
                        packet.Operation = ByteOrder.NetworkToHostOrder(br.ReadUInt16());
                        if (count < 6 + packet.HLen * 2 + packet.PLen * 2)
                        {
                            packet.SenderHardwareAddress = new byte[packet.HLen];
                            packet.SenderProtocolAddress = new byte[packet.PLen];
                            packet.TargetHardwareAddress = new byte[packet.HLen];
                            packet.TargetProtocolAddress = new byte[packet.PLen];
                        }
                        else
                        {
                            packet.SenderHardwareAddress = br.ReadBytes(packet.HLen);
                            packet.SenderProtocolAddress = br.ReadBytes(packet.PLen);
                            packet.TargetHardwareAddress = br.ReadBytes(packet.HLen);
                            packet.TargetProtocolAddress = br.ReadBytes(packet.PLen);
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
}