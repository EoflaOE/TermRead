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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using Terminaux.NerdFontGen.Decoy;

namespace Terminaux.NerdFontGen
{
    [Generator]
    public class NerdFontListGen : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Get the color data content
            var asm = typeof(NerdFontListGen).Assembly;
            var stream = asm.GetManifestResourceStream($"{asm.GetName().Name}.Resources.NerdFonts.json");
            using var reader = new StreamReader(stream);
            string content = reader.ReadToEnd();

            // Read all the Nerd Fonts data
            var list = JsonConvert.DeserializeObject<NerdFontInfo[]>(content);
            if (list is null)
                return;
            NerdFontsEnumGenerator(list, context);
            NerdFontsClassGenerator(list, context);
            NerdFontsDictionaryGenerator(list, context);
        }

        private void NerdFontsEnumGenerator(NerdFontInfo[] list, IncrementalGeneratorInitializationContext context)
        {
            string header =
                $$"""
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
                
                // <auto-generated/>
                                
                namespace Terminaux.Writer.CyclicWriters.Renderer.Tools
                {
                    /// <summary>
                    /// Nerd Fonts types
                    /// </summary>
                    public enum NerdFontsTypes
                    {
                
                """;
            string footer =
                $$"""
                    }
                }
                """;
            var builder = new StringBuilder(header);

            // Populate the types
            for (int typeIdx = 0; typeIdx < list.Length; typeIdx++)
            {
                NerdFontInfo typeInfo = list[typeIdx];
                string type = typeInfo.Type;

                // Build the enum values
                builder.AppendLine(
                    $$"""
                            /// <summary>
                            /// {{type}} - from Nerd Fonts
                            /// </summary>
                            {{type}},
                    """
                );
            }

            // Add the footer
            builder.AppendLine(footer);
            context.RegisterPostInitializationOutput(ctx =>
            {
                ctx.AddSource("NerdFontsTypes.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            });
        }

        private void NerdFontsClassGenerator(NerdFontInfo[] list, IncrementalGeneratorInitializationContext context)
        {
            string header =
                $$"""
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
                
                // <auto-generated/>
                                
                using System.Text.RegularExpressions;
                using Terminaux.Base;

                namespace Terminaux.Writer.CyclicWriters.Renderer.Tools
                {

                """;
            string footer =
                $$"""
                    }
                }
                """;

            // Populate the icon class for every type
            foreach (var typeInfo in list)
            {
                var builder = new StringBuilder(header);
                string name = typeInfo.Type;
                var icons = typeInfo.Icons;

                // Add the class declaration
                builder.AppendLine(
                    $$"""
                        /// <summary>
                        /// List of {{name}} Nerd Fonts icons ({{icons.Length}} icons installed)
                        /// </summary>
                        public static class {{name}}Icons
                        {
                    """
                );

                // Populate the character listing (public constants)
                for (int i = 0; i < icons.Length; i++)
                {
                    Icon icon = icons[i];
                    string iconName = icon.Name;
                    string iconFriendlyName = iconName.ToUpper()[0] + iconName.Substring(1).Replace("-", "_");
                    string iconUnicode = icon.Unicode;
                    builder.AppendLine(
                        $$"""
                                /// <summary>
                                /// Character constant of {{iconName}} Nerd Fonts icon containing unicode character U+{{iconUnicode}} in the {{name}} category ({{i + 1}} of {{icons.Length}})
                                /// </summary>
                                public const string {{iconFriendlyName}} =
                                    {{(iconUnicode.Length == 5 ? $"\"\\U000{iconUnicode}\"" : $"\"\\u{iconUnicode}\"")}};
                        """
                    );
                }

                // New line
                builder.AppendLine();

                // Add the footer
                builder.AppendLine(footer);
                context.RegisterPostInitializationOutput(ctx =>
                {
                    ctx.AddSource($"{name}Icons.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
                });
            }
        }

        private void NerdFontsDictionaryGenerator(NerdFontInfo[] list, IncrementalGeneratorInitializationContext context)
        {
            string header =
                $$"""
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
                
                // <auto-generated/>
                
                using System.Collections.Generic;

                namespace Terminaux.Writer.CyclicWriters.Renderer.Tools
                {
                    /// <summary>
                    /// Nerd Fonts tools
                    /// </summary>
                    public static partial class NerdFontsTools
                    {
                        private static readonly Dictionary<NerdFontsTypes, Dictionary<string, string>> nerdFontChars = new()
                        {
                
                """;
            string footer =
                $$"""
                        };
                    }
                }
                """;
            var builder = new StringBuilder(header);

            // Populate the icon dictionary for every type
            for (int typeIdx = 0; typeIdx < list.Length; typeIdx++)
            {
                NerdFontInfo typeInfo = list[typeIdx];
                string typeName = typeInfo.Type;
                var icons = typeInfo.Icons;
                builder.AppendLine(
                    $$"""
                                // {{typeName}} icons
                                { NerdFontsTypes.{{typeName}},
                                    new Dictionary<string, string>()
                                    {
                    """
                );

                // Populate the icon entries
                for (int iconIdx = 0; iconIdx < icons.Length; iconIdx++)
                {
                    Icon icon = icons[iconIdx];
                    string iconName = icon.Name;
                    string iconFriendlyName = iconName.ToUpper()[0] + iconName.Substring(1).Replace("-", "_");

                    // Build the dictionary entry
                    builder.AppendLine(
                        $$"""
                                            { "{{iconName}}", {{typeName}}Icons.{{iconFriendlyName}} },
                        """
                    );
                    if (iconIdx == icons.Length - 1 && typeIdx < list.Length - 1)
                        builder.AppendLine();
                }
                builder.AppendLine(
                    $$"""
                                    }
                                },
                    """
                );
            }

            // Add the footer
            builder.AppendLine(footer);
            context.RegisterPostInitializationOutput(ctx =>
            {
                ctx.AddSource("NerdFontsTools.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            });
        }
    }
}
