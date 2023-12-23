//
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
using Terminaux.Colors;
using Terminaux.Colors.Models.Parsing;

namespace Terminaux.Tests.Colors
{
    [TestFixture]
    public partial class ColorInitializationValidityTests
    {
        /// <summary>
        /// Tests trying to parse the color from hex
        /// </summary>
        [TestCase("#0F0F0F", ExpectedResult = true)]
        [TestCase("#0G0G0G", ExpectedResult = false)]
        [Description("Validity")]
        public bool TestTryParseColorFromHex(string TargetHex)
        {
            try
            {
                Debug.WriteLine($"Trying {TargetHex}...");
                var col = new Color(TargetHex);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Tests trying to parse the color from color numbers
        /// </summary>
        [TestCase(26, ExpectedResult = true)]
        [TestCase(260, ExpectedResult = false)]
        [TestCase(-26, ExpectedResult = false)]
        [Description("Validity")]
        public bool TestTryParseColorFromColorNum(int TargetColorNum)
        {
            try
            {
                Debug.WriteLine($"Trying colornum {TargetColorNum}...");
                var col = new Color(TargetColorNum);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Tests trying to parse the color from RGB
        /// </summary>
        [TestCase(4, 4, 4, ExpectedResult = true)]
        [TestCase(400, 4, 4, ExpectedResult = false)]
        [TestCase(4, 400, 4, ExpectedResult = false)]
        [TestCase(4, 4, 400, ExpectedResult = false)]
        [TestCase(4, 400, 400, ExpectedResult = false)]
        [TestCase(400, 4, 400, ExpectedResult = false)]
        [TestCase(400, 400, 4, ExpectedResult = false)]
        [TestCase(400, 400, 400, ExpectedResult = false)]
        [TestCase(-4, 4, 4, ExpectedResult = false)]
        [TestCase(4, -4, 4, ExpectedResult = false)]
        [TestCase(4, 4, -4, ExpectedResult = false)]
        [TestCase(4, -4, -4, ExpectedResult = false)]
        [TestCase(-4, 4, -4, ExpectedResult = false)]
        [TestCase(-4, -4, 4, ExpectedResult = false)]
        [TestCase(-4, -4, -4, ExpectedResult = false)]
        [Description("Validity")]
        public bool TestTryParseColorFromRGB(int R, int G, int B)
        {
            try
            {
                Debug.WriteLine($"Trying rgb {R}, {G}, {B}...");
                var col = new Color(R, G, B);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Tests trying to parse the color from RGB color specifier
        /// </summary>
        [TestCase("4;4;4", ExpectedResult = true)]
        [TestCase("400;4;4", ExpectedResult = false)]
        [TestCase("4;400;4", ExpectedResult = false)]
        [TestCase("4;4;400", ExpectedResult = false)]
        [TestCase("4;400;400", ExpectedResult = false)]
        [TestCase("400;4;400", ExpectedResult = false)]
        [TestCase("400;400;4", ExpectedResult = false)]
        [TestCase("400;400;400", ExpectedResult = false)]
        [TestCase("-4;4;4", ExpectedResult = false)]
        [TestCase("4;-4;4", ExpectedResult = false)]
        [TestCase("4;4;-4", ExpectedResult = false)]
        [TestCase("4;-4;-4", ExpectedResult = false)]
        [TestCase("-4;4;-4", ExpectedResult = false)]
        [TestCase("-4;-4;4", ExpectedResult = false)]
        [TestCase("-4;-4;-4", ExpectedResult = false)]
        [Description("Validity")]
        public bool TestTryParseColorFromSpecifier(string specifier)
        {
            try
            {
                Debug.WriteLine($"Trying rgb specifier {specifier}...");
                var col = new Color(specifier);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Tests trying to check the color specifier syntax validity from hex
        /// </summary>
        [TestCase("#0F0F0F", ExpectedResult = true)]
        [TestCase("#0G0G0G", ExpectedResult = true)]
        [TestCase("#FFF", ExpectedResult = true)]
        [TestCase("#GGG", ExpectedResult = true)]
        [Description("Validity")]
        public bool TestIsSpecifierValidFromHex(string TargetHex) =>
            ParsingTools.IsSpecifierValidRgbHash(TargetHex);

        /// <summary>
        /// Tests trying to check the color specifier syntax validity from color numbers
        /// </summary>
        [TestCase(26, ExpectedResult = true)]
        [TestCase(260, ExpectedResult = false)]
        [TestCase(-26, ExpectedResult = false)]
        [Description("Validity")]
        public bool TestIsSpecifierValidFromColorNum(int TargetColorNum) =>
            ParsingTools.IsSpecifierConsoleColors($"{TargetColorNum}");

        /// <summary>
        /// Tests trying to check the color specifier syntax validity from RGB
        /// </summary>
        [TestCase(4, 4, 4, ExpectedResult = true)]
        [TestCase(400, 4, 4, ExpectedResult = true)]
        [TestCase(4, 400, 4, ExpectedResult = true)]
        [TestCase(4, 4, 400, ExpectedResult = true)]
        [TestCase(4, 400, 400, ExpectedResult = true)]
        [TestCase(400, 4, 400, ExpectedResult = true)]
        [TestCase(400, 400, 4, ExpectedResult = true)]
        [TestCase(400, 400, 400, ExpectedResult = true)]
        [TestCase(-4, 4, 4, ExpectedResult = true)]
        [TestCase(4, -4, 4, ExpectedResult = true)]
        [TestCase(4, 4, -4, ExpectedResult = true)]
        [TestCase(4, -4, -4, ExpectedResult = true)]
        [TestCase(-4, 4, -4, ExpectedResult = true)]
        [TestCase(-4, -4, 4, ExpectedResult = true)]
        [TestCase(-4, -4, -4, ExpectedResult = true)]
        [Description("Validity")]
        public bool TestIsSpecifierValidFromRGB(int R, int G, int B) =>
            ParsingTools.IsSpecifierValid($"{R};{G};{B}");

        /// <summary>
        /// Tests trying to check the color specifier syntax validity from RGB color specifier
        /// </summary>
        [TestCase("4;4;4", ExpectedResult = true)]
        [TestCase("400;4;4", ExpectedResult = true)]
        [TestCase("4;400;4", ExpectedResult = true)]
        [TestCase("4;4;400", ExpectedResult = true)]
        [TestCase("4;400;400", ExpectedResult = true)]
        [TestCase("400;4;400", ExpectedResult = true)]
        [TestCase("400;400;4", ExpectedResult = true)]
        [TestCase("400;400;400", ExpectedResult = true)]
        [TestCase("-4;4;4", ExpectedResult = true)]
        [TestCase("4;-4;4", ExpectedResult = true)]
        [TestCase("4;4;-4", ExpectedResult = true)]
        [TestCase("4;-4;-4", ExpectedResult = true)]
        [TestCase("-4;4;-4", ExpectedResult = true)]
        [TestCase("-4;-4;4", ExpectedResult = true)]
        [TestCase("-4;-4;-4", ExpectedResult = true)]
        [Description("Validity")]
        public bool TestIsSpecifierValidFromSpecifier(string specifier) =>
            ParsingTools.IsSpecifierValid(specifier);

        /// <summary>
        /// Tests trying to parse the color from hex
        /// </summary>
        [TestCase("#0F0F0F", ExpectedResult = true)]
        [TestCase("#0G0G0G", ExpectedResult = false)]
        [TestCase("#FFF", ExpectedResult = true)]
        [TestCase("#GGG", ExpectedResult = false)]
        [Description("Validity")]
        public bool TestIsSpecifierAndValueValidFromHex(string TargetHex) =>
            ParsingTools.IsSpecifierAndValueValid(TargetHex);

        /// <summary>
        /// Tests trying to parse the color from color numbers
        /// </summary>
        [TestCase(26, ExpectedResult = true)]
        [TestCase(260, ExpectedResult = false)]
        [TestCase(-26, ExpectedResult = false)]
        [Description("Validity")]
        public bool TestIsSpecifierAndValueValidFromColorNum(int TargetColorNum) =>
            ParsingTools.IsSpecifierAndValueValid($"{TargetColorNum}");

        /// <summary>
        /// Tests trying to parse the color from RGB
        /// </summary>
        [TestCase(4, 4, 4, ExpectedResult = true)]
        [TestCase(400, 4, 4, ExpectedResult = false)]
        [TestCase(4, 400, 4, ExpectedResult = false)]
        [TestCase(4, 4, 400, ExpectedResult = false)]
        [TestCase(4, 400, 400, ExpectedResult = false)]
        [TestCase(400, 4, 400, ExpectedResult = false)]
        [TestCase(400, 400, 4, ExpectedResult = false)]
        [TestCase(400, 400, 400, ExpectedResult = false)]
        [TestCase(-4, 4, 4, ExpectedResult = false)]
        [TestCase(4, -4, 4, ExpectedResult = false)]
        [TestCase(4, 4, -4, ExpectedResult = false)]
        [TestCase(4, -4, -4, ExpectedResult = false)]
        [TestCase(-4, 4, -4, ExpectedResult = false)]
        [TestCase(-4, -4, 4, ExpectedResult = false)]
        [TestCase(-4, -4, -4, ExpectedResult = false)]
        [Description("Validity")]
        public bool TestIsSpecifierAndValueValidFromRGB(int R, int G, int B) =>
            ParsingTools.IsSpecifierAndValueValid($"{R};{G};{B}");

        /// <summary>
        /// Tests trying to parse the color from RGB color specifier
        /// </summary>
        [TestCase("4;4;4", ExpectedResult = true)]
        [TestCase("400;4;4", ExpectedResult = false)]
        [TestCase("4;400;4", ExpectedResult = false)]
        [TestCase("4;4;400", ExpectedResult = false)]
        [TestCase("4;400;400", ExpectedResult = false)]
        [TestCase("400;4;400", ExpectedResult = false)]
        [TestCase("400;400;4", ExpectedResult = false)]
        [TestCase("400;400;400", ExpectedResult = false)]
        [TestCase("-4;4;4", ExpectedResult = false)]
        [TestCase("4;-4;4", ExpectedResult = false)]
        [TestCase("4;4;-4", ExpectedResult = false)]
        [TestCase("4;-4;-4", ExpectedResult = false)]
        [TestCase("-4;4;-4", ExpectedResult = false)]
        [TestCase("-4;-4;4", ExpectedResult = false)]
        [TestCase("-4;-4;-4", ExpectedResult = false)]
        [Description("Validity")]
        public bool TestIsSpecifierAndValueValidFromSpecifier(string specifier) =>
            ParsingTools.IsSpecifierAndValueValid(specifier);

        /// <summary>
        /// Tests trying to parse the color from hex
        /// </summary>
        [TestCase("#0F0F0F", ExpectedResult = true)]
        [TestCase("#0G0G0G", ExpectedResult = false)]
        [TestCase("#FFF", ExpectedResult = true)]
        [TestCase("#GGG", ExpectedResult = false)]
        [Description("Validity")]
        public bool TestParseSpecifierFromHex(string TargetHex)
        {
            try
            {
                ParsingTools.ParseSpecifier(TargetHex);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tests trying to parse the color from color numbers
        /// </summary>
        [TestCase(26, ExpectedResult = true)]
        [TestCase(260, ExpectedResult = false)]
        [TestCase(-26, ExpectedResult = false)]
        [Description("Validity")]
        public bool TestParseSpecifierFromColorNum(int TargetColorNum)
        {
            try
            {
                ParsingTools.ParseSpecifier($"{TargetColorNum}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tests trying to parse the color from RGB
        /// </summary>
        [TestCase(4, 4, 4, ExpectedResult = true)]
        [TestCase(400, 4, 4, ExpectedResult = false)]
        [TestCase(4, 400, 4, ExpectedResult = false)]
        [TestCase(4, 4, 400, ExpectedResult = false)]
        [TestCase(4, 400, 400, ExpectedResult = false)]
        [TestCase(400, 4, 400, ExpectedResult = false)]
        [TestCase(400, 400, 4, ExpectedResult = false)]
        [TestCase(400, 400, 400, ExpectedResult = false)]
        [TestCase(-4, 4, 4, ExpectedResult = false)]
        [TestCase(4, -4, 4, ExpectedResult = false)]
        [TestCase(4, 4, -4, ExpectedResult = false)]
        [TestCase(4, -4, -4, ExpectedResult = false)]
        [TestCase(-4, 4, -4, ExpectedResult = false)]
        [TestCase(-4, -4, 4, ExpectedResult = false)]
        [TestCase(-4, -4, -4, ExpectedResult = false)]
        [Description("Validity")]
        public bool TestParseSpecifierFromRGB(int R, int G, int B)
        {
            try
            {
                ParsingTools.ParseSpecifier($"{R};{G};{B}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tests trying to parse the color from RGB color specifier
        /// </summary>
        [TestCase("4;4;4", ExpectedResult = true)]
        [TestCase("400;4;4", ExpectedResult = false)]
        [TestCase("4;400;4", ExpectedResult = false)]
        [TestCase("4;4;400", ExpectedResult = false)]
        [TestCase("4;400;400", ExpectedResult = false)]
        [TestCase("400;4;400", ExpectedResult = false)]
        [TestCase("400;400;4", ExpectedResult = false)]
        [TestCase("400;400;400", ExpectedResult = false)]
        [TestCase("-4;4;4", ExpectedResult = false)]
        [TestCase("4;-4;4", ExpectedResult = false)]
        [TestCase("4;4;-4", ExpectedResult = false)]
        [TestCase("4;-4;-4", ExpectedResult = false)]
        [TestCase("-4;4;-4", ExpectedResult = false)]
        [TestCase("-4;-4;4", ExpectedResult = false)]
        [TestCase("-4;-4;-4", ExpectedResult = false)]
        [Description("Validity")]
        public bool TestParseSpecifierFromSpecifier(string specifier)
        {
            try
            {
                ParsingTools.ParseSpecifier(specifier);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tests trying to parse the color from hex
        /// </summary>
        [TestCase("#0F0F0F", ExpectedResult = true)]
        [TestCase("#0G0G0G", ExpectedResult = false)]
        [TestCase("#FFF", ExpectedResult = true)]
        [TestCase("#GGG", ExpectedResult = false)]
        [Description("Validity")]
        public bool TestTryParseSpecifierFromHex(string TargetHex) =>
            ParsingTools.TryParseSpecifier(TargetHex, out _);

        /// <summary>
        /// Tests trying to parse the color from color numbers
        /// </summary>
        [TestCase(26, ExpectedResult = true)]
        [TestCase(260, ExpectedResult = false)]
        [TestCase(-26, ExpectedResult = false)]
        [Description("Validity")]
        public bool TestTryParseSpecifierFromColorNum(int TargetColorNum) =>
            ParsingTools.TryParseSpecifier($"{TargetColorNum}", out _);

        /// <summary>
        /// Tests trying to parse the color from RGB
        /// </summary>
        [TestCase(4, 4, 4, ExpectedResult = true)]
        [TestCase(400, 4, 4, ExpectedResult = false)]
        [TestCase(4, 400, 4, ExpectedResult = false)]
        [TestCase(4, 4, 400, ExpectedResult = false)]
        [TestCase(4, 400, 400, ExpectedResult = false)]
        [TestCase(400, 4, 400, ExpectedResult = false)]
        [TestCase(400, 400, 4, ExpectedResult = false)]
        [TestCase(400, 400, 400, ExpectedResult = false)]
        [TestCase(-4, 4, 4, ExpectedResult = false)]
        [TestCase(4, -4, 4, ExpectedResult = false)]
        [TestCase(4, 4, -4, ExpectedResult = false)]
        [TestCase(4, -4, -4, ExpectedResult = false)]
        [TestCase(-4, 4, -4, ExpectedResult = false)]
        [TestCase(-4, -4, 4, ExpectedResult = false)]
        [TestCase(-4, -4, -4, ExpectedResult = false)]
        [Description("Validity")]
        public bool TestTryParseSpecifierFromRGB(int R, int G, int B) =>
            ParsingTools.TryParseSpecifier($"{R};{G};{B}", out _);

        /// <summary>
        /// Tests trying to parse the color from RGB color specifier
        /// </summary>
        [TestCase("4;4;4", ExpectedResult = true)]
        [TestCase("400;4;4", ExpectedResult = false)]
        [TestCase("4;400;4", ExpectedResult = false)]
        [TestCase("4;4;400", ExpectedResult = false)]
        [TestCase("4;400;400", ExpectedResult = false)]
        [TestCase("400;4;400", ExpectedResult = false)]
        [TestCase("400;400;4", ExpectedResult = false)]
        [TestCase("400;400;400", ExpectedResult = false)]
        [TestCase("-4;4;4", ExpectedResult = false)]
        [TestCase("4;-4;4", ExpectedResult = false)]
        [TestCase("4;4;-4", ExpectedResult = false)]
        [TestCase("4;-4;-4", ExpectedResult = false)]
        [TestCase("-4;4;-4", ExpectedResult = false)]
        [TestCase("-4;-4;4", ExpectedResult = false)]
        [TestCase("-4;-4;-4", ExpectedResult = false)]
        [Description("Validity")]
        public bool TestTryParseSpecifierFromSpecifier(string specifier) =>
            ParsingTools.TryParseSpecifier(specifier, out _);
    }
}
