// -----------------------------------------------------------------------
//  <copyright file="ByteOrder.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Utilities
{
    using System.Net;

    /// <summary>
    ///     Functions for manipulating byte order. I think I got these from
    ///     http://blogs.msdn.com/b/jeremykuhne/archive/2005/07/21/441247.aspx
    /// </summary>
    internal class ByteOrder
    {
        /// <summary>
        ///     Converts a value to big-endian
        /// </summary>
        /// <param name="host">Value in little-endian</param>
        /// <returns>Value in big-endian</returns>
        public static short HostToNetworkOrder(short host)
        {
            return (short)(((host & 0xFF) << 8) | ((host >> 8) & 0xFF));
        }

        /// <summary>
        ///     Converts a value to little-endian
        /// </summary>
        /// <param name="host">Value in big-endian</param>
        /// <returns>Value in little-endian</returns>
        public static short NetworkToHostOrder(short host)
        {
            return (short)(((host & 0xFF) << 8) | ((host >> 8) & 0xFF));
        }

        /// <summary>
        ///     Converts a value to big-endian
        /// </summary>
        /// <param name="host">Value in little-endian</param>
        /// <returns>Value in big-endian</returns>
        public static int HostToNetworkOrder(int host)
        {
            return ((IPAddress.HostToNetworkOrder((short)host) & 0xffff) << 0x10)
                   | (IPAddress.HostToNetworkOrder((short)(host >> 0x10)) & 0xffff);
        }

        /// <summary>
        ///     Converts a value to big-endian
        /// </summary>
        /// <param name="host">Value in little-endian</param>
        /// <returns>Value in big-endian</returns>
        public static long HostToNetworkOrder(long host)
        {
            return ((IPAddress.HostToNetworkOrder((int)host) & 0xffffffff) << 0x20)
                   | (IPAddress.HostToNetworkOrder((int)(host >> 0x20)) & 0xffffffff);
        }

        /// <summary>
        ///     Converts a value to big-endian
        /// </summary>
        /// <param name="source">Value in little-endian</param>
        /// <returns>Value in big-endian</returns>
        public static uint HostToNetworkOrder(uint source)
        {
            return ((source & 0x000000FF) << 24) | ((source & 0x0000FF00) << 8) | ((source & 0x00FF0000) >> 8)
                   | ((source & 0xFF000000) >> 24);
        }

        /// <summary>
        ///     Converts a value to little-endian
        /// </summary>
        /// <param name="source">Value in big-endian</param>
        /// <returns>Value in little-endian</returns>
        public static uint NetworkToHostOrder(uint source)
        {
            return ((source & 0x000000FF) << 24) | ((source & 0x0000FF00) << 8) | ((source & 0x00FF0000) >> 8)
                   | ((source & 0xFF000000) >> 24);
        }

        /// <summary>
        ///     Converts a value to big-endian
        /// </summary>
        /// <param name="host">Value in little-endian</param>
        /// <returns>Value in big-endian</returns>
        public static ushort HostToNetworkOrder(ushort host)
        {
            return (ushort)(((host & 0xFF) << 8) | ((host >> 8) & 0xFF));
        }

        /// <summary>
        ///     Converts a value to little-endian
        /// </summary>
        /// <param name="host">Value in big-endian</param>
        /// <returns>Value in little-endian</returns>
        public static ushort NetworkToHostOrder(ushort host)
        {
            return (ushort)(((host & 0xFF) << 8) | ((host >> 8) & 0xFF));
        }
    }
}