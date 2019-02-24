// -----------------------------------------------------------------------
//  <copyright file="RadiotapFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for parsing Radiotap packets
    /// </summary>
    public class RadiotapFactory : PacketFactoryBase<Radiotap>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly RadiotapFactory Instance = new RadiotapFactory();

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        /// <param name="payload">Payload</param>
        /// <returns>A Packet with default values</returns>
        public Radiotap<T> Default<T>(T payload) where T : class, IPacket
        {
            return new Radiotap<T>
            {
                FieldData = Array.Empty<byte>(),
                LengthRadiotap = 8,
                Payload = payload
            };
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out Radiotap packet) => Radiotap.TryParse(buffer, out packet);
    }
}