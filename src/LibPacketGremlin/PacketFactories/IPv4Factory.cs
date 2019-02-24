// -----------------------------------------------------------------------
//  <copyright file="IPv4Factory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
    using System.Collections.Specialized;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets;
    using OutbreakLabs.LibPacketGremlin.Packets.IPv4Support;

    /// <summary>
    ///     Factory for producing IPv4 packets
    /// </summary>
    public class IPv4Factory : PacketFactoryBase<IPv4>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly IPv4Factory Instance = new IPv4Factory();

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        /// <param name="payload">Payload</param>
        /// <returns>A Packet with default values</returns>
        public IPv4<T> Default<T>(T payload) where T : class, IPacket
        {
            return new IPv4<T>
                       {
                           Version = 4,
                           HeaderLength = 5,
                           DifferentiatedServices = 0,
                           TotalLength = 0,
                           ID = 0,
                           FlagsAndFragOff = new BitVector32(0),
                           TTL = 0,
                           Protocol = 0,
                           HeaderChecksum = 0,
                           OptionsAndPadding = new byte[0],
                           SourceAddress = new IPv4Address(0),
                           DestAddress = new IPv4Address(0),
                           Payload = payload
                       };
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out IPv4 packet) => IPv4.TryParse(buffer, out packet);
    }
}