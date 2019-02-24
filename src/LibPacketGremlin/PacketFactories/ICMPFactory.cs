// -----------------------------------------------------------------------
//  <copyright file="ICMPFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for producing ICMP packets
    /// </summary>
    public class ICMPFactory : PacketFactoryBase<ICMP>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly ICMPFactory Instance = new ICMPFactory();

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <returns>A packet with default values</returns>
        public ICMP Default()
        {
            return new ICMP { Type = 0, Code = 0, Checksum = 0, ID = 0, Sequence = 0, Data = new byte[0] };
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out ICMP packet) => ICMP.TryParse(buffer, out packet);
    }
}