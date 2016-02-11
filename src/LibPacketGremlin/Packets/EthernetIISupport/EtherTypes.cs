// -----------------------------------------------------------------------
//  <copyright file="EtherTypes.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.EthernetIISupport
{
    /// <summary>
    ///     EtherType values for EthernetII
    /// </summary>
    public enum EtherTypes : ushort
    {
        IPv4 = 0x0800,

        ARP = 0x0806,

        WakeOnLAN = 0x0842,

        SYN3 = 0x1337,

        RARP = 0x8035,

        AppleTalk = 0x809B,

        AARP = 0x80F3,

        VLAN_tagged_frame = 0x8100,

        IPX_alt = 0x8137,

        Novell = 0x8138,

        IPv6 = 0x86DD,

        MAC_control = 0x8808,

        Slow_protocols = 0x8809,

        CobraNet = 0x8819,

        MPLS_unicast = 0x8847,

        MPLS_multicast = 0x8848,

        PPPoE_discovery_stage = 0x8863,

        PPPoE_session_stage = 0x8864,

        NLB_heartbeat = 0x886F,

        Jumbo_frames = 0x8870,

        EAPoLAN = 0x888E,

        PROFINET = 0x8892,

        HyperSCSI = 0x889A,

        ATAoE = 0x88A2,

        EtherCAT = 0x88A4,

        Provider_bridging = 0x88A8,

        Ethernet_Powerlink = 0x88AB,

        LLDP = 0x88CC,

        SERCOS_III = 0x88CD,

        CESoE = 0x88D8,

        HomePlug = 0x88E1,

        MAC_security = 0x88E5,

        Precision_time = 0x88F7,

        CFM = 0x8902,

        FCoE = 0x8914,

        QinQ = 0x9100,

        VLLT = 0xCAFE
    }
}