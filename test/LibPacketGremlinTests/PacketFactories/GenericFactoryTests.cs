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
    public class GenericFactoryTests
    {
        [Fact]
        public void GeneratesParseableDefault()
        {
            var defaultPacket = GenericFactory.Instance.Default();
            GenericFactory.Instance.ParseAs(defaultPacket.ToArray()).Should().NotBeNull();
        }
    }
}
