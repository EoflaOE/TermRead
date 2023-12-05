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

using SpecProbe.Software.Kernel;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Terminaux.Base
{
    /// <summary>
    /// Console platform class
    /// </summary>
    public static class ConsolePlatform
    {
        /// <summary>
        /// New line constant
        /// </summary>
        public static string NewLine { get => Environment.NewLine; }

        /// <summary>
        /// Polls $TERM_PROGRAM to get terminal emulator
        /// </summary>
        public static string GetTerminalEmulator() =>
            Environment.GetEnvironmentVariable("TERM_PROGRAM") ?? "";

        /// <summary>
        /// Polls $TERM to get terminal type (vt100, dumb, ...)
        /// </summary>
        public static string GetTerminalType() =>
            Environment.GetEnvironmentVariable("TERM") ?? "";

        /// <summary>
        /// Is Terminaux running from GRILO?
        /// </summary>
        public static bool IsRunningFromGrilo() =>
            (Assembly.GetEntryAssembly()?.GetName()?.Name?.StartsWith("GRILO")) ?? false;

        /// <summary>
        /// Is Terminaux running from TMUX?
        /// </summary>
        public static bool IsRunningFromTmux() =>
            Environment.GetEnvironmentVariable("TMUX") is not null;

        /// <summary>
        /// Is Terminaux running from GNU Screen?
        /// </summary>
        public static bool IsRunningFromScreen() =>
            Environment.GetEnvironmentVariable("STY") is not null;

        /// <summary>
        /// Is Terminaux running from Mono?
        /// </summary>
        public static bool IsRunningFromMono() =>
            Type.GetType("Mono.Runtime") is not null;

        /// <summary>
        /// Is this system a Windows system?
        /// </summary>
        public static bool IsOnWindows() =>
            Environment.OSVersion.Platform == PlatformID.Win32NT;

        /// <summary>
        /// Is this system a Unix system? True for macOS, too!
        /// </summary>
        public static bool IsOnUnix() =>
            Environment.OSVersion.Platform == PlatformID.Unix;

        /// <summary>
        /// Is this system a Unix system that contains musl libc?
        /// </summary>
        /// <returns>True if running on Unix systems that use musl libc. Otherwise, false.</returns>
        public static bool IsOnUnixMusl()
        {
            try
            {
                if (!IsOnUnix() || IsOnMacOS() || IsOnWindows())
                    return false;
                var gnuRel = gnuGetLibcVersion();
                return false;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Is this system a macOS system?
        /// </summary>
        public static bool IsOnMacOS()
        {
            if (IsOnUnix())
            {
                string System = UnameManager.GetUname(UnameTypes.KernelName);
                return System.Contains("Darwin");
            }
            else
                return false;
        }

        #region Interop
        [DllImport("libc", EntryPoint = "gnu_get_libc_version")]
        private static extern IntPtr gnuGetLibcVersion();
        #endregion
    }
}
