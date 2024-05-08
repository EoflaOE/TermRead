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

using EnumMagic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Terminaux.Base;
using Terminaux.Base.Buffered;
using Terminaux.Base.Checks;
using Terminaux.Base.Extensions;
using Terminaux.Colors;
using Terminaux.Inputs.Pointer;
using Terminaux.Inputs.Styles.Infobox;
using Terminaux.Reader;
using Terminaux.Sequences;
using Terminaux.Sequences.Builder.Types;
using Terminaux.Writer.ConsoleWriters;
using Terminaux.Writer.FancyWriters;
using Textify.General;

namespace Terminaux.Inputs.Interactive
{
    /// <summary>
    /// Tools for the interactive TUI implementation
    /// </summary>
    public static class InteractiveTuiTools
    {

        private static string _finalInfoRendered = "";
        private static readonly object _interactiveTuiLock = new();

        /// <summary>
        /// Opens the interactive TUI
        /// </summary>
        /// <param name="interactiveTui">The inherited class instance of the interactive TUI</param>
        /// <exception cref="TerminauxException"></exception>
        public static void OpenInteractiveTui<T>(BaseInteractiveTui<T> interactiveTui)
        {
            lock (_interactiveTuiLock)
            {
                if (interactiveTui is null)
                    throw new TerminauxException("Please provide a base Interactive TUI class and try again.");
                BaseInteractiveTui<T>.instances.Add(interactiveTui);

                // First, check to see if the interactive TUI has no data source
                if (interactiveTui.PrimaryDataSource is null && interactiveTui.SecondaryDataSource is null ||
                    interactiveTui.PrimaryDataSource.Length() == 0 && interactiveTui.SecondaryDataSource.Length() == 0 && !interactiveTui.AcceptsEmptyData)
                {
                    InfoBoxColor.WriteInfoBoxColorBack("The interactive TUI {0} doesn't contain any data source. This program can't continue.\n" + "Press any key to continue...", InteractiveTuiStatus.BoxForegroundColor, InteractiveTuiStatus.BoxBackgroundColor, interactiveTui.GetType().Name);
                    return;
                }
                bool notifyCrash = false;
                string crashReason = "";

                // Make the screen
                var screen = new Screen();
                ScreenTools.SetCurrent(screen);
                interactiveTui.screen = screen;

                // Now, run the application
                try
                {
                    // Loop until the user requests to exit
                    while (!interactiveTui.isExiting)
                    {
                        // Check the selection
                        interactiveTui.LastOnOverflow();
                        interactiveTui.FirstOnUnderflow();

                        // Draw the boxes
                        DrawInteractiveTui(interactiveTui);

                        // Draw the first pane
                        DrawInteractiveTuiItems(interactiveTui, 1);

                        // Draw the second pane
                        if (interactiveTui.SecondPaneInteractable)
                            DrawInteractiveTuiItems(interactiveTui, 2);
                        else
                            DrawInformationOnSecondPane(interactiveTui);
                        DrawStatus(interactiveTui);

                        // Wait for user input
                        ScreenTools.Render(screen);
                        RespondToUserInput(interactiveTui);
                    }
                }
                catch (Exception ex)
                {
                    notifyCrash = true;
                    crashReason = TextTools.FormatString("The interactive TUI, {0}, has crashed for the following reason:", interactiveTui.GetType().Name) + $" {ex.Message}";
                }
                finally
                {
                    BaseInteractiveTui<T>.instances.Remove(interactiveTui);
                }
                ScreenTools.UnsetCurrent(screen);

                // Clear the console to clean up
                ColorTools.LoadBack();

                // If there is a crash, notify the user about it
                if (notifyCrash)
                {
                    notifyCrash = false;
                    InfoBoxColor.WriteInfoBoxColorBack(crashReason + "\n" + "Press any key to continue...", InteractiveTuiStatus.BoxForegroundColor, InteractiveTuiStatus.BoxBackgroundColor);
                }

                // Reset some static variables
                InteractiveTuiStatus.CurrentPane = 1;
                InteractiveTuiStatus.FirstPaneCurrentSelection = 1;
                InteractiveTuiStatus.SecondPaneCurrentSelection = 1;
                InteractiveTuiStatus.Status = "";
            }
        }

        /// <summary>
        /// Initiates the selection movement
        /// </summary>
        /// <param name="interactiveTui">Interactive TUI to deal with</param>
        /// <param name="pos">Position to move the pane selection to</param>
        public static void SelectionMovement<T>(BaseInteractiveTui<T> interactiveTui, int pos)
        {
            // Check the position
            var data = InteractiveTuiStatus.CurrentPane == 2 ?
                       interactiveTui.SecondaryDataSource :
                       interactiveTui.PrimaryDataSource;
            int elements = data.Length();
            if (pos < 1)
                pos = 1;
            if (pos > elements)
                pos = elements;

            // Now, process the movement
            if (InteractiveTuiStatus.CurrentPane == 2)
                InteractiveTuiStatus.SecondPaneCurrentSelection = pos;
            else
                InteractiveTuiStatus.FirstPaneCurrentSelection = pos;
        }

