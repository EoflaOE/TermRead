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
using Terminaux.Base;
using Terminaux.Reader.Tools;

namespace Terminaux.Reader.Bindings.BaseBindings
{
    internal class Rubout : BaseBinding, IBinding
    {
        /// <inheritdoc/>
        public override ConsoleKeyInfo[] BoundKeys { get; } =
        {
            new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
            new ConsoleKeyInfo('\u007f', ConsoleKey.Backspace, false, false, false),
            new ConsoleKeyInfo('\0', ConsoleKey.Backspace, false, false, false)
        };

        /// <inheritdoc/>
        public override void DoAction(TermReaderState state)
        {
            // If we're at the start of the text, bail.
            if (state.CurrentTextPos == 0)
                return;

            // Remove one character from the current text position
            state.CurrentText.Remove(state.CurrentTextPos - 1, 1);

            // Re-write the text and set the current cursor position as appropriate
            string renderedText = state.PasswordMode ? new string(state.settings.PasswordMaskChar, state.currentText.ToString().Length) : state.currentText.ToString();

            // In the case of one line wrap, get the list of sentences
            if (state.OneLineWrap)
            {
                int longestSentenceLength = ConsoleTools.ActionWindowWidth() - state.settings.RightMargin - state.inputPromptLeft - 1;
                string[] incompleteSentences = ConsoleExtensions.GetWrappedSentences(renderedText, longestSentenceLength, 0);
                renderedText = state.OneLineWrap ? GetOneLineWrappedSentenceToRender(incompleteSentences, state) : renderedText;
                ConsoleTools.ActionSetCursorPosition(state.InputPromptLeft, state.InputPromptTop);
                ConsoleTools.ActionWriteString(renderedText + new string(' ', longestSentenceLength - renderedText.Length), state.settings);
                PositioningTools.GoBackOneLineWrapAware(ref state);
            }
            else
            {
                ConsoleTools.ActionSetCursorPosition(state.InputPromptLeft, state.InputPromptTop);
                ConsoleTools.ActionWriteString(renderedText + " ", state.settings);
                PositioningTools.GoBack(ref state);
            }
            ConsoleTools.ActionSetCursorPosition(state.CurrentCursorPosLeft, state.CurrentCursorPosTop);
        }
    }
}
