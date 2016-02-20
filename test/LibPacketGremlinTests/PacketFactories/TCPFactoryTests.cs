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
    public class TCPFactoryTests
    {
        [Fact]
        public void GeneratesParseableDefault()
        {
            var defaultPacket = TCPFactory.Instance.Default(GenericFactory.Instance.Default());
            TCPFactory.Instance.ParseAs(defaultPacket.ToArray()).Should().NotBeNull();
        }
    }
}
