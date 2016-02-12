// -----------------------------------------------------------------------
//  <copyright file="HardwareTypes.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.ARPSupport
{
    /// <summary>
    ///     Link Layer protocol type values for ARP
    /// </summary>
    public enum HardwareTypes : ushort
    {
        Ethernet = 1,

        IEEE802 = 6,

        ARCNET = 7,

        EthernetII = 8,

        Frame_Relay = 15,

        ATM = 16,

        HDLC = 17,

        Fibre_Channel = 18,

        ATM_Alt = 19,

        Serial = 20
    }
}