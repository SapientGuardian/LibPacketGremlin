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
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        bool TryParse(byte[] buffer, int index, int count, out IPacket packet);
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
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        bool TryParse(byte[] buffer, int index, int count, out T packet);
    }
}