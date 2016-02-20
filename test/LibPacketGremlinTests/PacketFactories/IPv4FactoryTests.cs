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
    public class IPv4FactoryTests
    {
        [Fact]
        public void GeneratesParseableDefault()
        {
            var defaultPacket = IPv4Factory.Instance.Default(GenericFactory.Instance.Default());
            IPv4Factory.Instance.ParseAs(defaultPacket.ToArray()).Should().NotBeNull();
        }
    }
}
