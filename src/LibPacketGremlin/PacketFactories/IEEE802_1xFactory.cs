// -----------------------------------------------------------------------
//  <copyright file="IEEE802_1xFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
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

        public override bool TryParse(ReadOnlySpan<byte> buffer, out IEEE802_1x packet) => IEEE802_1x.TryParse(buffer, out packet);
    }
}