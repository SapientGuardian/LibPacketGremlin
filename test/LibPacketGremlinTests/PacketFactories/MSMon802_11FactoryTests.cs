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
    public class MSMon802_11FactoryTests
    {
        [Fact]
        public void GeneratesParseableDefault()
        {
            var defaultPacket = MSMon802_11Factory.Instance.Default(GenericFactory.Instance.Default());
            MSMon802_11Factory.Instance.ParseAs(defaultPacket.ToArray()).Should().NotBeNull();
        }
    }
}
