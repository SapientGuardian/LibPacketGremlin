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
    public class LLCFactoryTests
    {
        [Fact]
        public void GeneratesParseableDefault()
        {
            var defaultPacket = LLCFactory.Instance.Default(GenericFactory.Instance.Default());
            LLCFactory.Instance.ParseAs(defaultPacket.ToArray()).Should().NotBeNull();
        }
    }
}
