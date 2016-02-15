// -----------------------------------------------------------------------
//  <copyright file="KeyTypes.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.IEEE802_1xSupport
{
    /// <summary>
    /// Known 802.1x key types
    /// </summary>
    public enum KeyTypes : byte
    {
        RC4 = 1,

        WPA2 = 2,

        WPA = 254
    }
}