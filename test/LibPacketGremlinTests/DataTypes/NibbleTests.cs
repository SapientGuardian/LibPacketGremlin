namespace LibPacketGremlinTests.DataTypes
{
    using System;

    using FluentAssertions;

    using OutbreakLabs.LibPacketGremlin.DataTypes;

    using Xunit;

    public class NibbleTests
    {
        [Fact]
        public void DefaultsTo0()
        {
            ((int)new Nibble()).Should().Be(0);
        }

        [Fact]
        public void AcceptsValuesUpTo15()
        {
            ((int)new Nibble(15)).Should().Be(15);
        }

        [Fact]
        public void ThrowsOverflowExceptionForValuesGTE16()
        {
            Assert.Throws<OverflowException>(() => new Nibble(16));
        }

        [Fact]
        public void ThrowsOverflowExceptionForNegativeValues()
        {
            Assert.Throws<OverflowException>(() => new Nibble(-1));
        }
    }
}
