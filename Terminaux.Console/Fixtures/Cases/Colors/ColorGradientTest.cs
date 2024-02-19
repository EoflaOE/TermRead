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

using System.Threading;
using Terminaux.Base;
using Terminaux.Colors;
using Terminaux.Colors.Gradients;

namespace Terminaux.Console.Fixtures.Cases.Colors
{
    internal class ColorGradientTest : IFixture
    {
        public string FixtureID => "ColorGradientTest";
        public void RunFixture()
        {
            var source = ColorTools.GetRandomColor();
            var target = ColorTools.GetRandomColor();
            var grads = ColorGradients.GetGradients(source, target, 100);
            ConsoleWrapper.CursorVisible = false;
            foreach (var grad in grads)
            {
                var color = grad.IntermediateColor;
                ColorTools.LoadBackDry(color, true);
                Thread.Sleep(50);
            }
            ColorTools.LoadBack();
            ConsoleWrapper.CursorVisible = true;
        }
    }
}
