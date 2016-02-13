// -----------------------------------------------------------------------
//  <copyright file="ARPFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for producing ARP packets
    /// </summary>
    public class ARPFactory : PacketFactoryBase<ARP>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly ARPFactory Instance = new ARPFactory();

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>        
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public override bool TryParse(byte[] buffer, int index, int count, out ARP packet)
            => ARP.TryParse(buffer, index, count, out packet);

        /// <summary>
        /// Constructs a packet with default values
        /// </summary>
        /// <returns>A packet with default values</returns>
        public ARP Default()
        {
            return new ARP
                       {
                           HType = 0,
                           PType = 0,
                           HLen = 0,
                           PLen = 0,
                           Operation = 0,
                           SenderHardwareAddress = new byte[0],
                           SenderProtocolAddress = new byte[0],
                           TargetHardwareAddress = new byte[0],
                           TargetProtocolAddress = new byte[0],
                       };
        }
    }
}