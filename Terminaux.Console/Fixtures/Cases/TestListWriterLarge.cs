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

using System.Collections.Generic;
using Terminaux.Colors.Data;
using Terminaux.Writer.ConsoleWriters;

namespace Terminaux.ConsoleDemo.Fixtures.Cases
{
    internal class TestListWriterLarge : IFixture
    {
        public string FixtureID => "TestListWriterLarge";
        public void RunFixture()
        {
            var choices = new List<string>();
            var arrayChoices = new List<string[]>();
            for (int i = 0; i < 1000; i++)
                choices.Add($"Number #{i + 1}");
            for (int i = 0; i < 1000; i++)
                arrayChoices.Add([$"Number #{i + 1}", $"Index #{i}"]);
            TextWriterColor.Write("Normal string list:");
            ListWriterColor.WriteList(choices, ConsoleColors.Gray, ConsoleColors.DarkGray, false);
            TextWriterColor.Write("Array string list:");
            ListWriterColor.WriteList(arrayChoices, ConsoleColors.Gray, ConsoleColors.DarkGray, false);
        }
    }
}
