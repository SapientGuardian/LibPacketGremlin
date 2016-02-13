namespace LibPacketGremlinTests.Extensions
{
    using System;

    using FluentAssertions;

    using OutbreakLabs.LibPacketGremlin.Extensions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    using Xunit;
    using System.Linq;
    using OutbreakLabs.LibPacketGremlin.PacketFactories;
    public class PacketExtensionsTests
    {
        [Fact]
        public void CanSerializeToArrray()
        {
            var packet = GenericFactory.Instance.Default(new byte[] { 1, 2, 3, 4, 5 });
            packet.ToArray().SequenceEqual(new byte[] { 1, 2, 3, 4, 5 }).Should().BeTrue();
        }

        [Fact]
        public void CanDecomposeLayers()
        {
            var packet = EthernetIIFactory.Instance.Default(IPv4Factory.Instance.Default(UDPFactory.Instance.Default(GenericFactory.Instance.Default())));

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
            var packet = EthernetIIFactory.Instance.Default(IPv4Factory.Instance.Default(UDPFactory.Instance.Default(GenericFactory.Instance.Default())));
            packet.Layer<UDP>().Should().NotBeNull();
        }

        [Fact]
        public void ShouldReturnNullIfLayerCannotBeLocated()
        {
            var packet = EthernetIIFactory.Instance.Default(IPv4Factory.Instance.Default(GenericFactory.Instance.Default()));
            packet.Layer<UDP>().Should().BeNull();
        }
    }
}
