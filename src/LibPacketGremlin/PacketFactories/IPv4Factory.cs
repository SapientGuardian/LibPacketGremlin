// -----------------------------------------------------------------------
//  <copyright file="IPv4Factory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for producing IPv4 packets
    /// </summary>
    public class IPv4Factory : PacketFactoryBase<IPv4>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly IPv4Factory Instance = new IPv4Factory();

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>        
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public override bool TryParse(byte[] buffer, int index, int count, out IPv4 packet)
            => IPv4.TryParse(buffer, index, count, out packet);
    }
}