        /// <summary>
        /// Switches between two panes
        /// </summary>
        /// <param name="interactiveTui">Interactive TUI to deal with</param>
        public static void SwitchSides<T>(BaseInteractiveTui<T> interactiveTui)
        {
            if (!interactiveTui.SecondPaneInteractable)
                return;
            InteractiveTuiStatus.CurrentPane++;
            if (InteractiveTuiStatus.CurrentPane > 2)
                InteractiveTuiStatus.CurrentPane = 1;
        }

        private static void DrawInteractiveTui<T>(BaseInteractiveTui<T> interactiveTui)
        {
            // Check to make sure that we don't get nulls on interactiveTui
            if (interactiveTui is null)
                throw new TerminauxInternalException("Attempted to render TUI items on null");

            // Remove the old screen part
            string partName = $"Interactive TUI - Main - {interactiveTui.GetType().Name}";
            if (interactiveTui.trackedParts.TryGetValue(partName, out var oldPart))
            {
                interactiveTui.screen?.RemoveBufferedPart(oldPart.Id);
                interactiveTui.trackedParts.Remove(partName);
            }

            // Make a screen part
            var part = new ScreenPart();

            // Prepare the console
            ConsoleWrapper.CursorVisible = false;
            int SeparatorHalfConsoleWidth = ConsoleWrapper.WindowWidth / 2;
            int SeparatorHalfConsoleWidthInterior = ConsoleWrapper.WindowWidth / 2 - 2;
            int SeparatorMaximumHeightInterior = ConsoleWrapper.WindowHeight - 4;

            // Make a separator that separates the two panes to make it look like Total Commander or Midnight Commander. We need information in the upper and the
            // lower part of the console, so we need to render the entire program to look like this: (just a concept mockup)
            //
            //       |  vvvvvvvvvvvvvvvvvvvv  (SeparatorHalfConsoleWidthInterior)
            //       | v                    v (SeparatorHalfConsoleWidth)
            // H: 0  |
            // H: 1  | a--------------------+c---------------------+ < ----> (SeparatorMinimumHeight)
            // H: 2  | |b                   ||d                    |  < ----> (SeparatorMinimumHeightInterior)
            // H: 3  | |                    ||                     |  <
            // H: 4  | |                    ||                     |  <
            // H: 5  | |                    ||                     |  <
            // H: 6  | |                    ||                     |  <
            // H: 7  | |                    ||                     |  <
            // H: 8  | |                    ||                     |  < ----> (SeparatorMaximumHeightInterior)
            // H: 9  | +--------------------++---------------------+ < ----> (SeparatorMaximumHeight)
            // H: 10 |
            //       | where a is the dimension for the first pane upper left corner           (0, SeparatorMinimumHeight                                     (usually 1))
            //       |   and b is the dimension for the first pane interior upper left corner  (1, SeparatorMinimumHeightInterior                             (usually 2))
            //       |   and c is the dimension for the second pane upper left corner          (SeparatorHalfConsoleWidth, SeparatorMinimumHeight             (usually 1))
            //       |   and d is the dimension for the second pane interior upper left corner (SeparatorHalfConsoleWidth + 1, SeparatorMinimumHeightInterior (usually 2))

            // First, the horizontal and vertical separators
            var finalForeColorFirstPane = InteractiveTuiStatus.CurrentPane == 1 ? InteractiveTuiStatus.PaneSelectedSeparatorColor : InteractiveTuiStatus.PaneSeparatorColor;
            var finalForeColorSecondPane = InteractiveTuiStatus.CurrentPane == 2 ? InteractiveTuiStatus.PaneSelectedSeparatorColor : InteractiveTuiStatus.PaneSeparatorColor;
            part.AddDynamicText(new(() =>
            {
                int SeparatorHalfConsoleWidthInterior = ConsoleWrapper.WindowWidth / 2 - 2;
                int SeparatorMinimumHeight = 1;
                int SeparatorMaximumHeightInterior = ConsoleWrapper.WindowHeight - 4;
                var builder = new StringBuilder();
                builder.Append(ColorTools.RenderSetConsoleColor(finalForeColorFirstPane));
                builder.Append(ColorTools.RenderSetConsoleColor(InteractiveTuiStatus.PaneBackgroundColor, true));
                builder.Append(BorderColor.RenderBorderPlain(0, SeparatorMinimumHeight, SeparatorHalfConsoleWidthInterior, SeparatorMaximumHeightInterior));
                return builder.ToString();
            }));
            part.AddDynamicText(new(() =>
            {
                int SeparatorHalfConsoleWidth = ConsoleWrapper.WindowWidth / 2;
                int SeparatorHalfConsoleWidthInterior = ConsoleWrapper.WindowWidth / 2 - 2;
                int SeparatorMinimumHeight = 1;
                int SeparatorMaximumHeightInterior = ConsoleWrapper.WindowHeight - 4;
                var builder = new StringBuilder();
                builder.Append(ColorTools.RenderSetConsoleColor(finalForeColorSecondPane));
                builder.Append(ColorTools.RenderSetConsoleColor(InteractiveTuiStatus.PaneBackgroundColor, true));
                builder.Append(BorderColor.RenderBorderPlain(SeparatorHalfConsoleWidth, SeparatorMinimumHeight, SeparatorHalfConsoleWidthInterior + (ConsoleWrapper.WindowWidth % 2 != 0 ? 1 : 0), SeparatorMaximumHeightInterior));
                return builder.ToString();
            }));

            // Populate appropriate bindings, depending on the SecondPaneInteractable value
            List<InteractiveTuiBinding> finalBindings;
            if (interactiveTui.Bindings is null || interactiveTui.Bindings.Length == 0)
                finalBindings =
                [
                    new InteractiveTuiBinding("Exit", ConsoleKey.Escape, null)
                ];
            else
                finalBindings = new(interactiveTui.Bindings)
                {
                    new InteractiveTuiBinding("Exit", ConsoleKey.Escape, null),
                    new InteractiveTuiBinding("Keybindings", ConsoleKey.K, null),
                };
            if (interactiveTui.SecondPaneInteractable)
                finalBindings.Add(
                    new InteractiveTuiBinding("Switch", ConsoleKey.Tab, null)
                );

            // Render the key bindings
            part.AddDynamicText(() =>
            {
                var bindingsBuilder = new StringBuilder(CsiSequences.GenerateCsiCursorPosition(1, ConsoleWrapper.WindowHeight));
                foreach (InteractiveTuiBinding binding in finalBindings)
                {
                    // First, check to see if the rendered binding info is going to exceed the console window width
                    string renderedBinding = $"{GetBindingKeyShortcut(binding, false)} {binding.BindingName}  ";
                    int bindingLength = ConsoleChar.EstimateCellWidth(renderedBinding);
                    int actualLength = ConsoleChar.EstimateCellWidth(VtSequenceTools.FilterVTSequences(bindingsBuilder.ToString()));
                    bool canDraw = bindingLength + actualLength < ConsoleWrapper.WindowWidth - 3;
                    if (canDraw)
                    {
                        bindingsBuilder.Append(
                            $"{ColorTools.RenderSetConsoleColor(InteractiveTuiStatus.KeyBindingOptionColor, false, true)}" +
                            $"{ColorTools.RenderSetConsoleColor(InteractiveTuiStatus.OptionBackgroundColor, true)}" +
                            GetBindingKeyShortcut(binding, false) +
                            $"{ColorTools.RenderSetConsoleColor(InteractiveTuiStatus.OptionForegroundColor)}" +
                            $"{ColorTools.RenderSetConsoleColor(InteractiveTuiStatus.BackgroundColor, true)}" +
                            $" {binding.BindingName}  "
                        );
                    }
                    else
                    {
                        // We can't render anymore, so just break and write a binding to show more
                        bindingsBuilder.Append(
                            $"{CsiSequences.GenerateCsiCursorPosition(ConsoleWrapper.WindowWidth - 2, ConsoleWrapper.WindowHeight)}" +
                            $"{ColorTools.RenderSetConsoleColor(InteractiveTuiStatus.KeyBindingOptionColor, false, true)}" +
                            $"{ColorTools.RenderSetConsoleColor(InteractiveTuiStatus.OptionBackgroundColor, true)}" +
                            " K "
                        );
                        break;
                    }
                }
                return bindingsBuilder.ToString();
            });

            // We've added the necessary buffer. Now, add that to the buffered part list
            interactiveTui.screen?.AddBufferedPart(partName, part);
            interactiveTui.trackedParts.Add(partName, part);
        }

