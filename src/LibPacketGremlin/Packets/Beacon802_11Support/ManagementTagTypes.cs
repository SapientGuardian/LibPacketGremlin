// -----------------------------------------------------------------------
//  <copyright file="ManagementTagTypes.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.Beacon802_11Support
{
    /// <summary>
    /// Known values for management tag types
    /// </summary>
    public enum ManagementTagTypes : byte
    {
        SSID = 0,

        SupportedDataRates = 1,

        FrequencyHoppingCHannelSet = 2,

        DirectSequenceChannelSet = 3,

        ContentionFreePeriod = 4,

        TrafficIndicationMap = 5,

        IBSSAdhocParameterSet = 6,

        CountryInformation = 7,

        RSNInformationElement = 0x30,

        CiscoCCXExt1 = 0x85,

        CiscoCCXExt2 = 0x88,

        CiscoCCXExt3 = 0x95,

        HighThroughput11nCapability = 0x2d,

        APNeighborReport = 0x34,

        HighThrouput11nInformation = 0x3d,

        QoSCapability = 0x2e,

        TransmitPowerControlRequest = 0x22,

        TransmitPowerControlResponse = 0x23,

        SupportedChannels = 0x24,

        ExtendedSupportedDataRates = 0x32,

        VendorSpecific = 0xdd
    }
}