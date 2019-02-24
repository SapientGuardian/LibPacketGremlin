// -----------------------------------------------------------------------
//  <copyright file="WakeOnLan.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    ///     Wake On LAN is used to turn on a device over a network
    /// </summary>
    public class WakeOnLan : IPacket
    {
        /// <summary>
        ///     The minimum number of bytes required for a successful parse
        /// </summary>
        private const int MinimumParseableBytes = 6 + 6 * 16;

        private byte[] dstMac;

        /// <summary>
        ///     Constructs am uninitialized packet
        /// </summary>
        internal WakeOnLan()
        {
        }

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
        ///     Gets or sets the password for waking the device
        /// </summary>
        public byte[] Password { get; set; }

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
        }

        /// <summary>
        ///     Gets the length of the packet
        /// </summary>
        /// <returns>Length of the packet</returns>
        public long Length()
        {
            return 6 + (16 * 6) + (this.Password?.Length ?? 0);
        }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                for (var i = 0; i < 6; i++)
                {
                    bw.Write((byte)0xFF);
                }

                for (var i = 0; i < 16; i++)
                {
                    bw.Write(this.DstMac);
                }

                bw.Write(this.Password);
            }
        }

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        public void CorrectFields()
        {
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(ReadOnlySpan<byte> buffer, out WakeOnLan packet)
        {
            try
            {
                if (buffer.Length < MinimumParseableBytes)
                {
                    packet = null;
                    return false;
                }

                // Too many bytes
                if (buffer.Length > 108)
                {
                    packet = null;
                    return false;
                }


                var br = new SpanReader(buffer);
                for (var i = 0; i < 6; i++)
                {
                    if (br.ReadByte() != 255)
                    {
                        // TODO: Option to ignore
                        // Invalid synchronization stream.
                        packet = null;
                        return false;
                    }
                }

                packet = new WakeOnLan();
                packet.DstMac = br.ReadBytes(6);
                for (var i = 0; i < 15; i++)
                {
                    var tmpMac = br.ReadBytes(6);
                    if (!tmpMac.SequenceEqual(packet.DstMac))
                    {
                        // TODO: Option to ignore
                        // Invalid target mac repetition
                        packet = null;
                        return false;
                    }
                }

                if (buffer.Length - br.Position >= 6)
                {
                    packet.Password = br.ReadBytes(6);
                }
                else if (buffer.Length - br.Position >= 4)
                {
                    packet.Password = br.ReadBytes(4);
                }
                else
                {
                    packet.Password = Array.Empty<byte>();
                }

                return true;

            }
            catch (Exception)
            {
                packet = null;
                return false;
            }
        }
    }
}