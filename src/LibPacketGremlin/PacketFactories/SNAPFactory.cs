// -----------------------------------------------------------------------
//  <copyright file="SNAPFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for parsing SNAP packets
    /// </summary>
    public class SNAPFactory : PacketFactoryBase<SNAP>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly SNAPFactory Instance = new SNAPFactory();

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        /// <param name="payload">Payload</param>
        /// <returns>A Packet with default values</returns>
        public SNAP<T> Default<T>(T payload) where T : class, IPacket
        {
            return new SNAP<T> { OrganizationCode = new byte[3], Payload = payload };
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out SNAP packet) => SNAP.TryParse(buffer, out packet);
    }
}