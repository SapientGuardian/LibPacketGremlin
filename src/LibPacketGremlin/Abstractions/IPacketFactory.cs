// -----------------------------------------------------------------------
//  <copyright file="IPacketFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Abstractions
{
    /// <summary>
    ///     Non-generic interface for packet factories
    /// </summary>
    public interface IPacketFactory
    {
        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="data">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        bool TryParse(byte[] data, out IPacket packet);
    }

    /// <summary>
    ///     Generic interface for packet factories
    /// </summary>
    /// <typeparam name="T">Type of packets produced by the factory</typeparam>
    public interface IPacketFactory<T> : IPacketFactory
        where T : IPacket
    {
        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="data">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        bool TryParse(byte[] data, out T packet);
    }
}