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
using Terminaux.Base;
using Terminaux.Base.Buffered;
using Terminaux.Colors;
using Terminaux.Inputs.Styles.Infobox;
using Terminaux.Inputs;
using Terminaux.Writer.CyclicWriters;
using System.Threading;
using Terminaux.Writer.CyclicWriters.Renderer;

namespace Terminaux.Console.Fixtures.Cases.Writer
{
    internal class TestProgressBars : IFixture
    {
        public FixtureCategory Category => FixtureCategory.Writer;

        public void RunFixture()
        {
            // Show a screen with a progress bar at the end
            var stickScreen = new Screen()
            {
                CycleFrequency = 50,
            };

            // Make a collection of renderables
            var container = new Container();
            var progressBar1 = new ProgressBar(
                "This is the test progress bar that contains a scrolling marquee.", 0, 100)
            {
                LeftMargin = 4,
                RightMargin = 4,
            };
            var progressBar2 = new ProgressBarNoText(0, 100)
            {
                LeftMargin = 4,
                RightMargin = 4,
            };
            var progressBar3 = new ProgressBar(
                "這是包含滾動選取框的測試進度條。", 0, 100)
            {
                LeftMargin = 4,
                RightMargin = 4,
            };
            var progressBar4 = new ProgressBar(
                "This is the test progress bar that contains a scrolling marquee.", 0, 100)
            {
                LeftMargin = 4,
                RightMargin = 4,
                Indeterminate = true,
            };
            var progressBar5 = new ProgressBarNoText(0, 100)
            {
                LeftMargin = 4,
                RightMargin = 4,
                Indeterminate = true,
            };
            var progressBar6 = new ProgressBar(
                "這是包含滾動選取框的測試進度條。", 0, 100)
            {
                LeftMargin = 4,
                RightMargin = 4,
                Indeterminate = true,
            };
            container.AddRenderable("Progress bar 1", progressBar1);
            container.SetRenderablePosition("Progress bar 1", new(4, ConsoleWrapper.WindowHeight - 6));
            container.AddRenderable("Progress bar 2", progressBar2);
            container.SetRenderablePosition("Progress bar 2", new(4, ConsoleWrapper.WindowHeight - 5));
            container.AddRenderable("Progress bar 3", progressBar3);
            container.SetRenderablePosition("Progress bar 3", new(4, ConsoleWrapper.WindowHeight - 4));
            container.AddRenderable("Progress bar 4", progressBar4);
            container.SetRenderablePosition("Progress bar 4", new(4, ConsoleWrapper.WindowHeight - 3));
            container.AddRenderable("Progress bar 5", progressBar5);
            container.SetRenderablePosition("Progress bar 5", new(4, ConsoleWrapper.WindowHeight - 2));
            container.AddRenderable("Progress bar 6", progressBar6);
            container.SetRenderablePosition("Progress bar 6", new(4, ConsoleWrapper.WindowHeight - 1));

            // Render them all
            try
            {
                // First, clear the screen
                ColorTools.LoadBack();

                // Then, show the progress bar
                var stickScreenPart = new ScreenPart();
                stickScreenPart.Position(4, ConsoleWrapper.WindowHeight - 1);
                stickScreenPart.AddDynamicText(() => ContainerTools.RenderContainer(container));
                stickScreen.AddBufferedPart("Test", stickScreenPart);
                ScreenTools.SetCurrent(stickScreen);
                ScreenTools.SetCurrentCyclic(stickScreen);
                ScreenTools.StartCyclicScreen();

                // Finally, increment the progress bar until it's full
                for (int progress = 0; progress < 100; progress++)
                {
                    progressBar1.Position = progress;
                    progressBar2.Position = progress;
                    progressBar3.Position = progress;
                    progressBar4.Position = progress;
                    progressBar5.Position = progress;
                    progressBar6.Position = progress;
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                InfoBoxModalColor.WriteInfoBoxModal($"Screen failed to render: {ex.Message}");
            }
            finally
            {
                ScreenTools.StopCyclicScreen();
                ScreenTools.UnsetCurrent(stickScreen);
                ColorTools.LoadBack();
            }
        }
    }
}
