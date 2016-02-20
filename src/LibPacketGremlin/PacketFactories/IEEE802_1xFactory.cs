// -----------------------------------------------------------------------
//  <copyright file="IEEE802_1xFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for parsing IEEE802_1x packets
    /// </summary>
    public class IEEE802_1xFactory : PacketFactoryBase<IEEE802_1x>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly IEEE802_1xFactory Instance = new IEEE802_1xFactory();

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public override bool TryParse(byte[] buffer, int index, int count, out IEEE802_1x packet)
            => IEEE802_1x.TryParse(buffer, index, count, out packet);

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        /// <param name="payload">Payload</param>
        /// <returns>A Packet with default values</returns>
        public IEEE802_1x<T> Default<T>(T payload) where T : class, IPacket
        {
            var packet = new IEEE802_1x<T> { Payload = payload };
            packet.CorrectFields();
            return packet;
        }
    }
}