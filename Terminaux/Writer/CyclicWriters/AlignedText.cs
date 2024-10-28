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

using Terminaux.Base;
using Terminaux.Base.Extensions;
using Terminaux.Colors;
using Terminaux.Writer.FancyWriters;
using Terminaux.Writer.FancyWriters.Tools;
using Textify.Data.Figlet;
using Textify.Data.Figlet.Utilities.Lines;
using Textify.General;

namespace Terminaux.Writer.CyclicWriters
{
    /// <summary>
    /// Aligned text renderable
    /// </summary>
    public class AlignedText : IStaticRenderable
    {
        private int top = 0;
        private string text = "";
        private bool oneLine = false;
        private int leftMargin = 0;
        private int rightMargin = 0;
        private Color foregroundColor = ColorTools.CurrentForegroundColor;
        private Color backgroundColor = ColorTools.CurrentBackgroundColor;
        private TextSettings settings = new();
        private bool customTop = false;
        private bool customColor = false;

        /// <summary>
        /// Top position
        /// </summary>
        public int Top
        {
            get => top;
            set
            {
                top = value;
                customTop = true;
            }
        }

        /// <summary>
        /// Top position
        /// </summary>
        public string Text
        {
            get => text;
            set
            {
                text = value;
                if (!customTop)
                    UpdateInternalTop();
            }
        }

        /// <summary>
        /// Left margin of the aligned figlet text
        /// </summary>
        public int LeftMargin
        {
            get => leftMargin;
            set
            {
                leftMargin = value;
                if (!customTop)
                    UpdateInternalTop();
            }
        }

        /// <summary>
        /// Right margin of the aligned figlet text
        /// </summary>
        public int RightMargin
        {
            get => rightMargin;
            set
            {
                rightMargin = value;
                if (!customTop)
                    UpdateInternalTop();
            }
        }

        /// <summary>
        /// Foreground color of the text
        /// </summary>
        public Color ForegroundColor
        {
            get => foregroundColor;
            set
            {
                foregroundColor = value;
                customColor = true;
            }
        }

        /// <summary>
        /// Background color of the text
        /// </summary>
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                customColor = true;
            }
        }

        /// <summary>
        /// Text settings to use
        /// </summary>
        public TextSettings Settings
        {
            get => settings;
            set => settings = value;
        }

        /// <summary>
        /// Whether to print all lines or only one line
        /// </summary>
        public bool OneLine
        {
            get => oneLine;
            set => oneLine = value;
        }

        /// <summary>
        /// Renders an aligned figlet text
        /// </summary>
        /// <returns>Rendered text that will be used by the renderer</returns>
        public string Render()
        {
            if (!OneLine)
                return AlignedTextColor.RenderAligned(
                    Top, Text, ForegroundColor, BackgroundColor, customColor, Settings.Alignment, LeftMargin, RightMargin);
            else
            {
                string[] sentences = ConsoleMisc.GetWrappedSentencesByWords(Text, ConsoleWrapper.WindowWidth - rightMargin - leftMargin);
                return AlignedTextColor.RenderAligned(
                    Top, sentences[0].Truncate(ConsoleWrapper.WindowWidth - 4), ForegroundColor, BackgroundColor, customColor, Settings.Alignment, LeftMargin, RightMargin);
            }
        }

        internal void UpdateInternalTop()
        {
            string[] sentences = ConsoleMisc.GetWrappedSentencesByWords(text, ConsoleWrapper.WindowWidth - rightMargin - leftMargin);

            // Install the values
            top = ConsoleWrapper.WindowHeight / 2 - sentences.Length / 2;
        }

        /// <summary>
        /// Makes a new instance of the aligned text renderer
        /// </summary>
        /// <param name="text">Text to use</param>
        /// <param name="vars">Variables to format the text with</param>
        public AlignedText(string text, params object[] vars)
        {
            // Install the values
            this.text = TextTools.FormatString(text, vars);
            UpdateInternalTop();
        }
    }
}
