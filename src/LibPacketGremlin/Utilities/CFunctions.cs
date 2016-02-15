// -----------------------------------------------------------------------
//  <copyright file="CFunctions.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.Utilities
{
    /// <summary>
    ///     Functions designed to mirror C
    /// </summary>
    public class CFunctions
    {
        /// <summary>
        /// Performs a bitwise XOR between two arrays
        /// </summary>
        /// <param name="dst">Array to change</param>
        /// <param name="dstOffset">Offset into the destination array</param>
        /// <param name="src">Source array</param>
        /// <param name="len">Number of bytes to XOR</param>
        internal static void XOR(byte[] dst, int dstOffset, byte[] src, int len)
        {
            for (var i = dstOffset; i < len + dstOffset; i++)
            {
                dst[i] ^= src[i - dstOffset];
            }
        }

        /// <summary>
        /// Port of C's memcmp, compares the bytes in the two supplied array
        /// </summary>
        /// <param name="b1">First array</param>
        /// <param name="b2">Second array</param>
        /// <returns>Returns an integer less than, equal to, or
        /// greater than zero if the bytes of b1 are found, respectively,
        /// to be less than, to match, or be greater than the bytes of
        /// b2.</returns>
        internal static int memcmp(byte[] b1, byte[] b2)
        {
            for (var i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    if ((b1[i] >= 0 && b2[i] >= 0) || (b1[i] < 0 && b2[i] < 0))
                    {
                        return b1[i] - b2[i];
                    }
                    if (b1[i] < 0 && b2[i] >= 0)
                    {
                        return 1;
                    }
                    if (b2[i] < 0 && b1[i] >= 0)
                    {
                        return -1;
                    }
                }
            }
            return 0;
        }
    }
}