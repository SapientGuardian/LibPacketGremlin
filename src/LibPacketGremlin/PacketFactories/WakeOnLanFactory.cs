// -----------------------------------------------------------------------
//  <copyright file="WakeOnLanFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using System;

    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for producing WakeOnLan packets
    /// </summary>
    public class WakeOnLanFactory : PacketFactoryBase<WakeOnLan>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly WakeOnLanFactory Instance = new WakeOnLanFactory();

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>        
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public override bool TryParse(byte[] buffer, int index, int count, out WakeOnLan packet)
            => WakeOnLan.TryParse(buffer, index, count, out packet);

        /// <summary>
        /// Constructs a packet with default values
        /// </summary>
        /// <returns>A packet with default values</returns>
        public WakeOnLan Default()
        {
            return new WakeOnLan
            {
                DstMac = new byte[6],
                Password = Array.Empty<byte>()
            };
        }
    }
}