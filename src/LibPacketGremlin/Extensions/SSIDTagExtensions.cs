// -----------------------------------------------------------------------
//  <copyright file="SSIDTagExtensions.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Extensions
{
    using System.Text;

    using OutbreakLabs.LibPacketGremlin.Packets.Beacon802_11Support;

    /// <summary>
    /// Extension methods for SSIDTag
    /// </summary>
    public static class SSIDTagExtensions
    {
        /// <summary>
        ///     Gets the SSID as a string
        /// </summary>
        /// <param name="ssidTag">The SSID tag</param>
        /// <returns>The SSID as a string</returns>
        public static string SSIDString(this SSIDTag ssidTag)
        {
            return Encoding.UTF8.GetString(ssidTag.SSIDBytes, 0, ssidTag.SSIDBytes.Length);
        }

        /// <summary>
        ///     Sets the SSID with a string
        /// </summary>
        /// <param name="ssidTag">The SSID tag on which to set the value</param>
        /// <param name="value">The value to set the SSID</param>
        public static void SetSSID(this SSIDTag ssidTag, string value)
        {
            ssidTag.SSIDBytes = Encoding.UTF8.GetBytes(value);
        }
    }
}