// -----------------------------------------------------------------------
//  <copyright file="SNAPFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    /// <summary>
    ///     Factory for parsing SNAP packets
    /// </summary>
    public class SNAPFactory : PacketFactoryBase<SNAP>
    {
        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly SNAPFactory Instance = new SNAPFactory();

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public override bool TryParse(byte[] buffer, int index, int count, out SNAP packet)
            => SNAP.TryParse(buffer, index, count, out packet);

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        /// <param name="payload">Payload</param>
        /// <returns>A Packet with default values</returns>
        public SNAP<T> Default<T>(T payload) where T : class, IPacket
        {
            return new SNAP<T> { OrganizationCode = new byte[3], Payload = payload };
        }
    }
}