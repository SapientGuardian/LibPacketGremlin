// -----------------------------------------------------------------------
//  <copyright file="TCPFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;

    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for producing TCP packets
    /// </summary>
    public class TCPFactory : PacketFactoryBase<TCP>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly TCPFactory Instance = new TCPFactory();

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        /// <param name="payload">Payload</param>
        /// <returns>A Packet with default values</returns>
        public TCP<T> Default<T>(T payload) where T : class, IPacket
        {
            var packet = new TCP<T>
                       {
                OptionsAndPadding = Array.Empty<byte>(),
                Payload = payload
                       };

            packet.CorrectFields();
            return packet;
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out TCP packet) => TCP.TryParse(buffer, out packet);
    }
}