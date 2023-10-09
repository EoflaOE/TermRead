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
using System.IO;
using System.Text;
using Terminaux.Base;
using Terminaux.Colors;
using Terminaux.Writer.ConsoleWriters;

namespace Terminaux.Writer.MiscWriters
{
    /// <summary>
    /// Ranged line handle writer
    /// </summary>
    public static class LineHandleRangedWriter
    {

        /// <summary>
        /// Prints the line of a text file with the specified line number and the column number if the specified condition is satisfied
        /// </summary>
        /// <param name="Condition">The condition to satisfy</param>
        /// <param name="Filename">Path to text file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself as the start.</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        public static void PrintLineWithHandleConditional(bool Condition, string Filename, int LineNumber, int startPos, int endPos) =>
            PrintLineWithHandleConditional(Condition, Filename, LineNumber, startPos, endPos, new Color(ConsoleColors.Gray));

        /// <summary>
        /// Prints the line of a text file with the specified line number and the column number if the specified condition is satisfied
        /// </summary>
        /// <param name="Condition">The condition to satisfy</param>
        /// <param name="Array">A string array containing the contents of the file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        public static void PrintLineWithHandleConditional(bool Condition, string[] Array, int LineNumber, int startPos, int endPos) =>
            PrintLineWithHandleConditional(Condition, Array, LineNumber, startPos, endPos, new Color(ConsoleColors.Gray));

        /// <summary>
        /// Prints the line of a text file with the specified line number and the column number if the specified condition is satisfied
        /// </summary>
        /// <param name="Condition">The condition to satisfy</param>
        /// <param name="Filename">Path to text file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        /// <param name="color">The color</param>
        public static void PrintLineWithHandleConditional(bool Condition, string Filename, int LineNumber, int startPos, int endPos, Color color)
        {
            if (Condition)
            {
                PrintLineWithHandle(Filename, LineNumber, startPos, endPos, color);
            }
        }

        /// <summary>
        /// Prints the line of a text file with the specified line number and the column number if the specified condition is satisfied
        /// </summary>
        /// <param name="Condition">The condition to satisfy</param>
        /// <param name="Array">A string array containing the contents of the file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        /// <param name="color">The color</param>
        public static void PrintLineWithHandleConditional(bool Condition, string[] Array, int LineNumber, int startPos, int endPos, Color color)
        {
            if (Condition)
            {
                PrintLineWithHandle(Array, LineNumber, startPos, endPos, color);
            }
        }

        /// <summary>
        /// Prints the line of a text file with the specified line number and the column number
        /// </summary>
        /// <param name="Filename">Path to text file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        public static void PrintLineWithHandle(string Filename, int LineNumber, int startPos, int endPos) =>
            PrintLineWithHandle(Filename, LineNumber, startPos, endPos, new Color(ConsoleColors.Gray));

        /// <summary>
        /// Prints the line of a text file with the specified line number and the column number
        /// </summary>
        /// <param name="Array">A string array containing the contents of the file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        public static void PrintLineWithHandle(string[] Array, int LineNumber, int startPos, int endPos) =>
            PrintLineWithHandle(Array, LineNumber, startPos, endPos, new Color(ConsoleColors.Gray));

        /// <summary>
        /// Prints the line of a text file with the specified line number and the column number
        /// </summary>
        /// <param name="Filename">Path to text file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        /// <param name="color">The color</param>
        public static void PrintLineWithHandle(string Filename, int LineNumber, int startPos, int endPos, Color color)
        {
            // Read the contents
            Filename = ConsoleExtensions.NeutralizePath(Filename, Environment.CurrentDirectory);
            var FileContents = File.ReadAllLines(Filename);

            // Do the job
            PrintLineWithHandle(FileContents, LineNumber, startPos, endPos, color);
        }

        /// <summary>
        /// Prints the line of a text file with the specified line number and the column number
        /// </summary>
        /// <param name="Array">A string array containing the contents of the file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        /// <param name="color">The color</param>
        public static void PrintLineWithHandle(string[] Array, int LineNumber, int startPos, int endPos, Color color) =>
            TextWriterColor.WriteColor(RenderLineWithHandle(Array, LineNumber, startPos, endPos, color), true, color);

        /// <summary>
        /// Renders the line of a text file with the specified line number and the column number if the specified condition is satisfied
        /// </summary>
        /// <param name="Condition">The condition to satisfy</param>
        /// <param name="Filename">Path to text file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        public static string RenderLineWithHandleConditional(bool Condition, string Filename, int LineNumber, int startPos, int endPos) =>
            RenderLineWithHandleConditional(Condition, Filename, LineNumber, startPos, endPos, new Color(ConsoleColors.Gray));

