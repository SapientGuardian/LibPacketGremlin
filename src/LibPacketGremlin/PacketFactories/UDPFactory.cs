// -----------------------------------------------------------------------
//  <copyright file="UDPFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for producing UDP packets
    /// </summary>
    public class UDPFactory : PacketFactoryBase<UDP>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly UDPFactory Instance = new UDPFactory();

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        /// <param name="payload">Payload</param>
        /// <returns>A Packet with default values</returns>
        public UDP<T> Default<T>(T payload) where T : class, IPacket
        {
            return new UDP<T> { Payload = payload };
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out UDP packet) => UDP.TryParse(buffer, out packet);
    }
}