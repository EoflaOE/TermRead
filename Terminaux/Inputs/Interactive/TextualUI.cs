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
using System.Collections.Generic;
using Terminaux.Base.Buffered;
using Terminaux.Inputs.Pointer;
using Terminaux.Writer.CyclicWriters.Renderer;
using Terminaux.Writer.CyclicWriters.Renderer.Tools;

namespace Terminaux.Inputs.Interactive
{
    /// <summary>
    /// Base textual UI class for your interactive applications
    /// </summary>
    public abstract class TextualUI
    {
        internal TextualUIState state = TextualUIState.Ready;
        internal Screen uiScreen = new();
        private Guid guid = Guid.NewGuid();
        private Action<TextualUI, ConsoleKeyInfo, PointerEventContext?>? fallback;
        private bool fallbackSet = false;

        /// <summary>
        /// Unique ID for this textual UI
        /// </summary>
        public Guid Guid =>
            guid;

        /// <summary>
        /// State of this textual UI
        /// </summary>
        public TextualUIState State =>
            state;

        /// <summary>
        /// Name of the interactive textual UI
        /// </summary>
        public string Name { get; set; } = "Interactive UI";

        /// <summary>
        /// Refresh delay. If set to zero or less than zero, this means that this UI doesn't refresh itself.
        /// </summary>
        public int RefreshDelay { get; set; } = 0;

        /// <summary>
        /// List of available keybindings
        /// </summary>
        /// <remarks>
        /// You can edit this to add your custom keybindings, but it's preferrable to either place them in a constructor or
        /// in the overridden value, and to define the delegates in separate private functions inside the UI class.
        /// </remarks>
        public virtual List<(Keybinding binding, Action<TextualUI, ConsoleKeyInfo, PointerEventContext?> action)> Keybindings { get; } = [];

        /// <summary>
        /// Fallback keybinding in case defined keybinding doesn't exist. Can only be set once.
        /// </summary>
        public Action<TextualUI, ConsoleKeyInfo, PointerEventContext?>? Fallback
        {
            get => fallback;
            set
            {
                if (!fallbackSet)
                    fallback = value;
                fallbackSet = true;
            }
        }

        /// <summary>
        /// List of renderable contianers that are going to be laid out on top of what <see cref="Render()"/> prints to the console.
        /// </summary>
        public virtual List<Container> Renderables { get; } = [];

        /// <summary>
        /// Renders this interactive textual UI
        /// </summary>
        /// <returns>A string generated by this function for the <see cref="TextualUITools.RunTui(TextualUI)"/> function to render to the console</returns>
        /// <remarks>
        /// Generated sequence, which will be used for rendering, has lower priority than <see cref="Renderables"/>, so any
        /// renderable container specified in that property will overwrite what's been rendered.
        /// </remarks>
        public abstract string Render();

        /// <summary>
        /// Tells the textual UI that the refresh is required
        /// </summary>
        public void RequireRefresh() =>
            uiScreen.RequireRefresh();
    }
}
