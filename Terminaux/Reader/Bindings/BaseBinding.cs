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
using System.Collections.Generic;
using System.Text;
using Terminaux.Base;
using Terminaux.Reader.Tools;
using Terminaux.Sequences.Tools;

namespace Terminaux.Reader.Bindings
{
    /// <summary>
    /// Base key binding
    /// </summary>
    public abstract class BaseBinding : IBinding
    {
        /// <summary>
        /// Key to bind to
        /// </summary>
        public virtual ConsoleKeyInfo[] BoundKeys { get; }

        /// <summary>
        /// Does this binding cause the input to exit?
        /// </summary>
        public virtual bool IsExit { get; }

        /// <summary>
        /// Whether the binding matched
        /// </summary>
        /// <param name="input">Input key</param>
        public virtual bool BindMatched(ConsoleKeyInfo input)
        {
            bool match = false;
            foreach (var key in BoundKeys)
            {
                match = input.Key == key.Key &&
                        input.KeyChar == key.KeyChar &&
                        input.Modifiers == key.Modifiers;
                if (match)
                    break;
            }
            return match;
        }

        /// <summary>
        /// Do the action
        /// </summary>
        /// <param name="state">State of the reader</param>
        public virtual void DoAction(TermReaderState state)
        {
            // Insert the character, but in the condition that it's not a control character
            if (char.IsControl(state.pressedKey.KeyChar))
                return;
            int longestSentenceLength = ConsoleTools.ActionWindowWidth() - state.settings.RightMargin;
            string[] incompleteSentencesPrimary = ConsoleExtensions.GetWrappedSentences(state.CurrentText.ToString(), longestSentenceLength, state.inputPromptLeft + state.settings.LeftMargin);
            state.CurrentText.Insert(state.CurrentTextPos, state.pressedKey.KeyChar);

            // Re-write the text and set the current cursor position as appropriate
            string renderedText = state.PasswordMode ? new string(state.settings.PasswordMaskChar, state.currentText.ToString().Length) : state.currentText.ToString();
            string[] incompleteSentences = ConsoleExtensions.GetWrappedSentences(renderedText, longestSentenceLength, state.inputPromptLeft + state.settings.LeftMargin);

            // In the case of one line wrap, get the list of sentences
            if (state.OneLineWrap)
            {
                longestSentenceLength = ConsoleTools.ActionWindowWidth() - state.settings.RightMargin - state.inputPromptLeft - 1;
                incompleteSentences = ConsoleExtensions.GetWrappedSentences(renderedText, longestSentenceLength, 0);
                renderedText = state.OneLineWrap ? GetOneLineWrappedSentenceToRender(incompleteSentences, state) : renderedText;
                ConsoleTools.ActionSetCursorPosition(state.InputPromptLeft, state.InputPromptTop);
                ConsoleTools.ActionWriteString(renderedText + new string(' ', longestSentenceLength - renderedText.Length), state.settings);
                PositioningTools.GoForwardOneLineWrapAware(1, ref state);
            }
            else
            {
                ConsoleTools.ActionSetCursorPosition(state.InputPromptLeft, state.InputPromptTop);
                ConsoleTools.ActionWriteString(renderedText, state.settings);
                PositioningTools.HandleTopChangeForInput(ref state);
                PositioningTools.GoForward(1, ref state);
                if (state.inputPromptTop + incompleteSentences.Length > ConsoleTools.ActionBufferHeight())
                {
                    state.inputPromptTop -= incompleteSentences.Length - incompleteSentencesPrimary.Length;
                    state.currentCursorPosTop -= incompleteSentences.Length - incompleteSentencesPrimary.Length;
                }
            }
            ConsoleTools.ActionSetCursorPosition(state.CurrentCursorPosLeft, state.CurrentCursorPosTop);
        }

        internal static string GetOneLineWrappedSentenceToRender(string[] incompleteSentences, TermReaderState state) =>
            GetOneLineWrappedSentenceToRender(incompleteSentences, state.CurrentTextPos);

        internal static string GetOneLineWrappedSentenceToRender(string[] incompleteSentences, int targetIndex)
        {
            string finalRenderedString = "";

            // Deal with trying to count the characters incrementally for each incomplete sentence until we find an index
            // that we want, then give the rendered string back.
            int currentIndex = 0;
            foreach (string sentence in incompleteSentences)
            {
                finalRenderedString = sentence;
                for (int i = 0; i < sentence.Length && currentIndex != targetIndex; i++)
                    currentIndex++;
                if (currentIndex == targetIndex)
                    break;
            }

            // Return it!
            return finalRenderedString;
        }
    }
}
