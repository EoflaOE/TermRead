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
using System.Text;
using Terminaux.Writer.FancyWriters;
using Terminaux.Writer.ConsoleWriters;
using Terminaux.Base.Buffered;
using Terminaux.Colors;
using Terminaux.Base;
using Terminaux.Colors.Data;
using Terminaux.Sequences.Builder.Types;
using Terminaux.Reader;
using Terminaux.Base.Checks;
using Terminaux.Base.Extensions;
using System.Threading;
using Terminaux.Inputs.Pointer;

namespace Terminaux.Inputs.Presentation
{
    /// <summary>
    /// Presentation tools
    /// </summary>
    public static class PresentationTools
    {
        /// <summary>
        /// The upper left corner of the exterior border (the left position)
        /// </summary>
        [Obsolete("These were initially reserved for internal use. Also, the presentation system will be revamped in the next few releases.")]
        public static int PresentationUpperBorderLeft =>
            2;
        /// <summary>
        /// The upper left corner of the exterior border (the top position)
        /// </summary>
        [Obsolete("These were initially reserved for internal use. Also, the presentation system will be revamped in the next few releases.")]
        public static int PresentationUpperBorderTop =>
            1;
        /// <summary>
        /// The upper left corner of the inner border (the left position)
        /// </summary>
        [Obsolete("These were initially reserved for internal use. Also, the presentation system will be revamped in the next few releases.")]
        public static int PresentationUpperInnerBorderLeft =>
            PresentationUpperBorderLeft + 1;
        /// <summary>
        /// The upper left corner of the inner border (the top position)
        /// </summary>
        [Obsolete("These were initially reserved for internal use. Also, the presentation system will be revamped in the next few releases.")]
        public static int PresentationUpperInnerBorderTop =>
            PresentationUpperBorderTop + 1;
        /// <summary>
        /// The lower right corner of the inner border (the left position)
        /// </summary>
        [Obsolete("These were initially reserved for internal use. Also, the presentation system will be revamped in the next few releases.")]
        public static int PresentationLowerInnerBorderLeft =>
            ConsoleWrapper.WindowWidth - PresentationUpperInnerBorderLeft * 2;
        /// <summary>
        /// The lower right corner of the inner border (the top position)
        /// </summary>
        [Obsolete("These were initially reserved for internal use. Also, the presentation system will be revamped in the next few releases.")]
        public static int PresentationLowerInnerBorderTop =>
            ConsoleWrapper.WindowHeight - PresentationUpperBorderTop * 2 - 4;
        /// <summary>
        /// The informational top position
        /// </summary>
        [Obsolete("These were initially reserved for internal use. Also, the presentation system will be revamped in the next few releases.")]
        public static int PresentationInformationalTop =>
            ConsoleWrapper.WindowHeight - 2;

        /// <summary>
        /// Present the presentation
        /// </summary>
        /// <param name="presentation">Presentation instance</param>
        public static void Present(Slideshow presentation) =>
            Present(presentation, false, false);

