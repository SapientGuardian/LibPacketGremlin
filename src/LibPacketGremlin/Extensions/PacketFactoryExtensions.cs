// -----------------------------------------------------------------------
//  <copyright file="PacketFactoryExtensions.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Extensions
{
    using OutbreakLabs.LibPacketGremlin.Abstractions;

    /// <summary>
    ///     Extensions for IPacketFactory
    /// </summary>
    public static class PacketFactoryExtensions
    {
        /// <summary>
        ///     Attempts to parse raw data as a packet
        /// </summary>
        /// <typeparam name="T">Type of packet to parse</typeparam>
        /// <param name="factory">Factory to use for parsing</param>
        /// <param name="buffer">Raw data to parse</param>
        /// <returns>New packet of specified type, or null if parsing failed</returns>
        public static T ParseAs<T>(this IPacketFactory<T> factory, byte[] buffer) where T : class, IPacket
        {
            T parsed;
            return factory.TryParse(buffer, out parsed) ? parsed : null;
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="factory">Factory to use for parsing</param>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public static bool TryParse(this IPacketFactory factory, byte[] buffer, out IPacket packet)
        {
            return factory.TryParse(buffer, 0, buffer.Length, out packet);
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <typeparam name="T">Type of packet to parse</typeparam>
        /// <param name="factory">Factory to use for parsing</param>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public static bool TryParse<T>(this IPacketFactory<T> factory, byte[] buffer, out T packet) where T : IPacket
        {
            return factory.TryParse(buffer, 0, buffer.Length, out packet);
        }
    }
}