        private static void DrawInteractiveTuiItems<T>(BaseInteractiveTui<T> interactiveTui, int paneNum)
        {
            // Check to make sure that we don't get nulls on interactiveTui
            if (interactiveTui is null)
                throw new TerminauxInternalException("Attempted to render TUI items on null");
            if (interactiveTui.screen is null)
                throw new TerminauxInternalException("Attempted to render TUI items on no screen");

            // Check to make sure that we're not rendering the second pane on the first-pane-only interactive TUI
            if (!interactiveTui.SecondPaneInteractable && paneNum > 1)
                throw new TerminauxInternalException("Tried to render interactive TUI items for the secondary pane on an interactive TUI that only allows interaction from one pane.");

            // Remove the old screen part
            string partName = $"Interactive TUI - Items [{paneNum}] - {interactiveTui.GetType().Name}";
            if (interactiveTui.trackedParts.TryGetValue(partName, out var oldPart))
            {
                interactiveTui.screen?.RemoveBufferedPart(oldPart.Id);
                interactiveTui.trackedParts.Remove(partName);
            }

            // Make a screen part
            var part = new ScreenPart();

            // Check the pane number
            if (paneNum < 1)
                paneNum = 1;
            if (paneNum > 2)
                paneNum = 2;

            // Get how many data are there in the chosen data source
            var data = paneNum == 2 ? interactiveTui.SecondaryDataSource : interactiveTui.PrimaryDataSource;
            int dataCount = data.Length();

            // Render the pane right away
            part.AddDynamicText(() =>
            {
                int SeparatorHalfConsoleWidth = ConsoleWrapper.WindowWidth / 2;
                int SeparatorHalfConsoleWidthInterior = ConsoleWrapper.WindowWidth / 2 - 2;
                int SeparatorMinimumHeightInterior = 2;
                int SeparatorMaximumHeightInterior = ConsoleWrapper.WindowHeight - 4;
                int answersPerPage = SeparatorMaximumHeightInterior;
                int paneCurrentSelection = paneNum == 2 ? InteractiveTuiStatus.SecondPaneCurrentSelection : InteractiveTuiStatus.FirstPaneCurrentSelection;
                int currentPage = (paneCurrentSelection - 1) / answersPerPage;
                int startIndex = answersPerPage * currentPage;
                var builder = new StringBuilder();
                for (int i = 0; i <= answersPerPage - 1; i++)
                {
                    // Populate the first pane
                    string finalEntry = "";
                    int finalIndex = i + startIndex;
                    T? dataObject = default;
                    if (finalIndex <= dataCount - 1)
                    {
                        dataObject = (T?)data.GetElementFromIndex(startIndex + i);
                        if (dataObject is null)
                            continue;

                        // Render an entry
                        var finalForeColor = finalIndex == paneCurrentSelection - 1 ? InteractiveTuiStatus.PaneSelectedItemForeColor : InteractiveTuiStatus.PaneItemForeColor;
                        var finalBackColor = finalIndex == paneCurrentSelection - 1 ? InteractiveTuiStatus.PaneSelectedItemBackColor : InteractiveTuiStatus.PaneItemBackColor;
                        int leftPos = paneNum == 2 ? SeparatorHalfConsoleWidth + 1 : 1;
                        int top = SeparatorMinimumHeightInterior + finalIndex - startIndex;
                        finalEntry = interactiveTui.GetEntryFromItem(dataObject).Truncate(SeparatorHalfConsoleWidthInterior - 4);
                        int width = ConsoleChar.EstimateCellWidth(finalEntry);
                        string text =
                            $"{CsiSequences.GenerateCsiCursorPosition(leftPos + 1, top + 1)}" +
                            $"{ColorTools.RenderSetConsoleColor(finalForeColor, false, true)}" +
                            $"{ColorTools.RenderSetConsoleColor(finalBackColor, true)}" +
                            finalEntry +
                            new string(' ', SeparatorHalfConsoleWidthInterior - width - (ConsoleWrapper.WindowWidth % 2 != 0 && paneNum == 2 ? 0 : 1)) +
                            $"{ColorTools.RenderSetConsoleColor(InteractiveTuiStatus.PaneItemBackColor, true)}";
                        builder.Append(text);
                    }
                    else
                        break;
                }

                // Render the vertical bar
                int left = paneNum == 2 ? SeparatorHalfConsoleWidthInterior * 2 + (ConsoleWrapper.WindowWidth % 2 != 0 && paneNum == 2 ? 2 : 1) : SeparatorHalfConsoleWidthInterior - 1;
                builder.Append(SliderVerticalColor.RenderVerticalSlider(paneCurrentSelection, dataCount, left, 1, 2, 2, InteractiveTuiStatus.PaneSeparatorColor, InteractiveTuiStatus.PaneBackgroundColor, false));
                return builder.ToString();
            });

            interactiveTui.screen?.AddBufferedPart(partName, part);
            interactiveTui.trackedParts.Add(partName, part);
        }

