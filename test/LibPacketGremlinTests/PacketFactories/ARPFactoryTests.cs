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
    public class ARPFactoryTests
    {
        [Fact]
        public void GeneratesParseableDefault()
        {
            var defaultPacket = ARPFactory.Instance.Default();
            ARPFactory.Instance.ParseAs(defaultPacket.ToArray()).Should().NotBeNull();
        }
    }
}
