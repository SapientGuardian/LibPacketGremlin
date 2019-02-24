// -----------------------------------------------------------------------
//  <copyright file="LLCFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for parsing LLC packets
    /// </summary>
    public class LLCFactory : PacketFactoryBase<LLC>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly LLCFactory Instance = new LLCFactory();

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        /// <param name="payload">Payload</param>
        /// <returns>A Packet with default values</returns>
        public LLC<T> Default<T>(T payload) where T : class, IPacket
        {
            return new LLC<T> { Payload = payload };
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out LLC packet) => LLC.TryParse(buffer, out packet);
    }
}