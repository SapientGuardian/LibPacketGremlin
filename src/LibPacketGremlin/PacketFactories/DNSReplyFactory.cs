// -----------------------------------------------------------------------
//  <copyright file="DNSReplyFactory.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.PacketFactories
{
    using DNS.Protocol;
    using OutbreakLabs.LibPacketGremlin.Packets;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Factory for producing DNS Reply packets
    /// </summary>
    public class DNSReplyFactory : PacketFactoryBase<DNSReply>
    {
        private static readonly Random RANDOM = new Random();

        /// <summary>
        ///     Convenience instance
        /// </summary>
        public static readonly DNSReplyFactory Instance = new DNSReplyFactory();

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        public override bool TryParse(byte[] buffer, int index, int count, out DNSReply packet)
            => DNSReply.TryParse(buffer, index, count, out packet);

        /// <summary>
        ///     Constructs a packet with default values
        /// </summary>
        /// <returns>A packet with default values</returns>
        public DNSReply Default()
        {
            var reply = new DNSReply();
            reply.Id = RANDOM.Next(UInt16.MaxValue);
            return reply;
        }
    }
}