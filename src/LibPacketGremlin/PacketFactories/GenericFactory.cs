// -----------------------------------------------------------------------
//  <copyright file="GenericFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;

    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for producing Generic packets
    /// </summary>
    public class GenericFactory : PacketFactoryBase<Generic>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly GenericFactory Instance = new GenericFactory();

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>        
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public override bool TryParse(byte[] buffer, int index, int count, out Generic packet)
            => Generic.TryParse(buffer, index, count, out packet);

        /// <summary>
        /// Constructs a packet with default values
        /// </summary>
        /// <returns>A packet with default values</returns>
        public Generic Default()
        {
            return new Generic
            {
                Buffer = Array.Empty<byte>()
            };
        }

        /// <summary>
        /// Constructs a packet with the specified buffer
        /// </summary>
        /// <param name="buffer">Buffer to embed</param>
        /// <returns>A packet with the specified buffer</returns>
        public Generic Default(byte[] buffer)
        {
            return new Generic
            {
                Buffer = buffer
            };
        }
    }
}