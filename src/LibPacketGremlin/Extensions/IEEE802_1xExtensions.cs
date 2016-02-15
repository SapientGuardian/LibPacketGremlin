// -----------------------------------------------------------------------
//  <copyright file="IEEE802_1xExtensions.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Extensions
{
    using System.Linq;
    using System.Security.Cryptography;

    using OutbreakLabs.LibPacketGremlin.Packets;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    /// <summary>
    ///     Extensions for IEEE802_1x
    /// </summary>
    public static class IEEE802_1xExtensions
    {
        /// <summary>
        ///     Checks whether the specified pairwise transit key is valid for the EAPoL key
        /// </summary>
        /// <param name="eapolKey">The EAPoL key to check against</param>
        /// <param name="ptk">The pairwise transit key to check</param>
        /// <returns>True if the key is valid, false if it is not</returns>
        public static bool PtkIsValid(this IEEE802_1x<EapolKey> eapolKey, byte[] ptk)
            => IEEE802_11Crypto.PtkIsValid(eapolKey, ptk);
    }
}