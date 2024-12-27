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
using Terminaux.Colors;
using System.Text;
using Terminaux.Writer.FancyWriters.Tools;
using Terminaux.Writer.ConsoleWriters;
using System.Diagnostics;
using Terminaux.Colors.Data;
using Textify.General;
using System.Linq;
using Terminaux.Base;
using Terminaux.Base.Checks;
using Terminaux.Base.Extensions;

namespace Terminaux.Writer.FancyWriters
{
    /// <summary>
    /// Border writer with color and text support
    /// </summary>
    public static class BorderTextColor
    {
        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        public static void WriteBorderPlain(string text) =>
            WriteBorderPlain(text, BorderSettings.GlobalSettings);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        public static void WriteBorderPlain(string text, int Left, int Top, int InteriorWidth, int InteriorHeight) =>
            WriteBorderPlain(text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="settings">Border settings to use</param>
        public static void WriteBorderPlain(string text, BorderSettings settings) =>
            WriteBorder(text, settings, new Color(ConsoleColors.Silver), ColorTools.CurrentBackgroundColor);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        public static void WriteBorderPlain(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings)
        {
            try
            {
                // StringBuilder to put out the final rendering text
                string rendered = RenderBorderPlain(text, Left, Top, InteriorWidth, InteriorHeight, settings);
                TextWriterWhereColor.WriteWhere(rendered, Left, Top);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine($"There is a serious error when printing text. {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        public static string RenderBorderPlain(string text, int Left, int Top, int InteriorWidth, int InteriorHeight) =>
            RenderBorderPlain(text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        public static string RenderBorderPlain(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings) =>
            RenderBorder("", text, Left, Top, InteriorWidth, InteriorHeight, settings, ColorTools.CurrentForegroundColor, ColorTools.CurrentBackgroundColor, ColorTools.GetGray(), false);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="title">Title to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        public static string RenderBorderPlain(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, params object[] vars) =>
            RenderBorderPlain(title, text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="title">Title to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        public static string RenderBorderPlain(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, params object[] vars) =>
            RenderBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, settings, ColorTools.CurrentForegroundColor, ColorTools.CurrentBackgroundColor, ColorTools.GetGray(), false, vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        public static void WriteBorder(string text) =>
            WriteBorder(text, BorderSettings.GlobalSettings, new Color(ConsoleColors.Silver), ColorTools.CurrentBackgroundColor, ColorTools.GetGray());

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="BorderColor">Border color</param>
        public static void WriteBorder(string text, Color BorderColor) =>
            WriteBorder(text, BorderSettings.GlobalSettings, BorderColor, ColorTools.CurrentBackgroundColor, ColorTools.GetGray());

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="BorderColor">Border color from Terminaux's <see cref="Color"/></param>
        /// <param name="BackgroundColor">Border background color from Terminaux's <see cref="Color"/></param>
        public static void WriteBorder(string text, Color BorderColor, Color BackgroundColor) =>
            WriteBorder(text, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, ColorTools.GetGray());

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="BorderColor">Border color from Terminaux's <see cref="Color"/></param>
        /// <param name="BackgroundColor">Border background color from Terminaux's <see cref="Color"/></param>
        /// <param name="TextColor">Border text color from Terminaux's <see cref="Color"/></param>
        public static void WriteBorder(string text, Color BorderColor, Color BackgroundColor, Color TextColor) =>
            WriteBorder(text, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, TextColor);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="settings">Border settings to use</param>
        public static void WriteBorder(string text, BorderSettings settings) =>
            WriteBorder(text, settings, ConsoleColors.Silver, ColorTools.CurrentBackgroundColor, ColorTools.GetGray());

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        public static void WriteBorder(string text, BorderSettings settings, Color BorderColor) =>
            WriteBorder(text, settings, BorderColor, ColorTools.CurrentBackgroundColor, ColorTools.GetGray());

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        public static void WriteBorder(string text, BorderSettings settings, Color BorderColor, Color BackgroundColor) =>
            WriteBorder(text, settings, BorderColor, BackgroundColor, ColorTools.GetGray());

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border text color</param>
        public static void WriteBorder(string text, BorderSettings settings, Color BorderColor, Color BackgroundColor, Color TextColor)
        {
            try
            {
                // StringBuilder to put out the final rendering text
                string rendered = RenderBorder(text, settings, BorderColor, BackgroundColor, TextColor);
                TextWriterRaw.WriteRaw(rendered);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine($"There is a serious error when printing text. {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        public static string RenderBorderPlain(string text) =>
            RenderBorderPlain(text, BorderSettings.GlobalSettings);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="settings">Border settings to use</param>
        public static string RenderBorderPlain(string text, BorderSettings settings) =>
            RenderBorder("", text, settings, ColorTools.CurrentForegroundColor, ColorTools.CurrentBackgroundColor, ColorTools.GetGray(), false);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="title">Title to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        public static string RenderBorderPlain(string title, string text, params object[] vars) =>
            RenderBorderPlain(title, text, BorderSettings.GlobalSettings, vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="title">Title to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="settings">Border settings to use</param>
        public static string RenderBorderPlain(string title, string text, BorderSettings settings, params object[] vars) =>
            RenderBorder(title, text, settings, ColorTools.CurrentForegroundColor, ColorTools.CurrentBackgroundColor, ColorTools.GetGray(), false, vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="BorderColor">Border color</param>
        public static string RenderBorder(string text, Color BorderColor) =>
            RenderBorder(text, BorderSettings.GlobalSettings, BorderColor, ColorTools.CurrentBackgroundColor);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        public static string RenderBorder(string text, Color BorderColor, Color BackgroundColor) =>
            RenderBorder(text, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, ColorTools.GetGray());

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border text color</param>
        public static string RenderBorder(string text, Color BorderColor, Color BackgroundColor, Color TextColor) =>
            RenderBorder(text, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, TextColor);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        public static string RenderBorder(string text, BorderSettings settings, Color BorderColor) =>
            RenderBorder("", text, settings, BorderColor, ColorTools.CurrentBackgroundColor);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        public static string RenderBorder(string text, BorderSettings settings, Color BorderColor, Color BackgroundColor) =>
            RenderBorder("", text, settings, BorderColor, BackgroundColor, ColorTools.GetGray());

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border text color</param>
        public static string RenderBorder(string text, BorderSettings settings, Color BorderColor, Color BackgroundColor, Color TextColor) =>
            RenderBorder("", text, settings, BorderColor, BackgroundColor, TextColor, true);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        public static void WriteBorder(string title, string text, params object[] vars) =>
            WriteBorder(title, text, BorderSettings.GlobalSettings, new Color(ConsoleColors.Silver), vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="BorderColor">Border color</param>
        public static void WriteBorder(string title, string text, Color BorderColor, params object[] vars) =>
            WriteBorder(title, text, BorderSettings.GlobalSettings, BorderColor, ColorTools.CurrentBackgroundColor, vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="BorderColor">Border color from Terminaux's <see cref="Color"/></param>
        /// <param name="BackgroundColor">Border background color from Terminaux's <see cref="Color"/></param>
        public static void WriteBorder(string title, string text, Color BorderColor, Color BackgroundColor, params object[] vars) =>
            WriteBorder(title, text, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, ColorTools.GetGray(), vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="BorderColor">Border color from Terminaux's <see cref="Color"/></param>
        /// <param name="BackgroundColor">Border background color from Terminaux's <see cref="Color"/></param>
        /// <param name="TextColor">Border text color from Terminaux's <see cref="Color"/></param>
        public static void WriteBorder(string title, string text, Color BorderColor, Color BackgroundColor, Color TextColor, params object[] vars) =>
            WriteBorder(title, text, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, TextColor, vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="settings">Border settings to use</param>
        public static void WriteBorder(string title, string text, BorderSettings settings, params object[] vars) =>
            WriteBorder(title, text, settings, new Color(ConsoleColors.Silver), vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        public static void WriteBorder(string title, string text, BorderSettings settings, Color BorderColor, params object[] vars) =>
            WriteBorder(title, text, settings, BorderColor, ColorTools.CurrentBackgroundColor, vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        public static void WriteBorder(string title, string text, BorderSettings settings, Color BorderColor, Color BackgroundColor, params object[] vars) =>
            WriteBorder(title, text, settings, BorderColor, BackgroundColor, ColorTools.GetGray(), vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border text color</param>
        public static void WriteBorder(string title, string text, BorderSettings settings, Color BorderColor, Color BackgroundColor, Color TextColor, params object[] vars)
        {
            try
            {
                // StringBuilder to put out the final rendering text
                string rendered = RenderBorder(title, text, settings, BorderColor, BackgroundColor, TextColor, vars);
                TextWriterRaw.WriteRaw(rendered);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine($"There is a serious error when printing text. {ex.Message}");
            }
        }

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        public static void WriteBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight) =>
            WriteBorder(text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, new Color(ConsoleColors.Silver));

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="BorderColor">Border color</param>
        public static void WriteBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, Color BorderColor) =>
            WriteBorder(text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, BorderColor, ColorTools.CurrentBackgroundColor);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="BorderColor">Border color from Terminaux's <see cref="Color"/></param>
        /// <param name="BackgroundColor">Border background color from Terminaux's <see cref="Color"/></param>
        public static void WriteBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, Color BorderColor, Color BackgroundColor) =>
            WriteBorder(text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, ColorTools.GetGray());

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="BorderColor">Border color from Terminaux's <see cref="Color"/></param>
        /// <param name="BackgroundColor">Border background color from Terminaux's <see cref="Color"/></param>
        /// <param name="TextColor">Border text color from Terminaux's <see cref="Color"/></param>
        public static void WriteBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, Color BorderColor, Color BackgroundColor, Color TextColor) =>
            WriteBorder(text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, TextColor);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        public static void WriteBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings) =>
            WriteBorder(text, Left, Top, InteriorWidth, InteriorHeight, settings, ConsoleColors.Silver);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        public static void WriteBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, Color BorderColor) =>
            WriteBorder(text, Left, Top, InteriorWidth, InteriorHeight, settings, BorderColor, ColorTools.CurrentBackgroundColor);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        public static void WriteBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, Color BorderColor, Color BackgroundColor) =>
            WriteBorder(text, Left, Top, InteriorWidth, InteriorHeight, settings, BorderColor, BackgroundColor, ColorTools.GetGray());

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border text color</param>
        public static void WriteBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, Color BorderColor, Color BackgroundColor, Color TextColor)
        {
            try
            {
                // StringBuilder to put out the final rendering text
                string rendered = RenderBorder(text, Left, Top, InteriorWidth, InteriorHeight, settings, BorderColor, BackgroundColor, TextColor);
                TextWriterRaw.WriteRaw(rendered);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine($"There is a serious error when printing text. {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="BorderColor">Border color</param>
        public static string RenderBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, Color BorderColor) =>
            RenderBorder(text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, BorderColor, ColorTools.CurrentBackgroundColor);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        public static string RenderBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, Color BorderColor, Color BackgroundColor) =>
            RenderBorder(text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, ColorTools.GetGray());

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border background color</param>
        public static string RenderBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, Color BorderColor, Color BackgroundColor, Color TextColor) =>
            RenderBorder(text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, TextColor);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        public static string RenderBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, Color BorderColor) =>
            RenderBorder("", text, Left, Top, InteriorWidth, InteriorHeight, settings, BorderColor, ColorTools.CurrentBackgroundColor, true);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        public static string RenderBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, Color BorderColor, Color BackgroundColor) =>
            RenderBorder("", text, Left, Top, InteriorWidth, InteriorHeight, settings, BorderColor, BackgroundColor, true);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border text color</param>
        public static string RenderBorder(string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, Color BorderColor, Color BackgroundColor, Color TextColor) =>
            RenderBorder("", text, Left, Top, InteriorWidth, InteriorHeight, settings, BorderColor, BackgroundColor, TextColor, true);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        public static void WriteBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, params object[] vars) =>
            WriteBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, new Color(ConsoleColors.Silver), vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="BorderColor">Border color</param>
        public static void WriteBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, Color BorderColor, params object[] vars) =>
            WriteBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, BorderColor, ColorTools.CurrentBackgroundColor, vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="BorderColor">Border color from Terminaux's <see cref="Color"/></param>
        /// <param name="BackgroundColor">Border background color from Terminaux's <see cref="Color"/></param>
        public static void WriteBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, Color BorderColor, Color BackgroundColor, params object[] vars) =>
            WriteBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, ColorTools.GetGray(), vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="BorderColor">Border color from Terminaux's <see cref="Color"/></param>
        /// <param name="BackgroundColor">Border background color from Terminaux's <see cref="Color"/></param>
        /// <param name="TextColor">Border text color from Terminaux's <see cref="Color"/></param>
        public static void WriteBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, Color BorderColor, Color BackgroundColor, Color TextColor, params object[] vars) =>
            WriteBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, TextColor, vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        public static void WriteBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, params object[] vars) =>
            WriteBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, settings, new Color(ConsoleColors.Silver), vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        public static void WriteBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, Color BorderColor, params object[] vars) =>
            WriteBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, settings, BorderColor, ColorTools.CurrentBackgroundColor, vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        public static void WriteBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, Color BorderColor, Color BackgroundColor, params object[] vars) =>
            WriteBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, settings, BorderColor, BackgroundColor, ColorTools.GetGray(), vars);