        /// <summary>
        /// Present the presentation
        /// </summary>
        /// <param name="presentation">Presentation instance</param>
        /// <param name="kiosk">Prevent any key other than ENTER from being pressed</param>
        /// <param name="required">Prevents exiting the presentation</param>
        public static void Present(Slideshow presentation, bool kiosk, bool required)
        {
            // Make a screen instance for the presentation
            var screen = new Screen();
            var buffer = new ScreenPart();
            ScreenTools.SetCurrent(screen);
            screen.AddBufferedPart("Presentation view", buffer);

            // Loop for each page
            var pages = presentation.Pages;
            bool presentExit = false;
            for (int i = 0; i < pages.Count; i++)
            {
                // Check to see if we're exiting
                if (presentExit)
                    break;

                // Get the page
                var page = pages[i];

                // Fill the buffer
                buffer.AddDynamicText(() =>
                {
                    var builder = new StringBuilder();

                    // Populate some variables
                    int presentationUpperBorderLeft = 2;
                    int presentationUpperBorderTop = 1;
                    int presentationUpperInnerBorderLeft = presentationUpperBorderLeft + 1;
                    int presentationUpperInnerBorderTop = presentationUpperBorderTop + 1;
                    int presentationLowerInnerBorderLeft = ConsoleWrapper.WindowWidth - presentationUpperInnerBorderLeft * 2;
                    int presentationLowerInnerBorderTop = ConsoleWrapper.WindowHeight - presentationUpperBorderTop * 2 - 4;
                    int presentationInformationalTop = ConsoleWrapper.WindowHeight - 2;

                    // Make a border
                    builder.Append(
                        BoxFrameColor.RenderBoxFrame($"{(!kiosk ? $"[{i + 1}/{pages.Count}] - " : "")}{page.Name} - {presentation.Name}", presentationUpperBorderLeft, presentationUpperBorderTop, presentationLowerInnerBorderLeft, presentationLowerInnerBorderTop, new Color(ConsoleColors.Silver), ColorTools.CurrentBackgroundColor) +
                        BoxColor.RenderBox(presentationUpperInnerBorderLeft, presentationUpperBorderTop, presentationLowerInnerBorderLeft, presentationLowerInnerBorderTop)
                    );

                    // Write the bindings
                    builder.Append(
                        CenteredTextColor.RenderCentered(presentationInformationalTop, $"[ENTER] {"Advance"}{(!kiosk && !required ? $" - [ESC] {"Exit"}" : "")}".Truncate(presentationLowerInnerBorderLeft + 1), new Color(ConsoleColors.White), ColorTools.CurrentBackgroundColor)
                    );

                    // Clear the presentation screen
                    builder.Append(
                        ClearPresentation()
                    );

                    // Generate the final string
                    return builder.ToString();
                });

                // We need to dynamically render all the elements, so screen ends here.
                ScreenTools.Render();

                // Render all elements
                var pageElements = page.Elements;
                bool checkOutOfBounds = false;
                foreach (var element in pageElements)
                {
                    // Check for possible out-of-bounds
                    if (element.IsPossibleOutOfBounds() && checkOutOfBounds)
                    {
                        TermReader.ReadPointerOrKey();
                        TextWriterRaw.WriteRaw(ClearPresentation());
                    }
                    checkOutOfBounds = true;

                    // Render it to the view
                    element.Render();

                    // Check to see if we need to invoke action
                    if (element.IsInput)
                        if (element.InvokeActionInput is not null)
                            element.InvokeActionInput([element.WrittenInput ?? ""]);
                        else
                        if (element.InvokeAction is not null)
                            element.InvokeAction();
                }

                // Wait for the ENTER key to be pressed if in kiosk mode. If not in kiosk mode, handle any key
                bool pageExit = false;
                while (!pageExit)
                {
                    // Get the keypress or mouse press
                    SpinWait.SpinUntil(() => PointerListener.InputAvailable);
                    if (PointerListener.PointerAvailable)
                    {
                        // Mouse input received.
                        var mouse = TermReader.ReadPointer();
                        switch (mouse.Button)
                        {
                            case PointerButton.Left:
                                if (mouse.ButtonPress != PointerButtonPress.Released)
                                    break;
                                pageExit = true;
                                break;
                        }
                    }
                    else if (ConsoleWrapper.KeyAvailable && !PointerListener.PointerActive)
                    {
                        var key = TermReader.ReadKey();
                        switch (key.Key)
                        {
                            case ConsoleKey.Escape:
                                if (required)
                                    break;
                                if (kiosk)
                                    break;
                                presentExit = true;
                                pageExit = true;
                                break;
                            case ConsoleKey.Enter:
                                pageExit = true;
                                break;
                        }
                    }
                }
            }

            // Clean up after ourselves
            ScreenTools.UnsetCurrent(screen);
            ColorTools.LoadBack();
            ConsoleWrapper.CursorVisible = true;
        }

        /// <summary>
        /// Checks to see if the presentation contains input
        /// </summary>
        /// <param name="presentation">Target presentation</param>
        /// <returns>True if one of the elements in a page contains input</returns>
        public static bool PresentationContainsInput(Slideshow presentation)
        {
            // Check every page
            foreach (var page in presentation.Pages)
                foreach (var element in page.Elements)
                    if (element.IsInput)
                        // One of the elements contains input
                        return true;

            // If not input, then false
            return false;
        }

        /// <summary>
        /// Clears the presentation
        /// </summary>
        public static string ClearPresentation()
        {
            var builder = new StringBuilder();

            // Populate some variables
            int presentationUpperBorderLeft = 2;
            int presentationUpperBorderTop = 1;
            int presentationUpperInnerBorderLeft = presentationUpperBorderLeft + 1;
            int presentationUpperInnerBorderTop = presentationUpperBorderTop + 1;
            int presentationLowerInnerBorderLeft = ConsoleWrapper.WindowWidth - presentationUpperInnerBorderLeft * 2;
            int presentationLowerInnerBorderTop = ConsoleWrapper.WindowHeight - presentationUpperBorderTop * 2 - 4;

            // Clear the presentation screen
            for (int y = presentationUpperInnerBorderTop; y <= presentationLowerInnerBorderTop + 1; y++)
                builder.Append(TextWriterWhereColor.RenderWhere(new string(' ', presentationLowerInnerBorderLeft), presentationUpperInnerBorderLeft, y));

            // Seek to the first position inside the border
            builder.Append(CsiSequences.GenerateCsiCursorPosition(presentationUpperInnerBorderLeft + 1, presentationUpperInnerBorderTop + 1));
            return builder.ToString();
        }

        static PresentationTools()
        {
            if (!ConsoleChecker.busy)
                ConsoleChecker.CheckConsole();
        }
    }
}
