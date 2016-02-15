# LibPacketGremlin
## Description
LibPacketGremlin is a library for manipulating packets on the .NET platform

## Motivation
LibPacketGremlin is a port of the core of PacketGremlin, an unreleased packet analyzer originally developed in 2006 that has gone through several refinements.
There exist other libraries for manipulating packets on .NET, but none that embrace generics to provide the sort of usage patterns that this library does.

## Status
LibPacketGremlin Supports the following packet types:

* ARP
* Ethernet II
* ICMP
* IEEE 802.11
* IEEE 802.11 Beacons
  * Partial support for MS vendor tags
  * SSID tag
* IEEE 802.1x
  * EAPoL Keys
* IPv4
* LLC
* MSMon 802.11
* Radiotap
* SNAP
* TCP
* UDP
* WakeOnLan

## Examples

### Parsing a byte array with TryParse
TryParse will attempt to parse the data as the specified packet type, returning true if successful, or false if not.
```C#
byte[] rawBytes = { 0x80, 0x00, 0x20, 0x7a, 0x3f, 0x3e, 0x80, 0x00, 0x20, 0x20, 0x3a, 0xae, 0x08, 0x00, 0xFF, 0xFF, 0xFF, 0xFF };
EthernetII packet;
var parseResult = EthernetIIFactory.Instance.TryParse(rawBytes, out packet);
```
### Parsing a byte array with ParseAs
ParseAs will attempt to parse the data as the specified packet type, returning the packet if succesful, or null if not.
```C#
byte[] rawBytes = { 0x80, 0x00, 0x20, 0x7a, 0x3f, 0x3e, 0x80, 0x00, 0x20, 0x20, 0x3a, 0xae, 0x08, 0x00, 0xFF, 0xFF, 0xFF, 0xFF };
EthernetII packet;
packet = EthernetIIFactory.Instance.ParseAs(rawBytes);
```

### Writing to a byte array
Packets have a WriteToStream method, which is trivially used with a MemoryStream to get a byte array.
```C#
byte[] packetBytes;

using (var ms = new MemoryStream())
{
	packet.WriteToStream(ms);
	packetBytes = ms.ToArray();
}
```

### Filtering to IPv4 UDP broadcast packets
This example assumes the use of SharpPcap to capture packets, creating an IObservable that we can use to get raw byte arrays of packets.
```C#
var obs = Observable.FromEventPattern<SharpPcap.PacketArrivalEventHandler, SharpPcap.CaptureEventArgs>(
                        ev => dev.OnPacketArrival += ev,
                        ev => dev.OnPacketArrival -= ev);
var udp4bcastPackets = from sharpPacketEvent in obs
let parsed = EthernetIIFactory.Instance.ParseAs(sharpPacketEvent.EventArgs.Packet.Data)
let layers = parsed?.Layers() ?? Enumerable.Empty<IPacket>()
let ipv4 = layers.OfType<IPv4>()?.FirstOrDefault()
where (layers.OfType<UDP>().Any()
    && layers.OfType<IPv4>().Any()
    && ipv4.DestAddress.GetAddressBytes().Last() == 255)
    select parsed;
```

## Attributions
The code from which LibPacketGremlin was taken was never intended to be open source.
As such, there may be some portions which were copied from Internet sources without attribution.
I've done my best to hunt these down, but apologies to anyone whose code shows up without it. If you recognize any such code, please file an issue.

Nearly all XML documentation descriptions for packet fields were taken from Wikipedia.

Most of the test data for 802.11 decryption use packets collected by [Daniel Smullen](http://www.daniel-smullen.com/).

Most of the code for wireless decryption was either inspired by or directly ported from the Aircrack-ng project.

Additional resources for wireless decryption: http://outbreaklabs.com/v2/post/wifi-decryption-resources-and-test-data

RC4 implementation was taken from http://www.idrix.fr/Root/Samples/rc4_derive_dotnet.cs

## License
LibPacketGremlin is released under the MIT license.