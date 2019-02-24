// -----------------------------------------------------------------------
//  <copyright file="MSMon802_11Factory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for parsing MSMon802_11 packets
    /// </summary>
    public class MSMon802_11Factory : PacketFactoryBase<MSMon802_11>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly MSMon802_11Factory Instance = new MSMon802_11Factory();

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        /// <param name="payload">Payload</param>
        /// <returns>A Packet with default values</returns>
        public MSMon802_11<T> Default<T>(T payload) where T : class, IPacket
        {
            return new MSMon802_11<T> { Payload = payload };
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out MSMon802_11 packet) => MSMon802_11.TryParse(buffer, out packet);
    }
}