using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace OutbreakLabs.LibPacketGremlin.Utilities
{
    internal ref struct SpanReader
    {
        private readonly ReadOnlySpan<byte> span;
        public int Position { get; set; }

        public SpanReader(ReadOnlySpan<byte> span)
        {
            this.span = span;
            this.Position = 0;
        }        

        public ReadOnlySpan<byte> Slice(int count)
        {
            var ret = span.Slice(Position, count);
            Position += count;
            return ret;
        }

        public ReadOnlySpan<byte> Slice()
        {
            var ret = span.Slice(Position);
            Position += ret.Length;
            return ret;
        }

        public byte[] ReadBytes(int count) => Slice(count).ToArray();

        public UInt16 ReadUInt16BigEndian() => BinaryPrimitives.ReadUInt16BigEndian(Slice(2));
        public UInt32 ReadUInt32BigEndian() => BinaryPrimitives.ReadUInt32BigEndian(Slice(4));    
        public UInt16 ReadUInt16LittleEndian() => BinaryPrimitives.ReadUInt16LittleEndian(Slice(2));
        public UInt32 ReadUInt32LittleEndian() => BinaryPrimitives.ReadUInt32LittleEndian(Slice(4));
        public UInt64 ReadUInt64LittleEndian() => BinaryPrimitives.ReadUInt64LittleEndian(Slice(8));
        public Int32 ReadInt32LittleEndian() => BinaryPrimitives.ReadInt32LittleEndian(Slice(4));

        public byte ReadByte()
        {
            var ret = span[Position];
            Position += 1;
            return ret;
        }
    }
}
