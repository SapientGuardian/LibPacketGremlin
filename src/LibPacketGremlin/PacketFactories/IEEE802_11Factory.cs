// -----------------------------------------------------------------------
//  <copyright file="IEEE802_11Factory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for parsing IEEE802_11 packets
    /// </summary>
    public class IEEE802_11Factory : PacketFactoryBase<IEEE802_11>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly IEEE802_11Factory Instance = new IEEE802_11Factory();

        public override bool TryParse(ReadOnlySpan<byte> buffer, out IEEE802_11 packet) => IEEE802_11.TryParse(buffer, out packet);
    }
}