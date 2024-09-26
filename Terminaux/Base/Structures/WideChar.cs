﻿//
// Terminaux  Copyright (C) 2023-2024  Aptivi
//
// This file is part of Terminaux
//
// Terminaux is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Terminaux is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY, without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using System;
using System.Diagnostics;

namespace Terminaux.Base.Structures
{
    /// <summary>
    /// Wide character representation that represents 4 bytes for a 32-bit character
    /// </summary>
    [Serializable]
    [DebuggerDisplay("'{ToString(),nq}' [Hi: {(int)high}, Lo: {(int)low}]")]
    public struct WideChar : IEquatable<WideChar>
    {
        internal char high = '\0';
        internal char low = '\0';

        /// <summary>
        /// Returns a string instance of this wide character
        /// </summary>
        /// <returns>Wide character as a string</returns>
        public override readonly string ToString() =>
            $"{low}{high}";

        /// <inheritdoc/>
        public override readonly bool Equals(object obj)
        {
            if (obj is WideChar WideChar)
                return Equals(WideChar);
            return base.Equals(obj);
        }

        /// <summary>
        /// Checks to see if this instance of the <see cref="WideChar"/> instance is equal to another instance of the <see cref="WideChar"/> instance
        /// </summary>
        /// <param name="other">Another instance of the <see cref="WideChar"/> instance to compare with this <see cref="WideChar"/> instance</param>
        /// <returns>True if both the <see cref="WideChar"/> instances match; otherwise, false.</returns>
        public readonly bool Equals(WideChar other)
            => Equals(this, other);

        /// <summary>
        /// Checks to see if the first instance of the <see cref="WideChar"/> instance is equal to another instance of the <see cref="WideChar"/> instance
        /// </summary>
        /// <param name="other">Another instance of the <see cref="WideChar"/> instance to compare with another</param>
        /// <param name="other2">Another instance of the <see cref="WideChar"/> instance to compare with another</param>
        /// <returns>True if both the <see cref="WideChar"/> instances match; otherwise, false.</returns>
        public readonly bool Equals(WideChar other, WideChar other2)
        {
            // Check all the properties
            return
                other.high == other2.high &&
                other.low == other2.low
            ;
        }

        /// <inheritdoc/>
        public static bool operator ==(WideChar a, WideChar b)
            => a.Equals(b);

        /// <inheritdoc/>
        public static bool operator !=(WideChar a, WideChar b)
            => !a.Equals(b);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -1466551034;
            hashCode = hashCode * -1521134295 + high.GetHashCode();
            hashCode = hashCode * -1521134295 + low.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Explicit operator for <see cref="WideChar"/>
        /// </summary>
        /// <param name="source">Source string that contains either one (low) or two (low/high) characters</param>
        public static explicit operator WideChar(string source) =>
            new(source);

        /// <summary>
        /// Explicit operator for <see cref="WideChar"/>
        /// </summary>
        /// <param name="charCode">Character code</param>
        public static explicit operator WideChar(int charCode) =>
            new(charCode);

        /// <summary>
        /// Implicit operator for <see cref="string"/>
        /// </summary>
        /// <param name="source">Source wide character</param>
        public static implicit operator string(WideChar source) =>
            source.ToString();

        /// <summary>
        /// Implicit operator for hi/lo values
        /// </summary>
        /// <param name="source">Source wide character</param>
        public static implicit operator (char high, char low)(WideChar source) =>
            (source.high, source.low);

        /// <summary>
        /// Implicit operator for char code
        /// </summary>
        /// <param name="source">Source wide character</param>
        public static implicit operator int(WideChar source) =>
            source.GetCharCode();

        private int GetCharCode() =>
            high << 8 | low;

        /// <summary>
        /// Makes a new wide character instance
        /// </summary>
        /// <param name="source">Source string that contains either one (low) or two (low/high) characters</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public WideChar(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("String is not specified");
            if (source.Length > 2)
                throw new ArgumentException("Source string may not contain greater than two characters");

            // Get hi/lo values and install them
            high = source.Length == 2 ? source[1] : '\0';
            low = source[0];
        }

        /// <summary>
        /// Makes a new wide character instance
        /// </summary>
        /// <param name="charCode">Character code that represents a wide character</param>
        public WideChar(int charCode)
        {
            high = (char)(charCode >> 1);
            low = (char)(charCode >> 8);
        }

        /// <summary>
        /// Makes a new wide character instance
        /// </summary>
        /// <param name="hi">Second byte of a wide character</param>
        /// <param name="lo">First byte of a wide character</param>
        public WideChar(char hi, char lo)
        {
            high = hi;
            low = lo;
        }
    }
}
