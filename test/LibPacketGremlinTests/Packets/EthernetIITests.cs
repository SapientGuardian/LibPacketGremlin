namespace LibPacketGremlinTests.Packets
{
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using OutbreakLabs.LibPacketGremlin.Packets;
    using OutbreakLabs.LibPacketGremlin.Packets.EthernetIISupport;
    using OutbreakLabs.LibPacketGremlin.PacketFactories;

    using Xunit;
    using OutbreakLabs.LibPacketGremlin.Extensions;
    public class EthernetIITests
    {
        [Fact]
        public void ParsesBasicFields()
        {
            byte[] rawBytes = { 0x80, 0x00, 0x20, 0x7a, 0x3f, 0x3e, 0x80, 0x00, 0x20, 0x20, 0x3a, 0xae, 0x08, 0x00, 0xFF, 0xFF, 0xFF, 0xFF };
            EthernetII packet;
            var parseResult = EthernetIIFactory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();
            packet.DstMac.SequenceEqual(new byte[] { 0x80, 0x00, 0x20, 0x7a, 0x3f, 0x3e }).Should().BeTrue();
            packet.SrcMac.SequenceEqual(new byte[] { 0x80, 0x00, 0x20, 0x20, 0x3a, 0xae }).Should().BeTrue();
            packet.EtherType.Should().Be(0x0800);
        }

        [Fact]
        public void SerializesCorrectly()
        {
            var packet = EthernetIIFactory.Instance.Default(GenericFactory.Instance.Default(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }));
            packet.DstMac = new byte[] { 0x80, 0x00, 0x20, 0x7a, 0x3f, 0x3e };
            packet.SrcMac = new byte[] { 0x80, 0x00, 0x20, 0x20, 0x3a, 0xae };
            packet.EtherType = 0x0800;
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

        [Fact]
        public void CorrectsEtherType()
        {
            var packet = EthernetIIFactory.Instance.Default(IPv4Factory.Instance.Default(GenericFactory.Instance.Default()));

            packet.EtherType.Should().Be(0); // Just making sure the test is valid.
            packet.CorrectFields();
            packet.EtherType.Should().Be((ushort)(EtherTypes.IPv4));
        }

        [Fact]
        public void CalculatesLength()
        {
            byte[] rawBytes = { 0x80, 0x00, 0x20, 0x7a, 0x3f, 0x3e, 0x80, 0x00, 0x20, 0x20, 0x3a, 0xae, 0x08, 0x00, 0xFF, 0xFF, 0xFF, 0xFF };
            EthernetII packet;
            var parseResult = EthernetIIFactory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();
            packet.Length().Should().Be(packet.ToArray().Length);
        }
    }
}
