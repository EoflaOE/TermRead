﻿//
// Nitrocid KS  Copyright (C) 2018-2024  Aptivi
//
// This file is part of Nitrocid KS
//
// Nitrocid KS is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Nitrocid KS is distributed in the hope that it will be useful,
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
using Terminaux.Writer.ConsoleWriters;
using Terminaux.Writer.FancyWriters.Tools;
using Terminaux.Colors;
using System.Text;
using Terminaux.Base.Buffered;
using Terminaux.Writer.FancyWriters;
using Terminaux.Base;
using System.Diagnostics;
using Textify.Sequences.Builder.Types;
using Textify.General;

namespace Terminaux.Inputs.Styles.Infobox
{
    /// <summary>
    /// Info box writer with buttons and color support
    /// </summary>
    public static class InfoBoxButtonsColor
    {
        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsPlain(string[] buttons, string text, params object[] vars) =>
            WriteInfoBoxButtonsPlain("", buttons, text,
                             BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                             BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                             BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                             BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
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
        public static int WriteInfoBoxButtonsPlain(string[] buttons, string text,
                                            char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                            char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar, params object[] vars) =>
            WriteInfoBoxButtonsPlain("", buttons, text,
                             UpperLeftCornerChar, LowerLeftCornerChar,
                             UpperRightCornerChar, LowerRightCornerChar,
                             UpperFrameChar, LowerFrameChar,
                             LeftFrameChar, RightFrameChar, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtons(string[] buttons, string text, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(buttons, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        new Color(ConsoleColors.Gray), ColorTools.currentBackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="InfoBoxButtonsColor">InfoBoxButtons color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColor(string[] buttons, string text, ConsoleColors InfoBoxButtonsColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(buttons, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        new Color(InfoBoxButtonsColor), ColorTools.currentBackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="InfoBoxButtonsColor">InfoBoxButtons color from Nitrocid KS's <see cref="Color"/></param>
        /// <param name="BackgroundColor">InfoBoxButtons background color from Nitrocid KS's <see cref="Color"/></param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColorBack(string[] buttons, string text, ConsoleColors InfoBoxButtonsColor, ConsoleColors BackgroundColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(buttons, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        new Color(InfoBoxButtonsColor), new Color(BackgroundColor), vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="InfoBoxButtonsColor">InfoBoxButtons color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColor(string[] buttons, string text, Color InfoBoxButtonsColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(buttons, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        InfoBoxButtonsColor, ColorTools.currentBackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="InfoBoxButtonsColor">InfoBoxButtons color from Nitrocid KS's <see cref="Color"/></param>
        /// <param name="BackgroundColor">InfoBoxButtons background color from Nitrocid KS's <see cref="Color"/></param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColorBack(string[] buttons, string text, Color InfoBoxButtonsColor, Color BackgroundColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(buttons, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        InfoBoxButtonsColor, BackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
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
        public static int WriteInfoBoxButtons(string[] buttons, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(buttons, text,
                UpperLeftCornerChar, LowerLeftCornerChar,
                UpperRightCornerChar, LowerRightCornerChar,
                UpperFrameChar, LowerFrameChar,
                LeftFrameChar, RightFrameChar,
                new Color(ConsoleColors.Gray), ColorTools.currentBackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="InfoBoxButtonsColor">InfoBoxButtons color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColor(string[] buttons, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar,
                                       ConsoleColors InfoBoxButtonsColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(buttons, text, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar, new Color(InfoBoxButtonsColor), ColorTools.currentBackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="InfoBoxButtonsColor">InfoBoxButtons color</param>
        /// <param name="BackgroundColor">InfoBoxButtons background color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColorBack(string[] buttons, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar,
                                       ConsoleColors InfoBoxButtonsColor, ConsoleColors BackgroundColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(
                buttons, text,
                UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar,
                UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar,
                new Color(InfoBoxButtonsColor), new Color(BackgroundColor), vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="InfoBoxButtonsColor">InfoBoxButtons color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColor(string[] buttons, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar,
                                       Color InfoBoxButtonsColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(buttons, text, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar, InfoBoxButtonsColor, ColorTools.currentBackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="InfoBoxButtonsColor">InfoBoxButtons color</param>
        /// <param name="BackgroundColor">InfoBoxButtons background color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColorBack(string[] buttons, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar,
                                       Color InfoBoxButtonsColor, Color BackgroundColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack("", buttons, text, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar, InfoBoxButtonsColor, BackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsPlain(string title, string[] buttons, string text, params object[] vars) =>
            WriteInfoBoxButtonsPlain(title, buttons, text,
                             BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                             BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                             BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                             BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
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
        public static int WriteInfoBoxButtonsPlain(string title, string[] buttons, string text,
                                            char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                            char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(title, buttons, text,
                UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar,
                UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar,
                ColorTools.currentForegroundColor, ColorTools.currentBackgroundColor, false, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtons(string title, string[] buttons, string text, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(title, buttons, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        new Color(ConsoleColors.Gray), ColorTools.currentBackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="InfoBoxTitledButtonsColor">InfoBoxTitledButtons color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColor(string title, string[] buttons, string text, ConsoleColors InfoBoxTitledButtonsColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(title, buttons, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        new Color(InfoBoxTitledButtonsColor), ColorTools.currentBackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="InfoBoxTitledButtonsColor">InfoBoxTitledButtons color from Nitrocid KS's <see cref="Color"/></param>
        /// <param name="BackgroundColor">InfoBoxTitledButtons background color from Nitrocid KS's <see cref="Color"/></param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColorBack(string title, string[] buttons, string text, ConsoleColors InfoBoxTitledButtonsColor, ConsoleColors BackgroundColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(title, buttons, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        new Color(InfoBoxTitledButtonsColor), new Color(BackgroundColor), vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="InfoBoxTitledButtonsColor">InfoBoxTitledButtons color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColor(string title, string[] buttons, string text, Color InfoBoxTitledButtonsColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(title, buttons, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        InfoBoxTitledButtonsColor, ColorTools.currentBackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="InfoBoxTitledButtonsColor">InfoBoxTitledButtons color from Nitrocid KS's <see cref="Color"/></param>
        /// <param name="BackgroundColor">InfoBoxTitledButtons background color from Nitrocid KS's <see cref="Color"/></param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColorBack(string title, string[] buttons, string text, Color InfoBoxTitledButtonsColor, Color BackgroundColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(title, buttons, text,
                        BorderTools.BorderUpperLeftCornerChar, BorderTools.BorderLowerLeftCornerChar,
                        BorderTools.BorderUpperRightCornerChar, BorderTools.BorderLowerRightCornerChar,
                        BorderTools.BorderUpperFrameChar, BorderTools.BorderLowerFrameChar,
                        BorderTools.BorderLeftFrameChar, BorderTools.BorderRightFrameChar,
                        InfoBoxTitledButtonsColor, BackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
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
        public static int WriteInfoBoxButtons(string title, string[] buttons, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(title, buttons, text,
                UpperLeftCornerChar, LowerLeftCornerChar,
                UpperRightCornerChar, LowerRightCornerChar,
                UpperFrameChar, LowerFrameChar,
                LeftFrameChar, RightFrameChar,
                new Color(ConsoleColors.Gray), ColorTools.currentBackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="InfoBoxTitledButtonsColor">InfoBoxTitledButtons color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColor(string title, string[] buttons, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar,
                                       ConsoleColors InfoBoxTitledButtonsColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(title, buttons, text, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar, new Color(InfoBoxTitledButtonsColor), ColorTools.currentBackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="InfoBoxTitledButtonsColor">InfoBoxTitledButtons color</param>
        /// <param name="BackgroundColor">InfoBoxTitledButtons background color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColorBack(string title, string[] buttons, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar,
                                       ConsoleColors InfoBoxTitledButtonsColor, ConsoleColors BackgroundColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(
                title, buttons, text,
                UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar,
                UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar,
                new Color(InfoBoxTitledButtonsColor), new Color(BackgroundColor), vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="InfoBoxTitledButtonsColor">InfoBoxTitledButtons color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColor(string title, string[] buttons, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar,
                                       Color InfoBoxTitledButtonsColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(title, buttons, text,
                UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar,
                UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar, InfoBoxTitledButtonsColor,
                ColorTools.currentBackgroundColor, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="InfoBoxTitledButtonsColor">InfoBoxTitledButtons color</param>
        /// <param name="BackgroundColor">InfoBoxTitledButtons background color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        public static int WriteInfoBoxButtonsColorBack(string title, string[] buttons, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar,
                                       Color InfoBoxTitledButtonsColor, Color BackgroundColor, params object[] vars) =>
            WriteInfoBoxButtonsColorBack(title, buttons, text,
                UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar,
                UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar,
                InfoBoxTitledButtonsColor, BackgroundColor, true, vars);

        /// <summary>
        /// Writes the info box plainly
        /// </summary>
        /// <param name="title">Title to be written</param>
        /// <param name="buttons">Button names to define. This must be from 1 to 3 buttons. Any more of them and you'll have to use the <see cref="InfoBoxSelectionColor"/> to get an option to use more buttons as choice selections.</param>
        /// <param name="UpperLeftCornerChar">Upper left corner character for info box</param>
        /// <param name="LowerLeftCornerChar">Lower left corner character for info box</param>
        /// <param name="UpperRightCornerChar">Upper right corner character for info box</param>
        /// <param name="LowerRightCornerChar">Lower right corner character for info box</param>
        /// <param name="UpperFrameChar">Upper frame character for info box</param>
        /// <param name="LowerFrameChar">Lower frame character for info box</param>
        /// <param name="LeftFrameChar">Left frame character for info box</param>
        /// <param name="RightFrameChar">Right frame character for info box</param>
        /// <param name="InfoBoxTitledButtonsColor">InfoBoxTitledButtons color</param>
        /// <param name="BackgroundColor">InfoBoxTitledButtons background color</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="useColor">Whether to use color or not</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <returns>Selected choice index (starting from zero), or -1 if exited, selection list is empty, or an error occurred</returns>
        internal static int WriteInfoBoxButtonsColorBack(string title, string[] buttons, string text,
                                       char UpperLeftCornerChar, char LowerLeftCornerChar, char UpperRightCornerChar, char LowerRightCornerChar,
                                       char UpperFrameChar, char LowerFrameChar, char LeftFrameChar, char RightFrameChar,
                                       Color InfoBoxTitledButtonsColor, Color BackgroundColor, bool useColor, params object[] vars)
        {
            // First, check the buttons count
            if (buttons is null || buttons.Length == 0)
                return -1;
            if (buttons.Length > 3)
            {
                // Looks like that we have more than three buttons. Use the selection choice instead.
                List<string> buttonNums = [];
                for (int i = 1; i <= buttons.Length; i++)
                    buttonNums.Add($"{i}");
                var choices = InputChoiceTools.GetInputChoices(buttonNums.ToArray(), buttons).ToArray();
                return InfoBoxSelectionColor.WriteInfoBoxSelectionColorBack(title, choices, text, InfoBoxTitledButtonsColor, BackgroundColor);
            }

            // Now, the button selection
            int selectedButton = 0;
            bool cancel = false;
            bool initialCursorVisible = ConsoleWrapper.CursorVisible;
            bool initialScreenIsNull = ScreenTools.CurrentScreen is null;
            var infoBoxScreenPart = new ScreenPart();
            var screen = new Screen();
            if (initialScreenIsNull)
            {
                infoBoxScreenPart.AddDynamicText(() =>
                {
                    ColorTools.SetConsoleColor(ColorTools.currentBackgroundColor, true);
                    return CsiSequences.GenerateCsiEraseInDisplay(2) + CsiSequences.GenerateCsiCursorPosition(1, 1);
                });
                ScreenTools.SetCurrent(screen);
            }
            ScreenTools.CurrentScreen.AddBufferedPart("Informational box", infoBoxScreenPart);
            try
            {
                infoBoxScreenPart.AddDynamicText(() =>
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
                    int maxWidth = ConsoleWrapper.WindowWidth - 4;
                    int maxHeight = splitFinalLines.Count + 5;
                    if (maxHeight >= ConsoleWrapper.WindowHeight)
                        maxHeight = ConsoleWrapper.WindowHeight - 4;
                    int maxRenderWidth = ConsoleWrapper.WindowWidth - 6;
                    int borderX = ConsoleWrapper.WindowWidth / 2 - maxWidth / 2 - 1;
                    int borderY = ConsoleWrapper.WindowHeight / 2 - maxHeight / 2 - 1;
                    var boxBuffer = new StringBuilder();
                    string border =
                        !string.IsNullOrEmpty(title) ?
                        BorderColor.RenderBorderPlain(title, borderX, borderY, maxWidth, maxHeight, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar) :
                        BorderColor.RenderBorderPlain(borderX, borderY, maxWidth, maxHeight, UpperLeftCornerChar, LowerLeftCornerChar, UpperRightCornerChar, LowerRightCornerChar, UpperFrameChar, LowerFrameChar, LeftFrameChar, RightFrameChar);
                    boxBuffer.Append(
                        $"{(useColor ? InfoBoxTitledButtonsColor.VTSequenceForeground : "")}" +
                        $"{(useColor ? BackgroundColor.VTSequenceBackground : "")}" +
                        $"{border}"
                    );

                    // Render text inside it
                    ConsoleWrapper.CursorVisible = false;
                    for (int i = 0; i < splitFinalLines.Count; i++)
                    {
                        var line = splitFinalLines[i];
                        if (i % (maxHeight - 5) == 0 && i > 0)
                        {
                            // Reached the end of the box. Bail, because we need to print the progress.
                            break;
                        }
                        boxBuffer.Append($"{CsiSequences.GenerateCsiCursorPosition(borderX + 2, borderY + 1 + i % maxHeight + 1)}{line}");
                    }

                    // Place the buttons from the right for familiarity
                    int buttonPanelPosX = borderX + 4;
                    int buttonPanelPosY = borderY + maxHeight - 3;
                    int maxButtonPanelWidth = maxWidth - 4;
                    int maxButtonWidth = maxButtonPanelWidth / 4 - 4;
                    for (int i = 1; i <= buttons.Length; i++)
                    {
                        // Get the text and the button position
                        string buttonText = buttons[i - 1];
                        int buttonX = maxButtonPanelWidth - i * maxButtonWidth;

                        // Determine whether it's a selected button or not
                        bool selected = i == selectedButton + 1;
                        var buttonForegroundColor = selected ? BackgroundColor : InfoBoxTitledButtonsColor;
                        var buttonBackgroundColor = selected ? InfoBoxTitledButtonsColor : BackgroundColor;

                        // Trim the button text to the max button width
                        buttonText = buttonText.Truncate(maxButtonWidth - 6);
                        int buttonTextX = buttonX + maxButtonWidth / 2 - buttonText.Length / 2;

                        // Render the button box
                        if (useColor)
                        {
                            boxBuffer.Append(
                                BorderColor.RenderBorder(title, buttonX, buttonPanelPosY, maxButtonWidth - 3, 1, buttonForegroundColor, buttonBackgroundColor) +
                                TextWriterWhereColor.RenderWhere(buttonText, buttonTextX, buttonPanelPosY + 1, buttonForegroundColor, buttonBackgroundColor)
                            );
                        }
                        else
                        {
                            boxBuffer.Append(
                                BorderColor.RenderBorderPlain(buttonX, buttonPanelPosY, maxButtonWidth - 3, 1) +
                                TextWriterWhereColor.RenderWherePlain(buttonText, buttonTextX, buttonPanelPosY + 1)
                            );
                        }
                    }

                    // Reset colors
                    if (useColor)
                    {
                        boxBuffer.Append(
                            ColorTools.currentForegroundColor.VTSequenceForeground +
                            ColorTools.currentBackgroundColor.VTSequenceBackground
                        );
                    }
                    return boxBuffer.ToString();
                });

                // Loop for input
                bool bail = false;
                while (!bail)
                {
                    // Render the screen
                    ScreenTools.Render();

                    // Wait for keypress
                    var key = Input.DetectKeypress().Key;
                    switch (key)
                    {
                        case ConsoleKey.LeftArrow:
                            selectedButton++;
                            if (selectedButton > buttons.Length - 1)
                                selectedButton = buttons.Length - 1;
                            break;
                        case ConsoleKey.RightArrow:
                            selectedButton--;
                            if (selectedButton < 0)
                                selectedButton = 0;
                            break;
                        case ConsoleKey.Enter:
                            bail = true;
                            break;
                        case ConsoleKey.Escape:
                            bail = true;
                            cancel = true;
                            break;
                    }
                }
            }
            catch (Exception ex) when (ex.GetType().Name != nameof(ThreadInterruptedException))
            {
                cancel = true;
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine("There is a serious error when printing text. {0}", ex.Message);
            }
            finally
            {
                if (useColor)
                {
                    TextWriterColor.WritePlain(
                        ColorTools.currentForegroundColor.VTSequenceForeground +
                        ColorTools.currentBackgroundColor.VTSequenceBackground
                    , false);
                }
                ConsoleWrapper.CursorVisible = initialCursorVisible;
                ScreenTools.CurrentScreen.RemoveBufferedPart("Informational box");
                if (initialScreenIsNull)
                    ScreenTools.UnsetCurrent(screen);
            }

            // Return the selected button
            if (cancel)
                selectedButton = -1;
            return selectedButton;
        }
    }
}
