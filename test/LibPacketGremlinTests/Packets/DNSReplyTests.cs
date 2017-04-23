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
    public class DNSReplyTests
    {

        [Fact]
        public void ParseAndSerializeAreEqual()
        {
            // Curiously, the output is different from the input at a byte level, but Wireshark says they are equivalent.
            byte[] rawBytes = {
                0x00, 0x02, 0x81, 0x80, 0x00, 0x01, 0x00, 0x01,
  0x00, 0x00, 0x00, 0x00, 0x0c, 0x6f, 0x75, 0x74,
  0x62, 0x72, 0x65, 0x61, 0x6b, 0x6c, 0x61, 0x62,
  0x73, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01,
  0x00, 0x01, 0xc0, 0x0c, 0x00, 0x01, 0x00, 0x01,
  0x00, 0x00, 0x03, 0xef, 0x00, 0x04, 0x60, 0x1f,
  0x21, 0x33
            };

            DNSReply packet;
            var parseResult = DNSReplyFactory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();
            rawBytes = packet.ToArray();

            parseResult = DNSReplyFactory.Instance.TryParse(rawBytes, out packet);
            packet.ToArray().SequenceEqual(rawBytes).Should().BeTrue();

        }
    }
}
