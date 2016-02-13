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
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>        
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public override bool TryParse(byte[] buffer, int index, int count, out ICMP packet)
            => ICMP.TryParse(buffer, index, count, out packet);

        /// <summary>
        /// Constructs a packet with default values
        /// </summary>
        /// <returns>A packet with default values</returns>
        public ICMP Default()
        {
            return new ICMP
            {
                Type = 0,
                Code = 0,
                Checksum = 0,
                ID = 0,
                Sequence = 0,
                Data = new byte[0]
            };
        }
    }
}