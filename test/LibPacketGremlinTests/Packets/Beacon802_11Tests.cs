namespace LibPacketGremlinTests.Packets
{
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using OutbreakLabs.LibPacketGremlin.Packets;
    using OutbreakLabs.LibPacketGremlin.Packets.Beacon802_11Support;
    using OutbreakLabs.LibPacketGremlin.PacketFactories;

    using Xunit;
    using OutbreakLabs.LibPacketGremlin.Extensions;
    public class Beacon802_11Tests
    {
        [Fact]
        public void ParsesBasicFields()
        {
            byte[] rawBytes = { 128, 65, 213, 3, 0, 0, 0, 0, 100, 0, 49, 4, 0, 31, 87, 105, 114, 101, 108, 101, 115, 115, 65, 99, 99, 101, 115, 115, 32, 40, 76, 101, 103, 97, 99, 121, 32, 68, 101, 118, 105, 99, 101, 115, 41, 1, 8, 130, 132, 139, 150, 140, 18, 152, 36, 3, 1, 6, 5, 4, 0, 1, 0, 0, 42, 1, 0, 48, 20, 1, 0, 0, 15, 172, 4, 1, 0, 0, 15, 172, 4, 1, 0, 0, 15, 172, 2, 0, 0, 50, 4, 176, 72, 96, 108, 221, 24, 0, 80, 242, 0, 0, 0, 221, 9, 0, 3, 127, 1, 1, 0, 0, 255, 127 };
            Beacon802_11 packet;
            var parseResult = Beacon802_11Factory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();
            
        }

        [Fact]
        public void SerializesCorrectly()
        {
            var packet =
                Beacon802_11Factory.Instance.ParseAs(
                    new byte[]
                        {
                            128, 65, 213, 3, 0, 0, 0, 0, 100, 0, 49, 4, 0, 31, 87, 105, 114, 101, 108, 101, 115, 115, 65,
                            99, 99, 101, 115, 115, 32, 40, 76, 101, 103, 97, 99, 121, 32, 68, 101, 118, 105, 99, 101, 115,
                            41, 1, 8, 130, 132, 139, 150, 140, 18, 152, 36, 3, 1, 6, 5, 4, 0, 1, 0, 0, 42, 1, 0, 48, 20, 1,
                            0, 0, 15, 172, 4, 1, 0, 0, 15, 172, 4, 1, 0, 0, 15, 172, 2, 0, 0, 50, 4, 176, 72, 96, 108, 221,
                            24, 0, 80, 242, 0, 0, 0, 221, 9, 0, 3, 127, 1, 1, 0, 0, 255, 127
                        });
            
            using (var ms = new MemoryStream())
            {
                packet.WriteToStream(ms);
                ms.ToArray()
                    .SequenceEqual(
                        new byte[]
                            { 128, 65, 213, 3, 0, 0, 0, 0, 100, 0, 49, 4, 0, 31, 87, 105, 114, 101, 108, 101, 115, 115, 65, 99, 99, 101, 115, 115, 32, 40, 76, 101, 103, 97, 99, 121, 32, 68, 101, 118, 105, 99, 101, 115, 41, 1, 8, 130, 132, 139, 150, 140, 18, 152, 36, 3, 1, 6, 5, 4, 0, 1, 0, 0, 42, 1, 0, 48, 20, 1, 0, 0, 15, 172, 4, 1, 0, 0, 15, 172, 4, 1, 0, 0, 15, 172, 2, 0, 0, 50, 4, 176, 72, 96, 108, 221, 24, 0, 80, 242, 0, 0, 0, 221, 9, 0, 3, 127, 1, 1, 0, 0, 255, 127 })
                    .Should()
                    .BeTrue();
            }
        }

    }
}
