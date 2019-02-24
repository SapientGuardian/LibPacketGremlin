// -----------------------------------------------------------------------
//  <copyright file="EthernetIIFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for parsing EthernetII packets
    /// </summary>
    public class EthernetIIFactory : PacketFactoryBase<EthernetII>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly EthernetIIFactory Instance = new EthernetIIFactory();

       
        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        /// <param name="payload">Payload</param>
        /// <returns>A Packet with default values</returns>
        public EthernetII<T> Default<T>(T payload) where T : class, IPacket
        {
            return new EthernetII<T> { SrcMac = new byte[6], DstMac = new byte[6], Payload = payload };
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out EthernetII packet) => EthernetII.TryParse(buffer, out packet);
    }
}