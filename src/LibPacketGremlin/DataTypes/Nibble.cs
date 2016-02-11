// -----------------------------------------------------------------------
//  <copyright file="Nibble.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace OutbreakLabs.LibPacketGremlin.DataTypes
{
    using System;

    /// <summary>
    ///     4-bit data type
    /// </summary>
    public struct Nibble
    {
        /// <summary>
        ///     Internal storage
        /// </summary>
        private readonly byte storage;

        /// <summary>
        ///     Conversion from int to Nibble
        /// </summary>
        /// <param name="val">Value to convert</param>
        public static implicit operator Nibble(int val)
        {
            return new Nibble(val);
        }

        /// <summary>
        ///     Conversion from Nibble to int
        /// </summary>
        /// <param name="val">Nibble to convert</param>
        public static implicit operator int(Nibble val)
        {
            return val.storage;
        }

        /// <summary>
        ///     Byte constructor
        /// </summary>
        /// <param name="val">Source value</param>
        public Nibble(byte val)
        {
            if (val > 15)
            {
                throw new OverflowException();
            }

            this.storage = val;
        }

        /// <summary>
        ///     Int constructor
        /// </summary>
        /// <param name="val">Source value</param>
        public Nibble(int val)
        {
            if (val > 15 || val < 0)
            {
                throw new OverflowException();
            }

            this.storage = (byte)val;
        }
    }
}