        private static void DrawInformationOnSecondPane<T>(BaseInteractiveTui<T> interactiveTui)
        {
            // Check to make sure that we don't get nulls on interactiveTui
            if (interactiveTui is null)
                throw new TerminauxInternalException("Attempted to draw info on null");
            if (interactiveTui.screen is null)
                throw new TerminauxInternalException("Attempted to draw info on no screen");

            // Check to make sure that we're not rendering the information pane on the both-panes interactive TUI
            if (interactiveTui.SecondPaneInteractable)
                throw new TerminauxInternalException("Tried to render information the secondary pane on an interactive TUI that allows interaction from two panes, messing the selection rendering up there.");

            // Remove the old screen part
            string partName = $"Interactive TUI - Info (2nd pane) - {interactiveTui.GetType().Name}";
            if (interactiveTui.trackedParts.TryGetValue(partName, out var oldPart))
            {
                interactiveTui.screen?.RemoveBufferedPart(oldPart.Id);
                interactiveTui.trackedParts.Remove(partName);
            }

            // Make a screen part
            var part = new ScreenPart();

            // Populate some positions
            int SeparatorHalfConsoleWidth = ConsoleWrapper.WindowWidth / 2;
            int SeparatorHalfConsoleWidthInterior = ConsoleWrapper.WindowWidth / 2 - 2;
            int SeparatorMaximumHeightInterior = ConsoleWrapper.WindowHeight - 4;

            // Populate some colors
            var ForegroundColor = InteractiveTuiStatus.ForegroundColor;
            var PaneItemBackColor = InteractiveTuiStatus.PaneItemBackColor;

            // Now, do the job!
            string finalInfoRendered;
            try
            {
                // Populate data source and its count
                int paneCurrentSelection = InteractiveTuiStatus.CurrentPane == 2 ?
                                           InteractiveTuiStatus.SecondPaneCurrentSelection :
                                           InteractiveTuiStatus.FirstPaneCurrentSelection;
                var data = InteractiveTuiStatus.CurrentPane == 2 ?
                           interactiveTui.SecondaryDataSource :
                           interactiveTui.PrimaryDataSource;
                int dataCount = data.Length();

                // Populate selected data
                if (dataCount > 0)
                {
                    T selectedData = (T)(data.GetElementFromIndex(paneCurrentSelection - 1) ??
                        throw new TerminauxInternalException("Attempted to render info about null data"));
                    finalInfoRendered = interactiveTui.GetInfoFromItem(selectedData);
                }
                else
                {
                    finalInfoRendered = "No info.";
                }
            }
            catch
            {
                finalInfoRendered = "Failed to get information.";
            }

            // Now, write info
            var finalForeColorSecondPane = InteractiveTuiStatus.CurrentPane == 2 ? InteractiveTuiStatus.PaneSelectedSeparatorColor : InteractiveTuiStatus.PaneSeparatorColor;
            part.AddDynamicText(() =>
            {
                int SeparatorHalfConsoleWidth = ConsoleWrapper.WindowWidth / 2;
                int SeparatorHalfConsoleWidthInterior = ConsoleWrapper.WindowWidth / 2 - 2;
                int SeparatorMinimumHeight = 1;
                int SeparatorMinimumHeightInterior = 2;
                int SeparatorMaximumHeightInterior = ConsoleWrapper.WindowHeight - 4;
                var builder = new StringBuilder();
                builder.Append(ColorTools.RenderSetConsoleColor(finalForeColorSecondPane));
                builder.Append(ColorTools.RenderSetConsoleColor(InteractiveTuiStatus.PaneBackgroundColor, true));
                builder.Append(BorderColor.RenderBorderPlain(SeparatorHalfConsoleWidth, SeparatorMinimumHeight, SeparatorHalfConsoleWidthInterior + (ConsoleWrapper.WindowWidth % 2 != 0 ? 1 : 0), SeparatorMaximumHeightInterior));

                _finalInfoRendered = finalInfoRendered;
                string[] finalInfoStrings = ConsoleMisc.GetWrappedSentencesByWords(finalInfoRendered, SeparatorHalfConsoleWidthInterior);
                for (int infoIndex = 0; infoIndex < finalInfoStrings.Length; infoIndex++)
                {
                    // Check to see if the info is overpopulated
                    if (infoIndex >= SeparatorMaximumHeightInterior - 1)
                    {
                        string truncated = "Shift+I = more info";
                        int truncatedWidth = ConsoleChar.EstimateCellWidth(truncated);
                        builder.Append(ColorTools.RenderSetConsoleColor(ForegroundColor));
                        builder.Append(ColorTools.RenderSetConsoleColor(PaneItemBackColor, true));
                        builder.Append(TextWriterWhereColor.RenderWhere(truncated + new string(' ', SeparatorHalfConsoleWidthInterior - truncatedWidth), SeparatorHalfConsoleWidth + 1, SeparatorMinimumHeightInterior + infoIndex));
                        break;
                    }

                    // Now, render the info
                    string finalInfo = finalInfoStrings[infoIndex];
                    int finalInfoWidth = ConsoleChar.EstimateCellWidth(finalInfo);
                    builder.Append(ColorTools.RenderSetConsoleColor(ForegroundColor));
                    builder.Append(ColorTools.RenderSetConsoleColor(PaneItemBackColor, true));
                    builder.Append(TextWriterWhereColor.RenderWhere(finalInfo + new string(' ', SeparatorHalfConsoleWidthInterior - finalInfoWidth), SeparatorHalfConsoleWidth + 1, SeparatorMinimumHeightInterior + infoIndex));
                }
                return builder.ToString();
            });

            interactiveTui.screen?.AddBufferedPart(partName, part);
            interactiveTui.trackedParts.Add(partName, part);
        }

