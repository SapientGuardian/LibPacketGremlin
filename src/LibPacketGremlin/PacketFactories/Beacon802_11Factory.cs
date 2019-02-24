// -----------------------------------------------------------------------
//  <copyright file="Beacon802_11Factory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for producing Beacon802_11 packets
    /// </summary>
    public class Beacon802_11Factory : PacketFactoryBase<Beacon802_11>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly Beacon802_11Factory Instance = new Beacon802_11Factory();

        public override bool TryParse(ReadOnlySpan<byte> buffer, out Beacon802_11 packet) => Beacon802_11.TryParse(buffer, out packet);
    }
}