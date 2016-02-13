// -----------------------------------------------------------------------
//  <copyright file="EthernetIIFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for parsing EthernetII packets
    /// </summary>
    public class EthernetIIFactory : PacketFactoryBase<EthernetII>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly EthernetIIFactory Instance = new EthernetIIFactory();

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>        
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public override bool TryParse(byte[] buffer, int index, int count, out EthernetII packet)
            => EthernetII.TryParse(buffer, index, count, out packet);
    }
}