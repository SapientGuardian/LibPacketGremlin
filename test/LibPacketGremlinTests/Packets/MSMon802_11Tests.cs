namespace LibPacketGremlinTests.Packets
{
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using OutbreakLabs.LibPacketGremlin.Packets;
    using OutbreakLabs.LibPacketGremlin.PacketFactories;

    using Xunit;
    using OutbreakLabs.LibPacketGremlin.Extensions;
    public class MSMon802_11Tests
    {
        [Fact]
        public void ParsesBasicFields()
        {
            byte[] rawBytes = { 2, 32, 0, 0, 0, 0, 128, 0, 0, 0, 0, 6, 0, 0, 0, 133, 9, 0, 0, 180, 255, 255, 255, 2, 253, 0, 28, 254, 32, 184, 207, 1, 72, 1, 202, 0, 0, 35, 105, 77, 105, 99, 0, 34, 192, 0, 14, 22, 0, 35, 105, 77, 105, 99, 208, 32 };
            MSMon802_11 packet;
            var parseResult = MSMon802_11Factory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();

        }

        [Fact]
        public void SerializesCorrectly()
        {
            MSMon802_11 packet;
            var parseResult = MSMon802_11Factory.Instance.TryParse(new byte[] { 2, 32, 0, 0, 0, 0, 128, 0, 0, 0, 0, 6, 0, 0, 0, 133, 9, 0, 0, 180, 255, 255, 255, 2, 253, 0, 28, 254, 32, 184, 207, 1, 72, 1, 202, 0, 0, 35, 105, 77, 105, 99, 0, 34, 192, 0, 14, 22, 0, 35, 105, 77, 105, 99, 208, 32 }, out packet);

            using (var ms = new MemoryStream())
            {
                packet.WriteToStream(ms);
                ms.ToArray()
                    .SequenceEqual(
                        new byte[]
                            { 2, 32, 0, 0, 0, 0, 128, 0, 0, 0, 0, 6, 0, 0, 0, 133, 9, 0, 0, 180, 255, 255, 255, 2, 253, 0, 28, 254, 32, 184, 207, 1, 72, 1, 202, 0, 0, 35, 105, 77, 105, 99, 0, 34, 192, 0, 14, 22, 0, 35, 105, 77, 105, 99, 208, 32 })
                    .Should()
                    .BeTrue();
            }
        }

        [Fact]
        public void CalculatesLength()
        {
            byte[] rawBytes = { 2, 32, 0, 0, 0, 0, 128, 0, 0, 0, 0, 6, 0, 0, 0, 133, 9, 0, 0, 180, 255, 255, 255, 2, 253, 0, 28, 254, 32, 184, 207, 1, 72, 1, 202, 0, 0, 35, 105, 77, 105, 99, 0, 34, 192, 0, 14, 22, 0, 35, 105, 77, 105, 99, 208, 32 };
            MSMon802_11 packet;
            var parseResult = MSMon802_11Factory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();

            packet.Length().Should().Be(packet.ToArray().Length);
        }
    }
}
