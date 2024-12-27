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
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text;
using Terminaux.Colors.Data;

namespace Terminaux.ColorDataGen
{
    [Generator]
    public class ColorPropGen : IIncrementalGenerator
    {
        private string colorContent = "";
        private string webSafeColorsContent = "";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Get the color data content
            var asm = typeof(ColorPropGen).Assembly;
            var stream = asm.GetManifestResourceStream($"{asm.GetName().Name}.Resources.ConsoleColorsData.json");
            using var reader = new StreamReader(stream);
            colorContent = reader.ReadToEnd();

            // Now, populate the color enumerations and data properties
            PopulateColorEnums(context);
            PopulateColorData(context);

            // Get the other color data
            var webSafeStream = asm.GetManifestResourceStream($"{asm.GetName().Name}.Resources.WebSafeColors.json");
            using var webSafeReader = new StreamReader(webSafeStream);
            webSafeColorsContent = webSafeReader.ReadToEnd();

            // Now, populate the other color properties
            PopulateWebSafeColorData(context);
        }

        private void PopulateColorData(IncrementalGeneratorInitializationContext context)
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

                namespace Terminaux.Colors.Data
                {
                    public partial class ConsoleColorData
                    {

                """;
            string footer =
                $$"""
                    }
                }
                """;
            var builder = new StringBuilder(header);

            // Read all the console color data
            var list = JsonConvert.DeserializeObject<ConsoleColorData[]>(colorContent);
            var names = list.Select((data) => data.Name).ToArray();
            if (list is null)
                return;

            // First, populate all the color data properties
            foreach (var colorData in list)
            {
                string colorHex = colorData.HexString;
                var colorRgb = colorData.RGB;
                string colorName = colorData.Name;
                int colorId = colorData.ColorId;
                string propName = names[colorId];
                builder.AppendLine(
                    $$"""
                            /// <summary>
                            /// [{{colorHex}}] Gets the console colors data for the {{propName}} color
                            /// </summary>
                            public static ConsoleColorData {{propName}} =>
                                new("{{colorHex}}", {{colorRgb.r}}, {{colorRgb.g}}, {{colorRgb.b}}, "{{colorName}}", {{colorId}});
                    """
                );
            }
            builder.AppendLine();

            // Then, populate a partial function that gets all this data
            builder.AppendLine(
                """
                        public static partial ConsoleColorData[] GetColorData()
                        {
                            return
                            [
                """
            );
            foreach (var colorData in list)
            {
                int colorId = colorData.ColorId;
                string propName = names[colorId];
                builder.AppendLine(
                    $$"""
                                    {{propName}},
                    """
                );
            }
            builder.AppendLine(
                """
                            ];
                        }
                """
            );

            // Add the source code to the compilation
            builder.AppendLine(footer);
            context.RegisterPostInitializationOutput(ctx =>
            {
                ctx.AddSource("ConsoleColorData.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            });
        }

        private void PopulateColorEnums(IncrementalGeneratorInitializationContext context)
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

                namespace Terminaux.Colors.Data
                {
                    /// <summary>
                    /// All 255 console colors
                    /// </summary>
                    public enum ConsoleColors
                    {
                
                """;
            string footer =
                $$"""
                    }
                }
                """;
            var builder = new StringBuilder(header);

            // Read all the console color data
            var list = JsonConvert.DeserializeObject<ConsoleColorData[]>(colorContent);
            if (list is null)
                return;

            // First, populate all the color data properties
            foreach (var colorData in list)
            {
                string colorHex = colorData.HexString;
                string colorName = colorData.Name;
                var (r, g, b) = colorData.RGB;
                builder.AppendLine(
                    $$"""
                            /// <summary>
                            /// [{{colorHex}}] Represents the {{colorName}} color with the RGB value of {{r}};{{g}};{{b}}
                            /// </summary>
                            {{colorName}},
                    """
                );
            }

            // Add the source code to the compilation
            builder.AppendLine(footer);
            context.RegisterPostInitializationOutput(ctx =>
            {
                ctx.AddSource("ConsoleColors.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            });
        }

        private void PopulateWebSafeColorData(IncrementalGeneratorInitializationContext context)
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

                namespace Terminaux.Colors.Data
                {
                    /// <summary>
                    /// Web safe color list as specified by W3C.
                    /// </summary>
                    public static class WebSafeColors
                    {
                        // Taken from https://github.com/jonathantneal/color-names

                """;
            string footer =
                $$"""
                    }
                }
                """;
            var builder = new StringBuilder(header);

            // Read all the console color data
            var list = JToken.Parse(webSafeColorsContent);
            if (list is null)
                return;

            // First, populate all the color data properties
            foreach (JProperty colorData in list.Cast<JProperty>())
            {
                string colorHex = colorData.Name;
                string colorName = colorData.Value.ToString();
                string propName = colorName.Replace(" ", "");
                builder.AppendLine(
                    $$"""
                            /// <summary>
                            /// [{{colorHex}}] Gets a color instance for the {{propName}} color
                            /// </summary>
                            public static Color {{propName}} =>
                                new("{{colorHex}}");
                    """
                );
            }
            builder.AppendLine();

            // Then, populate a function that gets all this data
            builder.AppendLine(
                """
                        /// <summary>
                        /// Gets the color list
                        /// </summary>
                        /// <returns>A color list</returns>
                        public static Color[] GetColorList()
                        {
                            return
                            [
                """
            );
            foreach (JProperty colorData in list.Cast<JProperty>())
            {
                string colorName = colorData.Value.ToString();
                string propName = colorName.Replace(" ", "");
                builder.AppendLine(
                    $$"""
                                    {{propName}},
                    """
                );
            }
            builder.AppendLine(
                """
                            ];
                        }
                """
            );
            builder.AppendLine();

            // Then, populate a function that gets all the names
            builder.AppendLine(
                """
                        /// <summary>
                        /// Gets the websafe color names
                        /// </summary>
                        /// <returns>A websafe color names array</returns>
                        public static string[] GetColorNames()
                        {
                            return
                            [
                """
            );
            foreach (JProperty colorData in list.Cast<JProperty>())
            {
                string colorName = colorData.Value.ToString();
                string propName = colorName.Replace(" ", "");
                builder.AppendLine(
                    $$"""
                                    "{{propName}}",
                    """
                );
            }
            builder.AppendLine(
                """
                            ];
                        }
                """
            );

            // Add the source code to the compilation
            builder.AppendLine(footer);
            context.RegisterPostInitializationOutput(ctx =>
            {
                ctx.AddSource("WebSafeColors.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            });
        }
    }
}
