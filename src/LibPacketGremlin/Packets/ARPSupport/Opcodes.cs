// -----------------------------------------------------------------------
//  <copyright file="Opcodes.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.ARPSupport
{
    /// <summary>
    ///     Opcode values for ARP
    /// </summary>
    public enum Opcodes : short
    {
        ARP_Request = 1,

        ARP_Reply = 2,

        RARP_Request = 3,

        RARP_Reply = 4,

        DRARP_Request = 5,

        DRARP_Reply = 6,

        DRARP_Error = 7,

        InARP_Request = 8,

        InARP_Reply = 9
    }
}