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
        ///     Constructs a packet with default values
        /// </summary>
        /// <returns>A packet with default values</returns>
        public Generic Default()
        {
            return new Generic { Buffer = Array.Empty<byte>() };
        }

        /// <summary>
        ///     Constructs a packet with the specified buffer
        /// </summary>
        /// <param name="buffer">Buffer to embed</param>
        /// <returns>A packet with the specified buffer</returns>
        public Generic Default(byte[] buffer)
        {
            return new Generic { Buffer = buffer };
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out Generic packet) => Generic.TryParse(buffer, out packet);
    }
}