        /// <summary>
        /// Renders the line of a text file with the specified line number and the column number if the specified condition is satisfied
        /// </summary>
        /// <param name="Condition">The condition to satisfy</param>
        /// <param name="Array">A string array containing the contents of the file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        public static string RenderLineWithHandleConditional(bool Condition, string[] Array, int LineNumber, int startPos, int endPos) =>
            RenderLineWithHandleConditional(Condition, Array, LineNumber, startPos, endPos, new Color(ConsoleColors.Gray));

        /// <summary>
        /// Renders the line of a text file with the specified line number and the column number if the specified condition is satisfied
        /// </summary>
        /// <param name="Condition">The condition to satisfy</param>
        /// <param name="Filename">Path to text file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        /// <param name="color">The color</param>
        public static string RenderLineWithHandleConditional(bool Condition, string Filename, int LineNumber, int startPos, int endPos, Color color)
        {
            if (Condition)
                return RenderLineWithHandle(Filename, LineNumber, startPos, endPos, color);
            return "";
        }

        /// <summary>
        /// Renders the line of a text file with the specified line number and the column number if the specified condition is satisfied
        /// </summary>
        /// <param name="Condition">The condition to satisfy</param>
        /// <param name="Array">A string array containing the contents of the file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        /// <param name="color">The color</param>
        public static string RenderLineWithHandleConditional(bool Condition, string[] Array, int LineNumber, int startPos, int endPos, Color color)
        {
            if (Condition)
                return RenderLineWithHandle(Array, LineNumber, startPos, endPos, color);
            return "";
        }

        /// <summary>
        /// Renders the line of a text file with the specified line number and the column number
        /// </summary>
        /// <param name="Filename">Path to text file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        public static string RenderLineWithHandle(string Filename, int LineNumber, int startPos, int endPos) =>
            RenderLineWithHandle(Filename, LineNumber, startPos, endPos, new Color(ConsoleColors.Gray));

        /// <summary>
        /// Renders the line of a text file with the specified line number and the column number
        /// </summary>
        /// <param name="Array">A string array containing the contents of the file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        public static string RenderLineWithHandle(string[] Array, int LineNumber, int startPos, int endPos) =>
            RenderLineWithHandle(Array, LineNumber, startPos, endPos, new Color(ConsoleColors.Gray));

        /// <summary>
        /// Renders the line of a text file with the specified line number and the column number
        /// </summary>
        /// <param name="Filename">Path to text file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        /// <param name="color">The color</param>
        public static string RenderLineWithHandle(string Filename, int LineNumber, int startPos, int endPos, Color color)
        {
            // Read the contents
            Filename = ConsoleExtensions.NeutralizePath(Filename, Environment.CurrentDirectory);
            var FileContents = File.ReadAllLines(Filename);

            // Do the job
            return RenderLineWithHandle(FileContents, LineNumber, startPos, endPos, color);
        }

        /// <summary>
        /// Renders the line of a text file with the specified line number and the column number
        /// </summary>
        /// <param name="Array">A string array containing the contents of the file</param>
        /// <param name="LineNumber">Line number (not index)</param>
        /// <param name="startPos">Column number (not index). This tells the handle where to place itself</param>
        /// <param name="endPos">Column number (not index). This tells the handle where to place itself as the end. Should be bigger than the start position.</param>
        /// <param name="color">The color</param>
        public static string RenderLineWithHandle(string[] Array, int LineNumber, int startPos, int endPos, Color color)
        {
            // Get the builder
            StringBuilder builder = new();

            // Get the line index from number
            if (LineNumber <= 0)
                LineNumber = 1;
            if (LineNumber > Array.Length)
                LineNumber = Array.Length;
            int LineIndex = LineNumber - 1;

            // Get the line
            string LineContent = Array[LineIndex];

            // Now, check the column numbers
            if (startPos < 0 || startPos > LineContent.Length)
                startPos = LineContent.Length;
            if (endPos < 0 || endPos > LineContent.Length)
                endPos = LineContent.Length;

            // Check to see if the start position is smaller than the end position
            startPos.SwapIfSourceLarger(ref endPos);

            // Place the line and the column handle
            int RepeatBlanks = startPos - 1;
            int RepeatMarkers = endPos - startPos;
            if (RepeatBlanks < 0)
                RepeatBlanks = 0;
            if (RepeatMarkers < 0)
                RepeatMarkers = 0;
            builder.AppendLine($"{color.VTSequenceForeground}  | {LineContent}");
            builder.AppendLine($"{color.VTSequenceForeground}  | {new string(' ', RepeatBlanks)}^{new string('~', RepeatMarkers)}");
            return builder.ToString();
        }

    }
}
