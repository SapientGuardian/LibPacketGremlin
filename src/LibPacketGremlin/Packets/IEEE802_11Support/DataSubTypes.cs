// -----------------------------------------------------------------------
//  <copyright file="DataSubTypes.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.IEEE802_11Support
{
    /// <summary>
    /// Known sub-types for 802.11 data
    /// </summary>
    public enum DataSubTypes
    {
        Data = 0,

        DataContentionFreeACK = 1,

        DataContentionFreePoll = 2,

        DataContentionFreeAckPoll = 3,

        NullData = 4,

        NullDataContentionFreeAck = 5,

        NullDataContentionFreePoll = 6,

        NullDataContentionFreeAckPoll = 7,

        QoS = 8
    }
}