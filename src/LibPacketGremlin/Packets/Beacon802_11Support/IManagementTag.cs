// -----------------------------------------------------------------------
//  <copyright file="IManagementTag.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.Beacon802_11Support
{
    using OutbreakLabs.LibPacketGremlin.Abstractions;

    /// <summary>
    ///     A marker interface for management tags
    /// </summary>
    public interface IManagementTag : IPacket
    {
        /// <summary>
        /// Gets a value indicating the type of tag
        /// </summary>
        byte TagType { get; }
    }
}