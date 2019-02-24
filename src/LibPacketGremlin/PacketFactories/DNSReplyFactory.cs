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
        ///     Constructs a packet with default values
        /// </summary>
        /// <returns>A packet with default values</returns>
        public DNSReply Default()
        {
            var reply = new DNSReply();
            reply.Id = RANDOM.Next(UInt16.MaxValue);
            return reply;
        }

        public override bool TryParse(ReadOnlySpan<byte> buffer, out DNSReply packet) => DNSReply.TryParse(buffer, out packet);
    }
}