        /// <summary>
        /// Writes the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border text color</param>
        public static void WriteBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, Color BorderColor, Color BackgroundColor, Color TextColor, params object[] vars)
        {
            try
            {
                // StringBuilder to put out the final rendering text
                string rendered = RenderBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, settings, BorderColor, BackgroundColor, TextColor, vars);
                TextWriterRaw.WriteRaw(rendered);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine($"There is a serious error when printing text. {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="BorderColor">Border color</param>
        public static string RenderBorder(string title, string text, Color BorderColor, params object[] vars) =>
            RenderBorder(title, text, BorderSettings.GlobalSettings, BorderColor, ColorTools.CurrentBackgroundColor, vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        public static string RenderBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, Color BorderColor, Color BackgroundColor, params object[] vars) =>
            RenderBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border text color</param>
        public static string RenderBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, Color BorderColor, Color BackgroundColor, Color TextColor, params object[] vars) =>
            RenderBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, TextColor, vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border text color</param>
        public static string RenderBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, Color BorderColor, Color BackgroundColor, Color TextColor, params object[] vars) =>
            RenderBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, settings, BorderColor, BackgroundColor, TextColor, true, vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        public static string RenderBorder(string title, string text, BorderSettings settings, Color BorderColor, params object[] vars) =>
            RenderBorder(title, text, settings, BorderColor, ColorTools.CurrentBackgroundColor, true, vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="BorderColor">Border color</param>
        public static string RenderBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, Color BorderColor, params object[] vars) =>
            RenderBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, BorderSettings.GlobalSettings, BorderColor, ColorTools.CurrentBackgroundColor, vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        public static string RenderBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, Color BorderColor, params object[] vars) =>
            RenderBorder(title, text, Left, Top, InteriorWidth, InteriorHeight, settings, BorderColor, ColorTools.CurrentBackgroundColor, true, vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        public static string RenderBorder(string title, string text, Color BorderColor, Color BackgroundColor, params object[] vars) =>
            RenderBorder(title, text, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, ColorTools.GetGray(), vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        public static string RenderBorder(string title, string text, BorderSettings settings, Color BorderColor, Color BackgroundColor, params object[] vars) =>
            RenderBorder(title, text, settings, BorderColor, BackgroundColor, ColorTools.GetGray(), vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border text color</param>
        public static string RenderBorder(string title, string text, Color BorderColor, Color BackgroundColor, Color TextColor, params object[] vars) =>
            RenderBorder(title, text, BorderSettings.GlobalSettings, BorderColor, BackgroundColor, TextColor, vars);

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border text color</param>
        public static string RenderBorder(string title, string text, BorderSettings settings, Color BorderColor, Color BackgroundColor, Color TextColor, params object[] vars)
        {
            var splitLines = text.SplitNewLines();
            int maxWidth = splitLines.Max((str) => str.Length);
            int maxHeight = splitLines.Length;
            if (maxWidth >= ConsoleWrapper.WindowWidth)
                maxWidth = ConsoleWrapper.WindowWidth - 4;
            if (maxHeight >= ConsoleWrapper.WindowHeight)
                maxHeight = ConsoleWrapper.WindowHeight - 4;
            int borderX = ConsoleWrapper.WindowWidth / 2 - maxWidth / 2 - 1;
            int borderY = ConsoleWrapper.WindowHeight / 2 - maxHeight / 2 - 1;
            return RenderBorder(title, text,
                borderX, borderY, maxWidth, maxHeight, settings, BorderColor, BackgroundColor, TextColor, true, vars);
        }

        /// <summary>
        /// Renders the border plainly
        /// </summary>
        /// <param name="title">Title to be written.</param>
        /// <param name="text">Text to be written.</param>
        /// <param name="vars">Variables to format the message before it's written.</param>
        /// <param name="Left">Where to place the border horizontally? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="Top">Where to place the border vertically? Please note that this value comes from the upper left corner, which is an exterior position.</param>
        /// <param name="InteriorWidth">The width of the interior window, excluding the two console columns for left and right frames</param>
        /// <param name="InteriorHeight">The height of the interior window, excluding the two console columns for upper and lower frames</param>
        /// <param name="settings">Border settings to use</param>
        /// <param name="BorderColor">Border color</param>
        /// <param name="BackgroundColor">Border background color</param>
        /// <param name="TextColor">Border text color</param>
        /// <param name="useColor">Whether to use the color or not</param>
        internal static string RenderBorder(string title, string text, int Left, int Top, int InteriorWidth, int InteriorHeight, BorderSettings settings, Color BorderColor, Color BackgroundColor, Color TextColor, bool useColor, params object[] vars)
        {
            StringBuilder border = new();
            try
            {
                // StringBuilder to put out the final rendering text
                if (useColor)
                {
                    border.Append(
                        BoxFrameColor.RenderBoxFrame(title, Left, Top, InteriorWidth, InteriorHeight, settings, BorderColor, BackgroundColor, TextColor, vars) +
                        BoxColor.RenderBox(Left + 1, Top, InteriorWidth, InteriorHeight, BackgroundColor)
                    );
                }
                else
                {
                    border.Append(
                        BoxFrameColor.RenderBoxFrame(title, Left, Top, InteriorWidth, InteriorHeight, settings, vars) +
                        BoxColor.RenderBox(Left + 1, Top, InteriorWidth, InteriorHeight)
                    );
                }

                // Wrap the sentences to fit the box
                if (!string.IsNullOrWhiteSpace(text))
                {
                    // Get the current foreground color
                    if (useColor)
                    {
                        border.Append(
                            ColorTools.RenderSetConsoleColor(TextColor) +
                            ColorTools.RenderSetConsoleColor(BackgroundColor, true)
                        );
                    }

                    // Now, split the sentences and count them to fit the box
                    string[] sentences = ConsoleMisc.GetWrappedSentencesByWords(text, InteriorWidth);
                    for (int i = 0; i < sentences.Length; i++)
                    {
                        string sentence = sentences[i];
                        if (Top + 1 + i > Top + InteriorHeight)
                            break;
                        border.Append(
                            TextWriterWhereColor.RenderWhere(sentence, Left + 1, Top + 1 + i)
                        );
                    }
                }

                // Write the resulting buffer
                if (useColor)
                {
                    border.Append(
                        ColorTools.RenderRevertForeground() +
                        ColorTools.RenderRevertBackground()
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine($"There is a serious error when printing text. {ex.Message}");
            }
            return border.ToString();
        }

        static BorderTextColor()
        {
            if (!ConsoleChecker.busy)
                ConsoleChecker.CheckConsole();
        }
    }
}
