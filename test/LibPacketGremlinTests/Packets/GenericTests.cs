namespace LibPacketGremlinTests.Packets
{
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using OutbreakLabs.LibPacketGremlin.Packets;

    using Xunit;
    using OutbreakLabs.LibPacketGremlin.PacketFactories;
    using OutbreakLabs.LibPacketGremlin.Extensions;
    public class GenericTests
    {
        [Fact]
        public void ParsesBasicFields()
        {
            byte[] rawBytes = { 0x80, 0x00, 0x20, 0x7a, 0x3f, 0x3e, 0x80, 0x00, 0x20, 0x20, 0x3a, 0xae, 0x08, 0x00, 0xFF, 0xFF, 0xFF, 0xFF };
            Generic packet;
            var parseResult = GenericFactory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();
            packet.Data.SequenceEqual(
                        new byte[]
                            {
                                0x80, 0x00, 0x20, 0x7a, 0x3f, 0x3e, 0x80, 0x00, 0x20, 0x20, 0x3a, 0xae, 0x08, 0x00, 0xFF,
                                0xFF, 0xFF, 0xFF
                            })
                    .Should()
                    .BeTrue();
        }

        [Fact]
        public void SerializesCorrectly()
        {
            var packet =
                new Generic(
                    new byte[]
                        {
                            0x80, 0x00, 0x20, 0x7a, 0x3f, 0x3e, 0x80, 0x00, 0x20, 0x20, 0x3a, 0xae, 0x08, 0x00, 0xFF, 0xFF,
                            0xFF, 0xFF
                        });

            using (var ms = new MemoryStream())
            {
                packet.WriteToStream(ms);
                ms.ToArray()
                    .SequenceEqual(
                        new byte[]
                            {
                                0x80, 0x00, 0x20, 0x7a, 0x3f, 0x3e, 0x80, 0x00, 0x20, 0x20, 0x3a, 0xae, 0x08, 0x00, 0xFF,
                                0xFF, 0xFF, 0xFF
                            })
                    .Should()
                    .BeTrue();
            }
        }
    }
}
