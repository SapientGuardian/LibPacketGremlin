using FluentAssertions;
using OutbreakLabs.LibPacketGremlin.Extensions;
using OutbreakLabs.LibPacketGremlin.PacketFactories;
using OutbreakLabs.LibPacketGremlin.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace LibPacketGremlinTests.Packets
{
    public class DNSQueryTests
    {

        [Fact]
        public void ParseAndSerializeAreEqual()
        {
            byte[] rawBytes = { 
                0x00, 0x02, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00,
  0x00, 0x00, 0x00, 0x00, 0x0c, 0x6f, 0x75, 0x74,
  0x62, 0x72, 0x65, 0x61, 0x6b, 0x6c, 0x61, 0x62,
  0x73, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01,
  0x00, 0x01
            };

            DNSQuery packet;
            var parseResult = DNSQueryFactory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();

            packet.ToArray().SequenceEqual(rawBytes).Should().BeTrue();

        }
    }
}
