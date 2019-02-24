// -----------------------------------------------------------------------
//  <copyright file="ARPFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for producing ARP packets
    /// </summary>
    public class ARPFactory : PacketFactoryBase<ARP>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly ARPFactory Instance = new ARPFactory();

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <returns>A packet with default values</returns>
        public ARP Default()
        {
            return new ARP
                       {
                           HType = 0,
                           PType = 0,
                           HLen = 0,
                           PLen = 0,
                           Operation = 0,
                           SenderHardwareAddress = new byte[0],
                           SenderProtocolAddress = new byte[0],
                           TargetHardwareAddress = new byte[0],
                           TargetProtocolAddress = new byte[0]
                       };
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out ARP packet) => ARP.TryParse(buffer, out packet);
    }
}