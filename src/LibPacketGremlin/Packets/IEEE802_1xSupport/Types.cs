// -----------------------------------------------------------------------
//  <copyright file="Types.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.IEEE802_1xSupport
{
    /// <summary>
    /// Known 802.1x packet types
    /// </summary>
    public enum Types : byte
    {
        EAP_PACKET = 0,

        EAPOL_START = 1,

        EAPOL_LOGOFF = 2,

        EAPOL_KEY = 3,

        EAPOL_ASF = 4
    }
}