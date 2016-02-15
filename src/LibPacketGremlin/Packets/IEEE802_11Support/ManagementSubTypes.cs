// -----------------------------------------------------------------------
//  <copyright file="ManagementSubTypes.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.IEEE802_11Support
{
    /// <summary>
    /// Known values for 802.11 management sub-types
    /// </summary>
    public enum ManagementSubTypes
    {
        AssociationRequest = 0,

        AssociationResponse = 1,

        ReassociationRequest = 2,

        ReassociationResponse = 3,

        ProbeRequest = 4,

        ProbeResponse = 5,

        Beacon = 8,

        ATIM = 9 //Announcement traffic indication map
        ,

        Disassociate = 10,

        Authentication = 11,

        Deauthentication = 12
        //    ,
        //ActionFrames = 13
        //    ,
        //BlockACKRequest = 24
        //    ,
        //BlockACK = 25
    }
}