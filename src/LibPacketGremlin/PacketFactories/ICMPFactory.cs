// -----------------------------------------------------------------------
//  <copyright file="ICMPFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for producing ICMP packets
    /// </summary>
    public class ICMPFactory : PacketFactoryBase<ICMP>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly ICMPFactory Instance = new ICMPFactory();

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="data">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public override bool TryParse(byte[] data, out ICMP packet) => ICMP.TryParse(data, out packet);
    }
}