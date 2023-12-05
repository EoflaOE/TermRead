﻿//
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
// but WITHOUT ANY WARRANTY, without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using System;
using System.Threading;
using System.Collections.Generic;
using Terminaux.Colors;
using System.Text;
using System.Linq;
using Terminaux.Writer.FancyWriters.Tools;
using Terminaux.Base;
using System.Diagnostics;
using Terminaux.Writer.ConsoleWriters;
using Terminaux.Writer.FancyWriters;
using Textify.General;
using Textify.Sequences.Builder.Types;

namespace Terminaux.Inputs.Styles.InfoboxTitled
{
    /// <summary>
    /// Info box writer with selection and color support
    /// </summary>
    public static class InfoBoxTitledSelectionColor
    {
        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="selections">List of choices</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxTitledSelectionPlain(string title, InputChoiceInfo[] selections, string text, params object[] vars) =>
            WriteInfoBoxTitledSelectionPlain(title, selections, text,
                             BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                             BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                             BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                             BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="selections">List of choices</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxTitledSelectionPlain(string title, InputChoiceInfo[] selections, string text,
                                            char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                            char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar, params object[] vars)
        {
            int selectedChoice = -1;

            // First, verify that we have selections
            if (selections is null || selections.Length == 0)
                return selectedChoice;

            bool initialCursorVisible = ConsoleWrapper.CursorVisible;
            try
            {
                // Deal with the lines to actually fit text in the infobox
                string finalInfoRendered = TextTools.FormatString(text, vars);
                string[] splitLines = finalInfoRendered.ToString().SplitNewLines();
                List<string> splitFinalLines = [];
                foreach (var line in splitLines)
                {
                    var lineSentences = TextTools.GetWrappedSentences(line, ConsoleWrapper.WindowWidth - 4);
                    foreach (var lineSentence in lineSentences)
                        splitFinalLines.Add(lineSentence);
                }

                // Trim the new lines until we reach a full line
                for (int i = splitFinalLines.Count - 1; i >= 0; i--)
                {
                    string line = splitFinalLines[i];
                    if (!string.IsNullOrWhiteSpace(line))
                        break;
                    splitFinalLines.RemoveAt(i);
                }

                // Fill the info box with text inside it
                int selectionChoices = selections.Length > 10 ? 10 : selections.Length;
                int selectionReservedHeight = 4 + selectionChoices;
                int maxWidth = ConsoleWrapper.WindowWidth - 4;
                int maxHeight = splitFinalLines.Count + selectionReservedHeight;
                if (maxHeight >= ConsoleWrapper.WindowHeight)
                    maxHeight = ConsoleWrapper.WindowHeight - 4;
                int maxRenderWidth = ConsoleWrapper.WindowWidth - 6;
                int borderX = ConsoleWrapper.WindowWidth / 2 - maxWidth / 2 - 1;
                int borderY = ConsoleWrapper.WindowHeight / 2 - maxHeight / 2 - 1;

                // Fill in some selection properties
                int selectionBoxPosX = borderX + 4;
                int selectionBoxPosY = borderY + maxHeight - selectionReservedHeight + 3;
                int maxSelectionWidth = maxWidth - selectionBoxPosX * 2 + 2;

                // Buffer the box
                var boxBuffer = new StringBuilder();
                string border = BorderTextColor.RenderBorderTextPlain(title, borderX, borderY, maxWidth, maxHeight, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar);
                string borderSelection = BorderTextColor.RenderBorderTextPlain(title, selectionBoxPosX, selectionBoxPosY - 1, maxSelectionWidth, selectionChoices, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar);
                boxBuffer.Append(border + borderSelection);

                // Render text inside it
                ConsoleWrapper.CursorVisible = false;
                for (int i = 0; i < splitFinalLines.Count; i++)
                {
                    var line = splitFinalLines[i];
                    if (i % (maxHeight - selectionReservedHeight) == 0 && i > 0)
                    {
                        // Reached the end of the box. Bail, because we need to print the selection box.
                        break;
                    }
                    boxBuffer.Append(
                        $"{CsiSequences.GenerateCsiCursorPosition(borderX + 2, borderY + 1 + i % maxHeight + 1)}" +
                        $"{line}"
                    );
                }

                // Render the final result
                TextWriterColor.WritePlain(boxBuffer.ToString(), false);

                // Wait for input
                bool bail = false;
                bool refresh = false;
                bool cancel = false;
                int currentSelection = 0;
                while (!bail)
                {
                    var input = new StringBuilder();

                    // Check to see if we need to refresh
                    if (refresh)
                    {
                        refresh = false;
                        TextWriterColor.WritePlain(boxBuffer.ToString(), false);
                    }

                    // Now, render the selections
                    int currentPage = currentSelection / selectionChoices;
                    int startIndex = selectionChoices * currentPage;
                    for (int i = 0; i <= selectionChoices - 1; i++)
                    {
                        // Populate the selection box
                        int finalIndex = i + startIndex;
                        if (finalIndex >= selections.Length)
                            break;
                        bool selected = finalIndex == currentSelection;
                        var choice = selections[finalIndex];
                        string AnswerTitle = choice.ChoiceTitle ?? "";

                        // Get the option
                        string AnswerOption = $"{(selected ? ">" : " ")} {choice}) {AnswerTitle}";
                        int AnswerTitleLeft = selections.Max(x => $"{(selected ? ">" : " ")} {x.ChoiceName}) ".Length);
                        int answerTitleMaxLeft = ConsoleWrapper.WindowWidth;
                        if (AnswerTitleLeft < answerTitleMaxLeft)
                        {
                            string renderedChoice = $"{(selected ? ">" : " ")} {choice.ChoiceName}) ";
                            int blankRepeats = AnswerTitleLeft - renderedChoice.Length;
                            AnswerOption = renderedChoice + new string(' ', blankRepeats) + $"{AnswerTitle}";
                        }
                        AnswerOption = AnswerOption.Truncate(maxSelectionWidth - 4);

                        // Render an entry
                        int leftPos = selectionBoxPosX + 1;
                        int top = selectionBoxPosY + finalIndex - startIndex;
                        input.Append(
                            TextWriterWhereColor.RenderWherePlain(AnswerOption + new string(' ', maxSelectionWidth - AnswerOption.Length - (ConsoleWrapper.WindowWidth % 2 != 0 ? 0 : 1)), leftPos, top)
                        );
                    }

                    // Render the vertical bar
                    int left = maxWidth - 3;
                    input.Append(
                        ProgressBarVerticalColor.RenderVerticalProgressPlain(100 * ((double)(currentSelection + 1) / selections.Length), left - 1, selectionBoxPosY - 1, ConsoleWrapper.WindowHeight - selectionChoices, 0, false)
                    );

                    // Handle keypress
                    TextWriterColor.WritePlain(input.ToString(), false);
                    var key = Input.DetectKeypress().Key;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            currentSelection--;
                            if (currentSelection < 0)
                                currentSelection = 0;
                            break;
                        case ConsoleKey.DownArrow:
                            currentSelection++;
                            if (currentSelection > selections.Length - 1)
                                currentSelection = selections.Length - 1;
                            break;
                        case ConsoleKey.Home:
                            currentSelection = 0;
                            break;
                        case ConsoleKey.End:
                            currentSelection = selections.Length - 1;
                            break;
                        case ConsoleKey.PageUp:
                            {
                                int currentPageMove = (currentSelection - 1) / selectionChoices;
                                int startIndexMove = selectionChoices * currentPageMove;
                                currentSelection = startIndexMove;
                                if (currentSelection < 0)
                                    currentSelection = 0;
                            }
                            break;
                        case ConsoleKey.PageDown:
                            {
                                int currentPageMove = currentSelection / selectionChoices;
                                int startIndexMove = selectionChoices * (currentPageMove + 1);
                                currentSelection = startIndexMove;
                                if (currentSelection > selections.Length - 1)
                                    currentSelection = selections.Length - 1;
                            }
                            break;
                        case ConsoleKey.Enter:
                            bail = true;
                            break;
                        case ConsoleKey.Escape:
                            bail = true;
                            cancel = true;
                            break;
                    }

                    // In case screensaver launched or window resized
                    refresh = ConsoleResizeListener.WasResized(false);
                }
                if (!cancel)
                    selectedChoice = currentSelection;
            }
            catch (Exception ex) when (ex.GetType().Name != nameof(ThreadInterruptedException))
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine("There is a serious error when printing text. {0}", ex.Message);
            }
            finally
            {
                ConsoleWrapper.CursorVisible = initialCursorVisible;
            }

