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

namespace Terminaux.Base
{
    /// <summary>
    /// Terminaux continuable exception class
    /// </summary>
    public class TerminauxContinuableException : Exception
    {
        /// <inheritdoc/>
        public TerminauxContinuableException()
        { }

        /// <inheritdoc/>
        public TerminauxContinuableException(string message) :
            base(message)
        { }

        /// <inheritdoc/>
        public TerminauxContinuableException(string message, Exception innerException) :
            base(message, innerException)
        { }
    }
}
