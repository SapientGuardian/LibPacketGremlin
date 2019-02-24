// -----------------------------------------------------------------------
//  <copyright file="WakeOnLanFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;

    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for producing WakeOnLan packets
    /// </summary>
    public class WakeOnLanFactory : PacketFactoryBase<WakeOnLan>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly WakeOnLanFactory Instance = new WakeOnLanFactory();

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <returns>A packet with default values</returns>
        public WakeOnLan Default()
        {
            return new WakeOnLan { DstMac = new byte[6], Password = Array.Empty<byte>() };
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out WakeOnLan packet) => WakeOnLan.TryParse(buffer, out packet);
    }
}