namespace LibPacketGremlinTests.Packets
{
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using OutbreakLabs.LibPacketGremlin.Packets;
    using OutbreakLabs.LibPacketGremlin.Extensions;

    using Xunit;
    using OutbreakLabs.LibPacketGremlin.PacketFactories;
    public class ARPTests
    {
        [Fact]
        public void ParsesBasicFields()
        {
            byte[] rawBytes = { 0x00, 0x01, 0x08, 0x00, 0x06, 0x04, 0x00, 0x01, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0xc0, 0xa8, 0x01, 0x01, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xc0, 0xa8, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            ARP packet;
            var parseResult = ARPFactory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();

            
            packet.HLen.Should().Be(6);
            packet.HType.Should().Be(0x0001);
            packet.Operation.Should().Be(0x0001);
            packet.PLen.Should().Be(4);
            packet.PType.Should().Be(0x0800);
            packet.SenderHardwareAddress.SequenceEqual(new byte[] { 0x02, 0x02, 0x02, 0x02, 0x02, 0x02 }).Should()
                    .BeTrue();
            packet.SenderProtocolAddress.SequenceEqual(System.Net.IPAddress.Parse("192.168.1.1").GetAddressBytes()).Should()
                    .BeTrue();
            packet.TargetHardwareAddress.SequenceEqual(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }).Should()
                    .BeTrue();
            packet.TargetProtocolAddress.SequenceEqual(System.Net.IPAddress.Parse("192.168.1.1").GetAddressBytes()).Should()
                    .BeTrue();
        }

        [Fact]
        public void SerializesCorrectly()
        {
            ARP packet;
            ARPFactory.Instance.TryParse(new byte[] { 0x00, 0x01, 0x08, 0x00, 0x06, 0x04, 0x00, 0x01, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0xc0, 0xa8, 0x01, 0x01, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xc0, 0xa8, 0x01, 0x01 }, out packet).Should().BeTrue();

            
            using (var ms = new MemoryStream())
            {
                packet.WriteToStream(ms);                
                ms.ToArray()
                    .SequenceEqual(
                        new byte[]
                            { 0x00, 0x01, 0x08, 0x00, 0x06, 0x04, 0x00, 0x01, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0xc0, 0xa8, 0x01, 0x01, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xc0, 0xa8, 0x01, 0x01})
                    .Should()
                    .BeTrue();
            }
        }

        [Fact]
        public void CorrectsHType()
        {
            ARP packet = ARPFactory.Instance.Default();
            packet.HType.Should().Be(0); // Just making sure our test is valid.
            packet.SetContainer(EthernetIIFactory.Instance.Default(GenericFactory.Instance.Default()));
            packet.CorrectFields();
            packet.HType.Should().Be(8);

        }

        [Fact]
        public void CalculatesLength()
        {
            byte[] rawBytes = { 0x00, 0x01, 0x08, 0x00, 0x06, 0x04, 0x00, 0x01, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0xc0, 0xa8, 0x01, 0x01, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xc0, 0xa8, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            ARP packet;
            var parseResult = ARPFactory.Instance.TryParse(rawBytes, out packet);

            parseResult.Should().BeTrue();


            packet.Length().Should().Be(packet.ToArray().Length);
        }
    }
}
