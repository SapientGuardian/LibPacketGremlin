// -----------------------------------------------------------------------
//  <copyright file="PacketFactoryBase.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;
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
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public virtual bool TryParse(byte[] buffer, int index, int count, out T packet)
            => this.TryParse(buffer.AsSpan(index, count), out packet);

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public virtual bool TryParse(byte[] buffer, int index, int count, out IPacket packet) 
            => this.TryParse(buffer.AsSpan(index, count), out packet);
        

        public abstract bool TryParse(ReadOnlySpan<byte> buffer, out T packet);
        public bool TryParse(ReadOnlySpan<byte> buffer, out IPacket packet)
        {
            T parsed;
            var result = this.TryParse(buffer, out parsed);
            packet = parsed;
            return result;
        }
    }
}