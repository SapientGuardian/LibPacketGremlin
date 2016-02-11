// -----------------------------------------------------------------------
//  <copyright file="IPv4Address.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Packets.IPv4Support
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    ///     Restricted subclass of IPAddress for IPv4
    /// </summary>
    public class IPv4Address : IPAddress
    {
        public IPv4Address(byte[] address)
            : base(address)
        {
            if (address.Length != 4)
            {
                throw new ArgumentException();
            }
        }

        public IPv4Address(UInt32 newAddress)
            : base(newAddress)
        {
        }

        public IPv4Address(IPAddress newAddress)
            : base(newAddress.GetAddressBytes())
        {
        }

        public new static IPv4Address Parse(String address)
        {
            if (IPAddress.Parse(address).AddressFamily == AddressFamily.InterNetwork)
            {
                return new IPv4Address(IPAddress.Parse(address).GetAddressBytes());
            }
            throw new ArgumentException();
        }
    }
}