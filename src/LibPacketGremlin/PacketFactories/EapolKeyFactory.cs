// -----------------------------------------------------------------------
//  <copyright file="EapolKeyFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for producing EapolKey packets
    /// </summary>
    public class EapolKeyFactory : PacketFactoryBase<EapolKey>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly EapolKeyFactory Instance = new EapolKeyFactory();

        public override bool TryParse(ReadOnlySpan<byte> buffer, out EapolKey packet) => EapolKey.TryParse(buffer, out packet);
    }
}