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
        /// <param name="data">Raw data to parse</param>
        /// <returns>New packet of specified type, or null if parsing failed</returns>
        public static T ParseAs<T>(this IPacketFactory<T> factory, byte[] data) where T : class, IPacket
        {
            T parsed;
            return factory.TryParse(data, out parsed) ? parsed : null;
        }
    }
}