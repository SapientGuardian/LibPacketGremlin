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
        /// <param name="data">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public override bool TryParse(byte[] data, out EthernetII packet) => EthernetII.TryParse(data, out packet);
    }
}