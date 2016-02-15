﻿namespace LibPacketGremlinTests.Extensions
{
    using System;

    using FluentAssertions;

    using OutbreakLabs.LibPacketGremlin.Extensions;
    using OutbreakLabs.LibPacketGremlin.Packets;

    using Xunit;
    using System.Linq;
    using OutbreakLabs.LibPacketGremlin.PacketFactories;
    using OutbreakLabs.LibPacketGremlin.Abstractions;
    using OutbreakLabs.LibPacketGremlin.Utilities;

    public class RC4Tests
    {
        [Fact]
        public void CanDecrypt()
        {
            var expected = new byte[] {
              0x72, 0xea, 0x7c, 0xf3, 0x62, 0xd0, 0x63, 0xb6, 0xf6, 0x3b, 0xd6, 0xfc, 0x1c, 0x6c, 0xc0, 0x18,
              0xd0, 0x10, 0x23, 0xd6, 0x86, 0x4e, 0x04, 0xf0, 0x0e, 0xc7, 0x34, 0xca, 0x66, 0x34, 0x01, 0xac,
              0x46, 0xd4, 0x7d, 0x15, 0x24, 0xa7, 0xaa, 0xb0, 0x06, 0x01, 0x4f, 0x9f, 0x61, 0x1c, 0x4e, 0x6d,
              0x54, 0x22, 0x83, 0x6f, 0x36, 0x43, 0x12, 0x01, 0x07, 0xb8, 0xdb, 0x3c, 0x32, 0xc6, 0xe2, 0x5f,
              0x66, 0x4e, 0x7e, 0x55, 0x4f, 0x5d, 0x61, 0x30, 0x8a, 0xb9, 0xb7, 0x11, 0xcb, 0xae, 0xe9, 0x68,
              0x66, 0x10, 0x7d, 0xe7, 0x39, 0x29, 0xef, 0xb0, 0x69, 0xca, 0xec, 0xcf, 0x2c, 0x68, 0x57, 0x52,
              0x62, 0x4f, 0x89, 0xd6, 0x58, 0x05, 0xe3, 0x6a, 0xa1, 0xd1, 0xc8, 0x2c, 0x0b, 0x2d, 0xe4, 0xc9,
              0x73, 0xd7, 0xb0, 0x4f, 0x65, 0x57, 0xa2, 0x19, 0xc7, 0xbf, 0xa1, 0xa7, 0x43, 0x49, 0x51, 0x14,
              0xa4, 0x12, 0x24, 0x15, 0xb1, 0x71, 0x39, 0xde, 0x5e, 0x95, 0x15, 0xb4, 0xb9, 0xb5, 0x61, 0x57,
              0xbf, 0x43, 0x24, 0xe2, 0xb8, 0x9f, 0x38, 0x2f, 0x45, 0x29, 0xfa, 0x95, 0x80, 0x18, 0x0d, 0x5d,
              0x41, 0x35, 0x2a, 0x83, 0x44, 0x37, 0x71, 0xcb, 0x80, 0x3d, 0x9d, 0xc4, 0xdc, 0xc4, 0x26, 0xea,
              0x28, 0xb9, 0xa7, 0x2b, 0x40, 0x56, 0x27, 0xee, 0xbc, 0xfa, 0xf7, 0x98, 0x09, 0x33, 0xd1, 0xe3,
              0x85, 0x79, 0x0d, 0xd8, 0x87, 0xa3, 0xfe, 0x8e, 0x37, 0xc4, 0x48, 0xbf, 0x66, 0x86, 0xfa, 0x49,
              0x0e, 0x0f, 0xf2, 0x82, 0x42, 0xfe, 0x87, 0xcd, 0x18, 0xb0, 0x8a, 0x91, 0xbb, 0x97, 0x9b, 0x4e,
              0x92, 0x36, 0xb2, 0x47, 0x12, 0xf0, 0xeb, 0x09, 0xdc, 0x7a, 0xdf, 0x60, 0xdf, 0xcd, 0x19, 0x7c,
              0x2d, 0x72, 0x4e, 0x3a, 0x6d, 0x8f, 0x53, 0x8e, 0xe1, 0x03, 0xeb, 0x44, 0x4a, 0x02, 0x9f, 0x52};
            var key = new byte[] {
              0x6E, 0x9C, 0x7A, 0x91, 0x9F, 0xB8, 0xAE, 0x93, 0xC1, 0xAB, 0x80, 0x3C, 0x09};
            var input = new byte[] {
              0x91, 0xCE, 0xFF, 0xF0, 0x9B, 0x76, 0xB1, 0xC7, 0xB3, 0xAE, 0xE9, 0xB6, 0x39, 0xE2, 0xE2, 0xCB,
              0x46, 0xBB, 0x20, 0xDA, 0xEF, 0x0D, 0xD8, 0x65, 0x75, 0x37, 0xFF, 0x8B, 0x78, 0x49, 0x39, 0x6E,
              0x08, 0x6D, 0x93, 0x0A, 0x40, 0xD1, 0xA7, 0xE6, 0x22, 0x6B, 0xFF, 0x94, 0x21, 0xA8, 0x0E, 0xDB,
              0x33, 0x2B, 0x4B, 0x88, 0x19, 0x6C, 0x94, 0x0E, 0xF3, 0xD8, 0xCC, 0xCB, 0xAF, 0x99, 0x89, 0x49,
              0x4F, 0x4B, 0x42, 0x4F, 0xF4, 0x96, 0xA6, 0xD3, 0xC9, 0x00, 0xB7, 0xC7, 0x11, 0x63, 0x95, 0x0E,
              0x29, 0x71, 0x0A, 0x38, 0xC2, 0x04, 0xDF, 0xB5, 0x01, 0xAA, 0xCA, 0x86, 0x47, 0x4A, 0xA3, 0x41,
              0xE6, 0x1A, 0x00, 0xA7, 0xD6, 0xFF, 0xE7, 0x89, 0x64, 0xB5, 0x38, 0x2A, 0x5D, 0xBC, 0x13, 0x94,
              0x8E, 0x0C, 0x93, 0xB6, 0xC8, 0x4E, 0x4F, 0xC6, 0x06, 0x9B, 0xEF, 0x9B, 0x56, 0xE3, 0x90, 0x54,
              0xCB, 0x34, 0x64, 0x0B, 0x3D, 0x12, 0x02, 0x62, 0xFE, 0xDC, 0xA8, 0x8E, 0x38, 0xCE, 0x36, 0x3A,
              0xE8, 0x4D, 0xF0, 0xED, 0x71, 0x59, 0xE5, 0xED, 0xA4, 0xB1, 0x12, 0xEB, 0xD5, 0x83, 0xA6, 0xC9,
              0x5C, 0x76, 0x98, 0x8D, 0x1B, 0xCB, 0x3C, 0x31, 0xF5, 0x10, 0xFE, 0x40, 0xCC, 0x83, 0x27, 0xE6,
              0xDF, 0xB9, 0xE1, 0x84, 0xFC, 0xC3, 0xCC, 0x8D, 0x7A, 0xA3, 0xE5, 0x61, 0x56, 0x92, 0xAD, 0x7C,
              0xA2, 0x62, 0x46, 0x6F, 0x5C, 0xA0, 0x16, 0xC4, 0x52, 0xDB, 0xF6, 0x75, 0xE1, 0x35, 0x22, 0x91,
              0xE8, 0x81, 0x87, 0x13, 0xC1, 0xC1, 0x0A, 0xB0, 0xBE, 0x20, 0xE7, 0xF5, 0x55, 0x55, 0x0C, 0xF3,
              0x99, 0x67, 0x53, 0xAF, 0x35, 0x15, 0xA9, 0x48, 0x72, 0xD8, 0xEB, 0x4A, 0xCF, 0x5B, 0xF5, 0xB6,
              0xBC, 0x30, 0x24, 0xCB, 0xFD, 0x6C, 0x5C, 0xF5, 0x62, 0x5C, 0xFD, 0xA5, 0x65, 0xD9, 0xD4, 0xD7};

            var rc4 = new RC4(key);
            var dest = new byte[input.Length];
            rc4.decrypt(input, input.Length, dest);
            expected.SequenceEqual(dest).Should().BeTrue();
        }
    }
}
