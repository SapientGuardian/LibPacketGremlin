namespace LibPacketGremlinTests.Packets
{
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using OutbreakLabs.LibPacketGremlin.Packets;
    using OutbreakLabs.LibPacketGremlin.PacketFactories;

    using Xunit;
    using OutbreakLabs.LibPacketGremlin.Extensions;
    public class RadiotapTests
    {
        [Fact]
        public void ParsesBasicFields()
        {
            byte[] rawBytes = System.IO.File.ReadAllBytes("Resources\\radiotap.bin");
            Radiotap packet;
            var parseResult = RadiotapFactory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();
            packet.Version.Should().Be(0);
            packet.Pad.Should().Be(0);
            packet.Present.Should().Be(0x000018ee);
            packet.FieldData.SequenceEqual(new byte[] { 0x10, 0x02, 0x85, 0x09, 0xa0, 0x00, 0xe2, 0x9c, 0x64, 0x00, 0x00, 0x46 }).Should().BeTrue();
            (packet.Payload is IEEE802_11).Should().BeTrue();
        }

        [Fact]
        public void SerializesCorrectly()
        {
            byte[] rawBytes = System.IO.File.ReadAllBytes("Resources\\radiotap.bin");

            Radiotap packet;
            var parseResult = RadiotapFactory.Instance.TryParse(rawBytes, out packet);
            
            using (var ms = new MemoryStream())
            {
                packet.WriteToStream(ms);
                ms.ToArray()
                    .SequenceEqual(rawBytes)
                    .Should()
                    .BeTrue();
            }
        }

    }
}
