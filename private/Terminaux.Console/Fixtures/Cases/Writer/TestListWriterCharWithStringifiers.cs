﻿//
// Terminaux  Copyright (C) 2023-2025  Aptivi
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

using System.Collections.Generic;
using Terminaux.Colors.Data;
using Terminaux.Writer.ConsoleWriters;

namespace Terminaux.Console.Fixtures.Cases.Writer
{
    internal class TestListWriterCharWithStringifiers : IFixture
    {
        public void RunFixture()
        {
            var NormalCharList = new List<char>() { '1', '2', '3' };
            var ArrayCharList = new List<char[]>() { { new char[] { '1', '2', '3' } }, { new char[] { '1', '2', '3' } }, { new char[] { '1', '2', '3' } } };
            TextWriterColor.Write("Normal char list:");
            ListWriterColor.WriteList(NormalCharList, ConsoleColors.Silver, ConsoleColors.Grey, false, (character) => $"{character} [{(int)character}]");
            TextWriterColor.Write("Array char list:");
            ListWriterColor.WriteList(ArrayCharList, ConsoleColors.Silver, ConsoleColors.Grey, false, stringifier: null, (character) => $"{(char)character} [{(int)(char)character}]");
        }
    }
}
