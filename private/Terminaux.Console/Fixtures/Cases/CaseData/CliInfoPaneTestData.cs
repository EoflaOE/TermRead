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

using System;
using System.Collections.Generic;
using Terminaux.Inputs.Interactive;
using Terminaux.Inputs.Pointer;

namespace Terminaux.Console.Fixtures.Cases.CaseData
{
    internal class CliInfoPaneTestData : BaseInteractiveTui<string>, IInteractiveTui<string>
    {
        internal static List<string> strings = [];

        public override InteractiveTuiBinding[] Bindings { get; } =
        [
            new InteractiveTuiBinding("Add",         ConsoleKey.F1, (_, index) => Add(index), true),
            new InteractiveTuiBinding("Delete",      ConsoleKey.F2, (_, index) => Remove(index)),
            new InteractiveTuiBinding("Delete",      PointerButton.Right, PointerButtonPress.Released, (_, index) => Remove(index)),
            new InteractiveTuiBinding("Delete Last", ConsoleKey.F3, (_, _)     => RemoveLast()),
        ];

        /// <inheritdoc/>
        public override IEnumerable<string> PrimaryDataSource =>
            strings;

        /// <inheritdoc/>
        public override bool AcceptsEmptyData =>
            true;

        /// <inheritdoc/>
        public override string GetInfoFromItem(string item)
        {
            string selected = item;

            // Check to see if we're given the test info
            if (string.IsNullOrEmpty(selected))
                InteractiveTuiStatus.Status = "No info.";
            else
                InteractiveTuiStatus.Status = $"{selected}";

            // Now, populate the info to the status
            return $" {InteractiveTuiStatus.Status}";
        }

        /// <inheritdoc/>
        public override string GetEntryFromItem(string item)
        {
            string selected = item;
            return selected;
        }

        private static void Add(int index)
        {
            strings.Add($"[{index}] --+-- [{index}]");
        }

        private static void Remove(int index)
        {
            if (strings.Count > 0)
                strings.RemoveAt(index);
        }

        private static void RemoveLast()
        {
            if (strings.Count > 0)
                strings.RemoveAt(strings.Count - 1);
        }
    }
}
