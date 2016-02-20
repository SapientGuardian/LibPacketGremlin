using FluentAssertions;
using OutbreakLabs.LibPacketGremlin.Extensions;
using OutbreakLabs.LibPacketGremlin.PacketFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LibPacketGremlinTests.PacketFactories
{
    public class UDPFactoryTests
    {
        [Fact]
        public void GeneratesParseableDefault()
        {
            var defaultPacket = UDPFactory.Instance.Default(GenericFactory.Instance.Default());
            UDPFactory.Instance.ParseAs(defaultPacket.ToArray()).Should().NotBeNull();
        }
    }
}