        private static void DrawStatus<T>(BaseInteractiveTui<T> interactiveTui)
        {
            if (interactiveTui is null)
                throw new TerminauxInternalException("Attempted to draw status on null");
            if (interactiveTui.screen is null)
                throw new TerminauxInternalException("Attempted to draw status on no screen");

            // Remove the old screen part
            string partName = $"Interactive TUI - Status - {interactiveTui.GetType().Name}";
            if (interactiveTui.trackedParts.TryGetValue(partName, out var oldPart))
            {
                interactiveTui.screen?.RemoveBufferedPart(oldPart.Id);
                interactiveTui.trackedParts.Remove(partName);
            }

            // Make a screen part
            var part = new ScreenPart();

            // Populate some necessary variables
            int paneCurrentSelection = InteractiveTuiStatus.CurrentPane == 2 ?
                                       InteractiveTuiStatus.SecondPaneCurrentSelection :
                                       InteractiveTuiStatus.FirstPaneCurrentSelection;
            var data = InteractiveTuiStatus.CurrentPane == 2 ?
                       interactiveTui.SecondaryDataSource :
                       interactiveTui.PrimaryDataSource;
            T selectedData = (T)data.GetElementFromIndex(paneCurrentSelection - 1);
            InteractiveTuiStatus.Status = interactiveTui.GetStatusFromItem(selectedData);

            // Now, write info
            part.AddDynamicText(() =>
            {
                var builder = new StringBuilder();
                builder.Append(ColorTools.RenderSetConsoleColor(InteractiveTuiStatus.ForegroundColor));
                builder.Append(ColorTools.RenderSetConsoleColor(InteractiveTuiStatus.BackgroundColor, true));
                builder.Append(TextWriterWhereColor.RenderWhere(InteractiveTuiStatus.Status.Truncate(ConsoleWrapper.WindowWidth - 3), 0, 0));
                builder.Append(ConsoleClearing.GetClearLineToRightSequence());
                return builder.ToString();
            });

            interactiveTui.screen?.AddBufferedPart(partName, part);
            interactiveTui.trackedParts.Add(partName, part);
        }

