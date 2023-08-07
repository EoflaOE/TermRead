﻿
// Terminaux  Copyright (C) 2023  Aptivi
// 
// This file is part of Terminaux
// 
// Terminaux is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Terminaux is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using Terminaux.Colors;
using Terminaux.ConsoleDemo.Fixtures;
using Terminaux.Writer.ConsoleWriters;

namespace KS.Kernel.Debugging.Testing.Facades
{
    internal class PrintWithNewLines : IFixture
    {
        public string FixtureID => "PrintWithNewLines";
        public void RunFixture()
        {
            TextWriterColor.Write("Hello world!\nHow's your day going?\nShould be directly after this:", false, new Color(ConsoleColors.Green));
            TextWriterColor.Write(" [{0}, {1}] ", true, new Color(ConsoleColors.Gray), vars: new object[] { Console.CursorLeft, Console.CursorTop });
        }
    }
}