            // Return the selected choice, or -1
            return selectedChoice;
        }

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="selections">List of choices</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxTitledSelection(string title, InputChoiceInfo[] selections, string text, params object[] vars) =>
            WriteInfoBoxTitledSelectionColor(title, selections, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        new Color(ConsoleColors.Gray), new Color(ConsoleColors.Black), vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="selections">List of choices</param>
        /// <param name="InfoBoxTitledSelectionColor">InfoBoxTitledSelection color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxTitledSelectionColor(string title, InputChoiceInfo[] selections, string text, ConsoleColors InfoBoxTitledSelectionColor, params object[] vars) =>
            WriteInfoBoxTitledSelectionColorBack(title, selections, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        new Color(InfoBoxTitledSelectionColor), new Color(ConsoleColors.Black), vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="selections">List of choices</param>
        /// <param name="InfoBoxTitledSelectionColor">InfoBoxTitledSelection color from Terminaux's <see cref="Color"/></param>
        /// <param name="BackgroundColor">InfoBoxTitledSelection background color from Terminaux's <see cref="Color"/></param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxTitledSelectionColorBack(string title, InputChoiceInfo[] selections, string text, ConsoleColors InfoBoxTitledSelectionColor, ConsoleColors BackgroundColor, params object[] vars) =>
            WriteInfoBoxTitledSelectionColorBack(title, selections, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        new Color(InfoBoxTitledSelectionColor), new Color(BackgroundColor), vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="selections">List of choices</param>
        /// <param name="InfoBoxTitledSelectionColor">InfoBoxTitledSelection color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxTitledSelectionColor(string title, InputChoiceInfo[] selections, string text, Color InfoBoxTitledSelectionColor, params object[] vars) =>
            WriteInfoBoxTitledSelectionColorBack(title, selections, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        InfoBoxTitledSelectionColor, new Color(ConsoleColors.Black), vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="selections">List of choices</param>
        /// <param name="InfoBoxTitledSelectionColor">InfoBoxTitledSelection color from Terminaux's <see cref="Color"/></param>
        /// <param name="BackgroundColor">InfoBoxTitledSelection background color from Terminaux's <see cref="Color"/></param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxTitledSelectionColorBack(string title, InputChoiceInfo[] selections, string text, Color InfoBoxTitledSelectionColor, Color BackgroundColor, params object[] vars) =>
            WriteInfoBoxTitledSelectionColorBack(title, selections, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        InfoBoxTitledSelectionColor, BackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="selections">List of choices</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxTitledSelection(string title, InputChoiceInfo[] selections, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar, params object[] vars) =>
            WriteInfoBoxTitledSelectionColor(title, selections, text,
                UpperLeftCornerChar, LowerLeftCornerChar,
                UpperRightCornerChar, LowerRightCornerChar,
                UpperFrameChar, LowerFrameChar,
                LeftFrameChar, RightFrameChar,
                new Color(ConsoleColors.Gray), new Color(ConsoleColors.Black), vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="selections">List of choices</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="InfoBoxTitledSelectionColor">InfoBoxTitledSelection color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxTitledSelectionColor(string title, InputChoiceInfo[] selections, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar,
                                       ConsoleColors InfoBoxTitledSelectionColor, params object[] vars) =>
            WriteInfoBoxTitledSelectionColorBack(title, selections, text, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar, new Color(InfoBoxTitledSelectionColor), new Color(ConsoleColors.Black), vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="selections">List of choices</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="InfoBoxTitledSelectionColor">InfoBoxTitledSelection color</param>
        /// <param name="BackgroundColor">InfoBoxTitledSelection background color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxTitledSelectionColorBack(string title, InputChoiceInfo[] selections, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar,
                                       Color InfoBoxTitledSelectionColor, Color BackgroundColor, params object[] vars)
        {
            int selectedChoice = -1;

            // First, verify that we have selections
            if (selections is null || selections.Length == 0)
                return selectedChoice;

            bool initialCursorVisible = ConsoleWrapper.CursorVisible;
            try
            {
                // Deal with the lines to actually fit text in the infobox
                string finalInfoRendered = TextTools.FormatString(text, vars);
                string[] splitLines = finalInfoRendered.ToString().SplitNewLines();
                List<string> splitFinalLines = [];
                foreach (var line in splitLines)
                {
                    var lineSentences = TextTools.GetWrappedSentences(line, ConsoleWrapper.WindowWidth - 4);
                    foreach (var lineSentence in lineSentences)
                        splitFinalLines.Add(lineSentence);
                }

                // Trim the new lines until we reach a full line
                for (int i = splitFinalLines.Count - 1; i >= 0; i--)
                {
                    string line = splitFinalLines[i];
                    if (!string.IsNullOrWhiteSpace(line))
                        break;
                    splitFinalLines.RemoveAt(i);
                }

                // Fill the info box with text inside it
                int selectionChoices = selections.Length > 10 ? 10 : selections.Length;
                int selectionReservedHeight = 4 + selectionChoices;
                int maxWidth = ConsoleWrapper.WindowWidth - 4;
                int maxHeight = splitFinalLines.Count + selectionReservedHeight;
                if (maxHeight >= ConsoleWrapper.WindowHeight)
                    maxHeight = ConsoleWrapper.WindowHeight - 4;
                int maxRenderWidth = ConsoleWrapper.WindowWidth - 6;
                int borderX = ConsoleWrapper.WindowWidth / 2 - maxWidth / 2 - 1;
                int borderY = ConsoleWrapper.WindowHeight / 2 - maxHeight / 2 - 1;

                // Fill in some selection properties
                int selectionBoxPosX = borderX + 4;
                int selectionBoxPosY = borderY + maxHeight - selectionReservedHeight + 3;
                int maxSelectionWidth = maxWidth - selectionBoxPosX * 2 + 2;

                // Buffer the box
                var boxBuffer = new StringBuilder();
                string border = BorderTextColor.RenderBorderTextPlain(title, borderX, borderY, maxWidth, maxHeight, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar);
                string borderSelection = BorderTextColor.RenderBorderTextPlain(title, selectionBoxPosX, selectionBoxPosY - 1, maxSelectionWidth, selectionChoices, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar);
                boxBuffer.Append(
                    $"{InfoBoxTitledSelectionColor.VTSequenceForeground}" +
                    $"{BackgroundColor.VTSequenceBackground}" +
                    $"{border}" +
                    $"{borderSelection}"
                );

                // Render text inside it
                ConsoleWrapper.CursorVisible = false;
                for (int i = 0; i < splitFinalLines.Count; i++)
                {
                    var line = splitFinalLines[i];
                    if (i % (maxHeight - selectionReservedHeight) == 0 && i > 0)
                    {
                        // Reached the end of the box. Bail, because we need to print the selection box.
                        break;
                    }
                    boxBuffer.Append(
                        $"{CsiSequences.GenerateCsiCursorPosition(borderX + 2, borderY + 1 + i % maxHeight + 1)}" +
                        $"{line}"
                    );
                }

                // Render the final result
                boxBuffer.Append(
                    new Color(ConsoleColors.White).VTSequenceForeground +
                    new Color(ConsoleColors.Black).VTSequenceBackground
                );
                TextWriterColor.WritePlain(boxBuffer.ToString(), false);

                // Wait for input
                bool bail = false;
                bool refresh = false;
                bool cancel = false;
                int currentSelection = 0;
                while (!bail)
                {
                    var input = new StringBuilder();

                    // Check to see if we need to refresh
                    if (refresh)
                    {
                        refresh = false;
                        TextWriterColor.WritePlain(boxBuffer.ToString(), false);
                    }

                    // Now, render the selections
                    int currentPage = currentSelection / selectionChoices;
                    int startIndex = selectionChoices * currentPage;
                    for (int i = 0; i <= selectionChoices - 1; i++)
                    {
                        // Populate the selection box
                        int finalIndex = i + startIndex;
                        if (finalIndex >= selections.Length)
                            break;
                        bool selected = finalIndex == currentSelection;
                        var choice = selections[finalIndex];
                        string AnswerTitle = choice.ChoiceTitle ?? "";

                        // Get the option
                        string AnswerOption = $"{(selected ? ">" : " ")} {choice}) {AnswerTitle}";
                        int AnswerTitleLeft = selections.Max(x => $"{(selected ? ">" : " ")} {x.ChoiceName}) ".Length);
                        int answerTitleMaxLeft = ConsoleWrapper.WindowWidth;
                        if (AnswerTitleLeft < answerTitleMaxLeft)
                        {
                            string renderedChoice = $"{(selected ? ">" : " ")} {choice.ChoiceName}) ";
                            int blankRepeats = AnswerTitleLeft - renderedChoice.Length;
                            AnswerOption = renderedChoice + new string(' ', blankRepeats) + $"{AnswerTitle}";
                        }
                        AnswerOption = AnswerOption.Truncate(maxSelectionWidth - 4);

                        // Render an entry
                        var finalForeColor = selected ? BackgroundColor : InfoBoxTitledSelectionColor;
                        var finalBackColor = selected ? InfoBoxTitledSelectionColor : BackgroundColor;
                        int leftPos = selectionBoxPosX + 1;
                        int top = selectionBoxPosY + finalIndex - startIndex;
                        input.Append(
                            TextWriterWhereColor.RenderWhere(AnswerOption + new string(' ', maxSelectionWidth - AnswerOption.Length - (ConsoleWrapper.WindowWidth % 2 != 0 ? 0 : 1)), leftPos, top, finalForeColor, finalBackColor)
                        );
                    }

                    // Render the vertical bar
                    int left = maxWidth - 3;
                    input.Append(
                        ProgressBarVerticalColor.RenderVerticalProgress(100 * ((double)(currentSelection + 1) / selections.Length), left - 1, selectionBoxPosY - 1, ConsoleWrapper.WindowHeight - selectionChoices, 0, BackgroundColor, BackgroundColor, false)
                    );

                    // Handle keypress
                    TextWriterColor.WritePlain(input.ToString(), false);
                    var key = Input.DetectKeypress().Key;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            currentSelection--;
                            if (currentSelection < 0)
                                currentSelection = 0;
                            break;
                        case ConsoleKey.DownArrow:
                            currentSelection++;
                            if (currentSelection > selections.Length - 1)
                                currentSelection = selections.Length - 1;
                            break;
                        case ConsoleKey.Home:
                            currentSelection = 0;
                            break;
                        case ConsoleKey.End:
                            currentSelection = selections.Length - 1;
                            break;
                        case ConsoleKey.PageUp:
                            {
                                int currentPageMove = (currentSelection - 1) / selectionChoices;
                                int startIndexMove = selectionChoices * currentPageMove;
                                currentSelection = startIndexMove;
                                if (currentSelection < 0)
                                    currentSelection = 0;
                            }
                            break;
                        case ConsoleKey.PageDown:
                            {
                                int currentPageMove = currentSelection / selectionChoices;
                                int startIndexMove = selectionChoices * (currentPageMove + 1);
                                currentSelection = startIndexMove;
                                if (currentSelection > selections.Length - 1)
                                    currentSelection = selections.Length - 1;
                            }
                            break;
                        case ConsoleKey.Enter:
                            bail = true;
                            break;
                        case ConsoleKey.Escape:
                            bail = true;
                            cancel = true;
                            break;
                    }

                    // In case screensaver launched or window resized
                    refresh = ConsoleResizeListener.WasResized(false);
                }
                if (!cancel)
                    selectedChoice = currentSelection;
            }
            catch (Exception ex) when (ex.GetType().Name != nameof(ThreadInterruptedException))
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine("There is a serious error when printing text. {0}", ex.Message);
            }
            finally
            {
                ConsoleWrapper.CursorVisible = initialCursorVisible;
            }

            // Return the selected choice, or -1
            return selectedChoice;
        }

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="selections">List of choices</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="InfoBoxTitledSelectionColor">InfoBoxTitledSelection color</param>
        /// <param name="BackgroundColor">InfoBoxTitledSelection background color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxTitledSelectionColorBack(string title, InputChoiceInfo[] selections, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar,
                                       ConsoleColors InfoBoxTitledSelectionColor, ConsoleColors BackgroundColor, params object[] vars)
        {
            int selectedChoice = -1;

            // First, verify that we have selections
            if (selections is null || selections.Length == 0)
                return selectedChoice;

            bool initialCursorVisible = ConsoleWrapper.CursorVisible;
            try
            {
                // Deal with the lines to actually fit text in the infobox
                string finalInfoRendered = TextTools.FormatString(text, vars);
                string[] splitLines = finalInfoRendered.ToString().SplitNewLines();
                List<string> splitFinalLines = [];
                foreach (var line in splitLines)
                {
                    var lineSentences = TextTools.GetWrappedSentences(line, ConsoleWrapper.WindowWidth - 4);
                    foreach (var lineSentence in lineSentences)
                        splitFinalLines.Add(lineSentence);
                }

                // Trim the new lines until we reach a full line
                for (int i = splitFinalLines.Count - 1; i >= 0; i--)
                {
                    string line = splitFinalLines[i];
                    if (!string.IsNullOrWhiteSpace(line))
                        break;
                    splitFinalLines.RemoveAt(i);
                }

                // Fill the info box with text inside it
                int selectionChoices = selections.Length > 10 ? 10 : selections.Length;
                int selectionReservedHeight = 4 + selectionChoices;
                int maxWidth = ConsoleWrapper.WindowWidth - 4;
                int maxHeight = splitFinalLines.Count + selectionReservedHeight;
                if (maxHeight >= ConsoleWrapper.WindowHeight)
                    maxHeight = ConsoleWrapper.WindowHeight - 4;
                int maxRenderWidth = ConsoleWrapper.WindowWidth - 6;
                int borderX = ConsoleWrapper.WindowWidth / 2 - maxWidth / 2 - 1;
                int borderY = ConsoleWrapper.WindowHeight / 2 - maxHeight / 2 - 1;

                // Fill in some selection properties
                int selectionBoxPosX = borderX + 4;
                int selectionBoxPosY = borderY + maxHeight - selectionReservedHeight + 3;
                int maxSelectionWidth = maxWidth - selectionBoxPosX * 2 + 2;

                // Buffer the box
                var boxBuffer = new StringBuilder();
                string border = BorderTextColor.RenderBorderTextPlain(title, borderX, borderY, maxWidth, maxHeight, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar);
                string borderSelection = BorderTextColor.RenderBorderTextPlain(title, selectionBoxPosX, selectionBoxPosY - 1, maxSelectionWidth, selectionChoices, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar);
                boxBuffer.Append(
                    $"{new Color(InfoBoxTitledSelectionColor).VTSequenceForeground}" +
                    $"{new Color(BackgroundColor).VTSequenceBackground}" +
                    $"{border}" +
                    $"{borderSelection}"
                );

                // Render text inside it
                ConsoleWrapper.CursorVisible = false;
                for (int i = 0; i < splitFinalLines.Count; i++)
                {
                    var line = splitFinalLines[i];
                    if (i % (maxHeight - selectionReservedHeight) == 0 && i > 0)
                    {
                        // Reached the end of the box. Bail, because we need to print the selection box.
                        break;
                    }
                    boxBuffer.Append(
                        $"{CsiSequences.GenerateCsiCursorPosition(borderX + 2, borderY + 1 + i % maxHeight + 1)}" +
                        $"{line}"
                    );
                }

                // Render the final result
                boxBuffer.Append(
                    new Color(ConsoleColors.White).VTSequenceForeground +
                    new Color(ConsoleColors.Black).VTSequenceBackground
                );
                TextWriterColor.WritePlain(boxBuffer.ToString(), false);

                // Wait for input
                bool bail = false;
                bool refresh = false;
                bool cancel = false;
                int currentSelection = 0;
                while (!bail)
                {
                    var input = new StringBuilder();

                    // Check to see if we need to refresh
                    if (refresh)
                    {
                        refresh = false;
                        TextWriterColor.WritePlain(boxBuffer.ToString(), false);
                    }

                    // Now, render the selections
                    int currentPage = currentSelection / selectionChoices;
                    int startIndex = selectionChoices * currentPage;
                    for (int i = 0; i <= selectionChoices - 1; i++)
                    {
                        // Populate the selection box
                        int finalIndex = i + startIndex;
                        if (finalIndex >= selections.Length)
                            break;
                        bool selected = finalIndex == currentSelection;
                        var choice = selections[finalIndex];
                        string AnswerTitle = choice.ChoiceTitle ?? "";

                        // Get the option
                        string AnswerOption = $"{(selected ? ">" : " ")} {choice}) {AnswerTitle}";
                        int AnswerTitleLeft = selections.Max(x => $"{(selected ? ">" : " ")} {x.ChoiceName}) ".Length);
                        int answerTitleMaxLeft = ConsoleWrapper.WindowWidth;
                        if (AnswerTitleLeft < answerTitleMaxLeft)
                        {
                            string renderedChoice = $"{(selected ? ">" : " ")} {choice.ChoiceName}) ";
                            int blankRepeats = AnswerTitleLeft - renderedChoice.Length;
                            AnswerOption = renderedChoice + new string(' ', blankRepeats) + $"{AnswerTitle}";
                        }
                        AnswerOption = AnswerOption.Truncate(maxSelectionWidth - 4);

                        // Render an entry
                        var finalForeColor = selected ? BackgroundColor : InfoBoxTitledSelectionColor;
                        var finalBackColor = selected ? InfoBoxTitledSelectionColor : BackgroundColor;
                        int leftPos = selectionBoxPosX + 1;
                        int top = selectionBoxPosY + finalIndex - startIndex;
                        input.Append(
                            TextWriterWhereColor.RenderWhere(AnswerOption + new string(' ', maxSelectionWidth - AnswerOption.Length - (ConsoleWrapper.WindowWidth % 2 != 0 ? 0 : 1)), leftPos, top, finalForeColor, finalBackColor)
                        );
                    }

                    // Render the vertical bar
                    int left = maxWidth - 3;
                    input.Append(
                        ProgressBarVerticalColor.RenderVerticalProgress(100 * ((double)(currentSelection + 1) / selections.Length), left - 1, selectionBoxPosY - 1, ConsoleWrapper.WindowHeight - selectionChoices, 0, BackgroundColor, BackgroundColor, false)
                    );

                    // Handle keypress
                    TextWriterColor.WritePlain(input.ToString(), false);
                    var key = Input.DetectKeypress().Key;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            currentSelection--;
                            if (currentSelection < 0)
                                currentSelection = 0;
                            break;
                        case ConsoleKey.DownArrow:
                            currentSelection++;
                            if (currentSelection > selections.Length - 1)
                                currentSelection = selections.Length - 1;
                            break;
                        case ConsoleKey.Home:
                            currentSelection = 0;
                            break;
                        case ConsoleKey.End:
                            currentSelection = selections.Length - 1;
                            break;
                        case ConsoleKey.PageUp:
                            {
                                int currentPageMove = (currentSelection - 1) / selectionChoices;
                                int startIndexMove = selectionChoices * currentPageMove;
                                currentSelection = startIndexMove;
                                if (currentSelection < 0)
                                    currentSelection = 0;
                            }
                            break;
                        case ConsoleKey.PageDown:
                            {
                                int currentPageMove = currentSelection / selectionChoices;
                                int startIndexMove = selectionChoices * (currentPageMove + 1);
                                currentSelection = startIndexMove;
                                if (currentSelection > selections.Length - 1)
                                    currentSelection = selections.Length - 1;
                            }
                            break;
                        case ConsoleKey.Enter:
                            bail = true;
                            break;
                        case ConsoleKey.Escape:
                            bail = true;
                            cancel = true;
                            break;
                    }

                    // In case screensaver launched or window resized
                    refresh = ConsoleResizeListener.WasResized(false);
                }
                if (!cancel)
                    selectedChoice = currentSelection;
            }
            catch (Exception ex) when (ex.GetType().Name != nameof(ThreadInterruptedException))
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine("There is a serious error when printing text. {0}", ex.Message);
            }
            finally
            {
                ConsoleWrapper.CursorVisible = initialCursorVisible;
            }

            // Return the selected choice, or -1
            return selectedChoice;
        }
    }
}
