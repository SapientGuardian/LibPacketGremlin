// -----------------------------------------------------------------------
//  <copyright file="MSMon802_11Factory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for parsing MSMon802_11 packets
    /// </summary>
    public class MSMon802_11Factory : PacketFactoryBase<MSMon802_11>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly MSMon802_11Factory Instance = new MSMon802_11Factory();

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public override bool TryParse(byte[] buffer, int index, int count, out MSMon802_11 packet)
            => MSMon802_11.TryParse(buffer, index, count, out packet);

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        /// <param name="payload">Payload</param>
        /// <returns>A Packet with default values</returns>
        public MSMon802_11<T> Default<T>(T payload) where T : class, IPacket
        {
            return new MSMon802_11<T> { Payload = payload };
        }
    }
}