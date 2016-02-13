﻿namespace LibPacketGremlinTests.Packets
{
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using OutbreakLabs.LibPacketGremlin.Packets;
    using OutbreakLabs.LibPacketGremlin.Extensions;

    using Xunit;
    using OutbreakLabs.LibPacketGremlin.PacketFactories;
    public class WakeOnLanTests
    {
        [Fact]
        public void ParsesBasicFields()
        {
            byte[] rawBytes = {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
            };

            WakeOnLan packet;
            var parseResult = WakeOnLanFactory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();

            

            packet.DstMac.SequenceEqual(new byte[] { 0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7 }).Should()
                    .BeTrue();

        }

        [Fact]
        public void SerializesCorrectly()
        {
            WakeOnLan packet;
            WakeOnLanFactory.Instance.TryParse(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
            }, out packet).Should().BeTrue();

            
            using (var ms = new MemoryStream())
            {
                packet.WriteToStream(ms);                
                ms.ToArray()
                    .SequenceEqual(
                        new byte[]
                            {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
                ,0x00, 0x0f, 0xea, 0x81, 0xde, 0xd7
            })
                    .Should()
                    .BeTrue();
            }
        }

    }
}
