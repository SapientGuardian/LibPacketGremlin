namespace LibPacketGremlinTests.Extensions
{
    using System;

    using FluentAssertions;

    using OutbreakLabs.LibPacketGremlin.Extensions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    using Xunit;
    using System.Linq;
    public class PacketExtensionsTests
    {
        [Fact]
        public void CanSerializeToArrray()
        {
            var packet = new Generic(new byte[] { 1, 2, 3, 4, 5 });
            packet.ToArray().SequenceEqual(new byte[] { 1, 2, 3, 4, 5 }).Should().BeTrue();
        }

        [Fact]
        public void CanDecomposeLayers()
        {
            var packet = new EthernetII<IPv4<UDP<Generic>>>(new IPv4<UDP<Generic>>(new UDP<Generic>(new Generic())));

            var layers = packet.Layers().ToArray();

            layers.Should().NotBeNull();
            layers.Should().HaveCount(4);
            (layers[0] is EthernetII).Should().BeTrue();
            (layers[1] is IPv4).Should().BeTrue();
            (layers[2] is UDP).Should().BeTrue();
            (layers[3] is Generic).Should().BeTrue();
        }

        [Fact]
        public void CanLocateSingleLayer()
        {
            var packet = new EthernetII<IPv4<UDP<Generic>>>(new IPv4<UDP<Generic>>(new UDP<Generic>(new Generic())));
            packet.Layer<UDP>().Should().NotBeNull();
        }

        [Fact]
        public void ShouldReturnNullIfLayerCannotBeLocated()
        {
            var packet = new EthernetII<IPv4<Generic>>(new IPv4<Generic>(new Generic()));
            packet.Layer<UDP>().Should().BeNull();
        }
    }
}
