// -----------------------------------------------------------------------
//  <copyright file="PacketFactoryBase.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using OutbreakLabs.LibPacketGremlin.Abstractions;

    /// <summary>
    ///     Provides linkage from generic parse to IPacket parse
    /// </summary>
    /// <typeparam name="T">The type of the packets produced by the factory</typeparam>
    public abstract class PacketFactoryBase<T> : IPacketFactory<T>
        where T : IPacket
    {
        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="data">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public abstract bool TryParse(byte[] data, out T packet);

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="data">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public bool TryParse(byte[] data, out IPacket packet)
        {
            T parsed;
            var result = this.TryParse(data, out parsed);
            packet = parsed;
            return result;
        }
    }
}