// -----------------------------------------------------------------------
//  <copyright file="IPacket.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Abstractions
{
    using System.IO;

    /// <summary>
    ///     The interface to which all packets must conform
    /// </summary>
    public interface IPacket
    {
        /// <summary>
        ///     Get the payload, if applicable
        /// </summary>
        IPacket Payload { get; }

        /// <summary>
        ///     Write the contents of this packet to a stream
        /// </summary>
        /// <param name="stream">Destination stream</param>
        void WriteToStream(Stream stream);

        /// <summary>
        ///     Correct fields such as checksums. Recursive.
        /// </summary>
        void CorrectFields();

        /// <summary>
        ///     Set the enclosing packet
        /// </summary>
        /// <param name="container">Packet that contains this one</param>
        void SetContainer(IPacket container);

        /// <summary>
        ///     Gets the length of the packet
        /// </summary>
        /// <returns>Length of the packet</returns>
        long Length();
    }
}