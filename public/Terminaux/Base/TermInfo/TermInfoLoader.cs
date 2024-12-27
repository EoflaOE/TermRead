//
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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Terminaux.Base.TermInfo.Extensions;
using Terminaux.Base.TermInfo.Parsing;
using Textify.General;

namespace Terminaux.Base.TermInfo
{
    internal static class TermInfoLoader
    {
        public static TermInfoDesc? Load(string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = Environment.GetEnvironmentVariable("TERM");
                if (string.IsNullOrEmpty(name))
                    return null;
            }

            var directories = new List<string>();

            // TERMINFO
            directories.AddIfNotNullOrEmpty(Environment.GetEnvironmentVariable("TERMINFO"));

            // ~/.terminfo
            var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!string.IsNullOrWhiteSpace(profile))
            {
                directories.Add(Path.Combine(profile, ".terminfo"));
            }

            // TERMINFO_DIRS
            var dirs = Environment.GetEnvironmentVariable("TERMINFO_DIRS");
            if (!string.IsNullOrWhiteSpace(dirs))
            {
                var separator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':';
                foreach (var path in dirs.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries))
                {
                    directories.Add(path);
                }
            }

            // Fallback
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                directories.Add("/etc/terminfo");
                directories.Add("/lib/terminfo");
                directories.Add("/usr/share/terminfo");
            }

            // Check all directories
            foreach (var directory in directories)
            {
                var desc = Load(directory, name);
                if (desc != null)
                    return desc;
            }

            // Last resort (builtins tracked from Debian's NCurses: https://packages.debian.org/sid/all/ncurses-base/download
            // and https://packages.debian.org/sid/all/ncurses-term/download)
            var builtins = TermInfoDesc.GetBuiltinPaths();
            foreach (string builtin in builtins)
            {
                string termNamePath = builtin.RemovePrefix("Terminaux.Resources.TermFiles.");
                string termName = termNamePath.Substring(termNamePath.IndexOf('.') + 1);
                if (termName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return Load(typeof(TermInfoDesc).Assembly.GetManifestResourceStream(builtin));
            }

            // We're totally screwed
            return null;
        }

        public static TermInfoDesc Load(Stream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            return TermInfoParser.Parse(stream);
        }

        private static TermInfoDesc? Load(string path, string name)
        {
            var files = new List<string>
            {
                Path.Combine(path, name[0].ToString(), name),
                Path.Combine(path, Convert.ToByte(name[0]).ToString("x"), name),
            };

            foreach (var file in files)
            {
                if (File.Exists(file))
                    return Load(File.OpenRead(file));
            }

            return null;
        }
    }
}
