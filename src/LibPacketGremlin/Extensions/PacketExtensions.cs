// -----------------------------------------------------------------------
//  <copyright file="PacketExtensions.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Extensions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using OutbreakLabs.LibPacketGremlin.Abstractions;

    /// <summary>
    ///     Extensions for IPacket
    /// </summary>
    public static class PacketExtensions
    {
        /// <summary>
        ///     Serializes the packet to a byte array
        /// </summary>
        /// <param name="packet">Packet to serialize</param>
        /// <returns>Byte array serialization of packet</returns>
        public static byte[] ToArray(this IPacket packet)
        {
            byte[] output;
            using (var ms = new MemoryStream())
            {
                packet.WriteToStream(ms);
                output = ms.ToArray();
            }
            return output;
        }

        /// <summary>
        ///     Enumerates all layers of a packet
        /// </summary>
        /// <param name="packet">Packet to decompose</param>
        /// <returns>All layers of the packet</returns>
        public static IEnumerable<IPacket> Layers(this IPacket packet)
        {
            var nextLayer = packet;

            while (nextLayer != null)
            {
                yield return nextLayer;
                nextLayer = nextLayer.Payload;
            }
        }

        /// <summary>
        ///     Gets a specific layer of a packet
        /// </summary>
        /// <typeparam name="T">Type of layer to locate</typeparam>
        /// <param name="packet">Packet to search</param>
        /// <returns>Requested layer, or null if it is not found</returns>
        public static T Layer<T>(this IPacket packet) where T : IPacket
        {
            return packet.Layers().OfType<T>().SingleOrDefault();
        }
    }
}