        private static void RespondToUserInput<T>(BaseInteractiveTui<T> interactiveTui)
        {
            // Check to make sure that we don't get nulls on interactiveTui
            if (interactiveTui is null)
                throw new TerminauxInternalException("Attempted to respond to user input on null");

            // Populate some necessary variables
            int paneCurrentSelection = InteractiveTuiStatus.CurrentPane == 2 ?
                                       InteractiveTuiStatus.SecondPaneCurrentSelection :
                                       InteractiveTuiStatus.FirstPaneCurrentSelection;
            var data = InteractiveTuiStatus.CurrentPane == 2 ?
                       interactiveTui.SecondaryDataSource :
                       interactiveTui.PrimaryDataSource;
            int dataCount = data.Length();

            // Populate selected data
            object selectedData = data.GetElementFromIndex(paneCurrentSelection - 1);

            // Wait for key
            bool loopBail = false;
            bool timed = interactiveTui.RefreshInterval > 0 && !interactiveTui.SecondPaneInteractable;
            Stopwatch sw = new();
            if (timed)
                sw.Start();
            while (!loopBail)
            {
                SpinWait.SpinUntil(() => PointerListener.InputAvailable || (timed && sw.ElapsedMilliseconds >= interactiveTui.RefreshInterval));
                bool timedOut = timed && sw.ElapsedMilliseconds >= interactiveTui.RefreshInterval;
                if (timedOut)
                {
                    sw.Restart();
                    loopBail = true;
                    continue;
                }
                if (PointerListener.PointerAvailable)
                {
                    void UpdateSelectionBasedOnMouse(PointerEventContext mouse)
                    {
                        // First, check to see if the cursor has moved to the other side or not
                        int SeparatorMinimumHeight = 1;
                        int SeparatorMaximumHeightInterior = ConsoleWrapper.WindowHeight - 4;
                        if (mouse.Coordinates.y < SeparatorMinimumHeight || mouse.Coordinates.y > SeparatorMaximumHeightInterior + 2)
                            return;
                        int SeparatorHalfConsoleWidth = ConsoleWrapper.WindowWidth / 2;
                        int SeparatorHalfConsoleWidthInterior = ConsoleWrapper.WindowWidth / 2 - 2;
                        bool refresh = false;
                        int oldPane = InteractiveTuiStatus.CurrentPane;
                        if (interactiveTui.SecondPaneInteractable)
                        {
                            if (mouse.Coordinates.x >= 2 && mouse.Coordinates.x <= SeparatorHalfConsoleWidthInterior - 1)
                            {
                                if (InteractiveTuiStatus.CurrentPane != 1)
                                {
                                    InteractiveTuiStatus.CurrentPane = 1;
                                    refresh = true;
                                }
                            }
                            else if (mouse.Coordinates.x >= SeparatorHalfConsoleWidth + 1 && mouse.Coordinates.x <= SeparatorHalfConsoleWidth + SeparatorHalfConsoleWidthInterior - 1)
                            {
                                if (InteractiveTuiStatus.CurrentPane != 2)
                                {
                                    InteractiveTuiStatus.CurrentPane = 2;
                                    refresh = true;
                                }
                            }
                            else
                                return;
                        }
                        else
                        {
                            if (mouse.Coordinates.x >= SeparatorHalfConsoleWidth - 1 && mouse.Coordinates.x <= SeparatorHalfConsoleWidth + SeparatorHalfConsoleWidthInterior + 1)
                                return;
                            if (mouse.Coordinates.x <= 1)
                                return;
                        }
                        if (refresh)
                        {
                            data =
                                InteractiveTuiStatus.CurrentPane == 2 ?
                                interactiveTui.SecondaryDataSource :
                                interactiveTui.PrimaryDataSource;
                            dataCount = data.Length();
                        }

                        // Now, update the selection relative to the mouse pointer location
                        int SeparatorMinimumHeightInterior = 2;
                        int answersPerPage = SeparatorMaximumHeightInterior;
                        paneCurrentSelection = InteractiveTuiStatus.CurrentPane == 2 ? InteractiveTuiStatus.SecondPaneCurrentSelection : InteractiveTuiStatus.FirstPaneCurrentSelection;
                        int currentPage = (paneCurrentSelection - 1) / answersPerPage;
                        int startIndex = answersPerPage * currentPage;
                        int endIndex = answersPerPage * (currentPage + 1) - 1;
                        if (mouse.Coordinates.y < SeparatorMinimumHeightInterior || mouse.Coordinates.y >= SeparatorMaximumHeightInterior + 2)
                            return;
                        int listIndex = mouse.Coordinates.y - SeparatorMinimumHeightInterior;
                        listIndex = startIndex + listIndex;
                        if (listIndex + 1 > dataCount)
                            return;
                        listIndex = listIndex > dataCount ? dataCount : listIndex;
                        if (listIndex + 1 != paneCurrentSelection || InteractiveTuiStatus.CurrentPane != oldPane)
                        {
                            if (listIndex + 1 != paneCurrentSelection)
                                SelectionMovement(interactiveTui, listIndex + 1);
                            loopBail = true;
                        }
                    }

                    // Mouse input received.
                    var mouse = TermReader.ReadPointer();
                    switch (mouse.Button)
                    {
                        case PointerButton.WheelUp:
                            loopBail = true;
                            if (InteractiveTuiStatus.CurrentPane == 2)
                                SelectionMovement(interactiveTui, InteractiveTuiStatus.SecondPaneCurrentSelection - 1);
                            else
                                SelectionMovement(interactiveTui, InteractiveTuiStatus.FirstPaneCurrentSelection - 1);
                            break;
                        case PointerButton.WheelDown:
                            loopBail = true;
                            if (InteractiveTuiStatus.CurrentPane == 2)
                                SelectionMovement(interactiveTui, InteractiveTuiStatus.SecondPaneCurrentSelection + 1);
                            else
                                SelectionMovement(interactiveTui, InteractiveTuiStatus.FirstPaneCurrentSelection + 1);
                            break;
                        case PointerButton.Left:
                            if (mouse.ButtonPress != PointerButtonPress.Released)
                                break;
                            UpdateSelectionBasedOnMouse(mouse);

                            // First, check the bindings
                            var allBindings = interactiveTui.Bindings;
                            if (allBindings is null || allBindings.Length == 0)
                                break;

                            // Now, get the implemented bindings from the pressed key
                            var implementedBindings = allBindings.Where((binding) =>
                                binding.BindingKeyName == ConsoleKey.Enter);
                            if (implementedBindings.Any())
                                loopBail = true;
                            foreach (var implementedBinding in implementedBindings)
                            {
                                var binding = implementedBinding.BindingAction;
                                if (binding is null)
                                    continue;
                                binding.Invoke(selectedData, paneCurrentSelection - 1);
                            }
                            break;
                        case PointerButton.None:
                            if (mouse.ButtonPress != PointerButtonPress.Moved)
                                break;
                            UpdateSelectionBasedOnMouse(mouse);
                            break;
                    }
                }
                else if (ConsoleWrapper.KeyAvailable && !PointerListener.PointerActive)
                {
                    var key = TermReader.ReadKey();
                    loopBail = true;
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (InteractiveTuiStatus.CurrentPane == 2)
                                SelectionMovement(interactiveTui, InteractiveTuiStatus.SecondPaneCurrentSelection - 1);
                            else
                                SelectionMovement(interactiveTui, InteractiveTuiStatus.FirstPaneCurrentSelection - 1);
                            break;
                        case ConsoleKey.DownArrow:
                            if (InteractiveTuiStatus.CurrentPane == 2)
                                SelectionMovement(interactiveTui, InteractiveTuiStatus.SecondPaneCurrentSelection + 1);
                            else
                                SelectionMovement(interactiveTui, InteractiveTuiStatus.FirstPaneCurrentSelection + 1);
                            break;
                        case ConsoleKey.Home:
                            SelectionMovement(interactiveTui, 1);
                            break;
                        case ConsoleKey.End:
                            SelectionMovement(interactiveTui, dataCount);
                            break;
                        case ConsoleKey.PageUp:
                            {
                                int SeparatorMaximumHeightInterior = ConsoleWrapper.WindowHeight - 4;
                                int answersPerPage = SeparatorMaximumHeightInterior;
                                int currentPage = (paneCurrentSelection - 1) / answersPerPage;
                                int startIndex = answersPerPage * currentPage;
                                SelectionMovement(interactiveTui, startIndex);
                            }
                            break;
                        case ConsoleKey.PageDown:
                            {
                                int SeparatorMaximumHeightInterior = ConsoleWrapper.WindowHeight - 4;
                                int answersPerPage = SeparatorMaximumHeightInterior;
                                int currentPage = (paneCurrentSelection - 1) / answersPerPage;
                                int startIndex = answersPerPage * (currentPage + 1) + 1;
                                SelectionMovement(interactiveTui, startIndex);
                            }
                            break;
                        case ConsoleKey.I:
                            if (key.Modifiers.HasFlag(ConsoleModifiers.Shift) && !string.IsNullOrEmpty(_finalInfoRendered))
                            {
                                // User needs more information in the infobox
                                InfoBoxColor.WriteInfoBoxColorBack(_finalInfoRendered, InteractiveTuiStatus.BoxForegroundColor, InteractiveTuiStatus.BoxBackgroundColor);
                            }
                            break;
                        case ConsoleKey.K:
                            // First, check the bindings length
                            var bindings = interactiveTui.Bindings;
                            if (bindings is null || bindings.Length == 0)
                                break;

                            // User needs an infobox that shows all available keys
                            string section = "Available keys";
                            int maxBindingLength = bindings
                                .Max((itb) => ConsoleChar.EstimateCellWidth(GetBindingKeyShortcut(itb)));
                            string[] bindingRepresentations = bindings
                                .Select((itb) => $"{GetBindingKeyShortcut(itb) + new string(' ', maxBindingLength - ConsoleChar.EstimateCellWidth(GetBindingKeyShortcut(itb))) + $" | {itb.BindingName}"}")
                                .ToArray();
                            InfoBoxColor.WriteInfoBoxColorBack(
                                $"{section}{CharManager.NewLine}" +
                                $"{new string('=', ConsoleChar.EstimateCellWidth(section))}{CharManager.NewLine}{CharManager.NewLine}" +
                                $"{string.Join("\n", bindingRepresentations)}"
                            , InteractiveTuiStatus.BoxForegroundColor, InteractiveTuiStatus.BoxBackgroundColor);
                            break;
                        case ConsoleKey.F:
                            // Search function
                            if (!data.Any())
                                break;
                            var entriesString = data.Select(interactiveTui.GetEntryFromItem).ToArray();
                            string keyword = InfoBoxInputColor.WriteInfoBoxInputColorBack("Write a search term (case insensitive)", InteractiveTuiStatus.BoxForegroundColor, InteractiveTuiStatus.BoxBackgroundColor).ToLower();
                            var resultEntries = entriesString.Select((entry, idx) => ($"{idx + 1}", entry)).Where((tuple) => tuple.entry.ToLower().Contains(keyword)).ToArray();
                            if (resultEntries.Length > 0)
                            {
                                var choices = InputChoiceTools.GetInputChoices(resultEntries);
                                int answer = InfoBoxSelectionColor.WriteInfoBoxSelection(choices, "Select one of the entries:");
                                if (answer < 0)
                                    break;
                                var resultIdx = int.Parse(resultEntries[answer].Item1);
                                SelectionMovement(interactiveTui, resultIdx);
                            }
                            else
                                InfoBoxColor.WriteInfoBoxColorBack("No item found.", InteractiveTuiStatus.BoxForegroundColor, InteractiveTuiStatus.BoxBackgroundColor);
                            break;
                        case ConsoleKey.Escape:
                            // User needs to exit
                            interactiveTui.HandleExit();
                            interactiveTui.isExiting = true;
                            break;
                        case ConsoleKey.Tab:
                            // User needs to switch sides
                            SwitchSides(interactiveTui);
                            break;
                        default:
                            // First, check the bindings
                            var allBindings = interactiveTui.Bindings;
                            if (allBindings is null || allBindings.Length == 0)
                                break;

                            // Now, get the implemented bindings from the pressed key
                            var implementedBindings = allBindings.Where((binding) =>
                                binding.BindingKeyName == key.Key && binding.BindingKeyModifiers == key.Modifiers);
                            foreach (var implementedBinding in implementedBindings)
                            {
                                var binding = implementedBinding.BindingAction;
                                if (binding is null)
                                    continue;
                                binding.Invoke(selectedData, paneCurrentSelection - 1);
                            }
                            break;
                    }
                }
            }
        }

        private static string GetBindingKeyShortcut(InteractiveTuiBinding bind, bool mark = true)
        {
            string markStart = mark ? "[" : " ";
            string markEnd = mark ? "]" : " ";
            return $"{markStart}{(bind.BindingKeyModifiers != 0 ? $"{bind.BindingKeyModifiers} + " : "")}{bind.BindingKeyName}{markEnd}";
        }

        static InteractiveTuiTools()
        {
            if (!ConsoleChecker.busy)
                ConsoleChecker.CheckConsole();
        }
    }
}
