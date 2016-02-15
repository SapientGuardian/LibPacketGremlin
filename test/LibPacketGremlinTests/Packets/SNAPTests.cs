﻿namespace LibPacketGremlinTests.Packets
{
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using OutbreakLabs.LibPacketGremlin.Packets;


    using Xunit;
    using OutbreakLabs.LibPacketGremlin.PacketFactories;
    using OutbreakLabs.LibPacketGremlin.Extensions;
    public class SNAPTests
    {
        [Fact]
        public void ParsesBasicFields()
        {
            byte[] rawBytes = { 0x00, 0x00, 0x00, 0x88, 0x8e, 0x02, 0x03, 0x00, 0x5f, 0x02, 0x00, 0x8a, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x72, 0xd3, 0xd5, 0x5d, 0xeb, 0x88, 0xf2, 0xf9, 0x3a, 0x73, 0x6d, 0x0b, 0xdd, 0x02, 0x0b, 0x38, 0x42, 0xe4, 0x48, 0xe1, 0x25, 0xc4, 0x93, 0x0a, 0xab, 0x75, 0x0b, 0x85, 0x9e, 0xb7, 0xb6, 0xe4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            SNAP packet;
            var parseResult = SNAPFactory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();
        }

        [Fact]
        public void SerializesCorrectly()
        {
            SNAP packet;
            SNAPFactory.Instance.TryParse(new byte[] { 0x00, 0x00, 0x00, 0x88, 0x8e, 0x02, 0x03, 0x00, 0x5f, 0x02, 0x00, 0x8a, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x72, 0xd3, 0xd5, 0x5d, 0xeb, 0x88, 0xf2, 0xf9, 0x3a, 0x73, 0x6d, 0x0b, 0xdd, 0x02, 0x0b, 0x38, 0x42, 0xe4, 0x48, 0xe1, 0x25, 0xc4, 0x93, 0x0a, 0xab, 0x75, 0x0b, 0x85, 0x9e, 0xb7, 0xb6, 0xe4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            , out packet).Should().BeTrue();


            using (var ms = new MemoryStream())
            {
                packet.WriteToStream(ms);
                ms.ToArray()
                    .SequenceEqual(
                        new byte[]
                            { 0x00, 0x00, 0x00, 0x88, 0x8e, 0x02, 0x03, 0x00, 0x5f, 0x02, 0x00, 0x8a, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x72, 0xd3, 0xd5, 0x5d, 0xeb, 0x88, 0xf2, 0xf9, 0x3a, 0x73, 0x6d, 0x0b, 0xdd, 0x02, 0x0b, 0x38, 0x42, 0xe4, 0x48, 0xe1, 0x25, 0xc4, 0x93, 0x0a, 0xab, 0x75, 0x0b, 0x85, 0x9e, 0xb7, 0xb6, 0xe4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })
                    .Should()
                    .BeTrue();
            }
        }
    }
}