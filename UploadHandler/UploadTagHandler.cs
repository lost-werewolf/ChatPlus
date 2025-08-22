using System;
using System.Collections.Generic;
using System.Globalization;
using ChatPlus.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI.Chat;

namespace ChatPlus.UploadHandler
{
    public sealed class UploadTagHandler : ITagHandler
    {
        private static readonly Dictionary<string, Texture2D> Registry =
            new(StringComparer.OrdinalIgnoreCase);

        public static bool Unregister(string key) => Registry.Remove(key);
        public static bool IsRegistered(string key) => Registry.ContainsKey(key);
        public static bool TryGet(string key, out Texture2D tex) => Registry.TryGetValue(key, out tex);
        public static void Clear() => Registry.Clear();
        public static string GetPrefixTag() => "[u";
        public static string GenerateTag(string key) => $"[u:{key}]";
        public static string GenerateTag(string key, float size) =>
            $"{GetPrefixTag()}:{key}|size={size.ToString("0.##", CultureInfo.InvariantCulture)}]";

        public static bool Register(string key, Texture2D texture)
        {
            if (texture == null || string.IsNullOrWhiteSpace(key))
                return false;

            Registry[key] = texture;
            return true;
        }

        TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
        {
            string key = text ?? string.Empty;

            if (Registry.TryGetValue(key, out var texture))
            {
                return new UploadSnippet(texture, 232) // one chat height + 10 lines = 32*20+10 = 32+200=232
                {
                    Text = GenerateTag(key)
                };
            }

            return new TextSnippet(key);
        }

        // Accepts "cat size=64", "cat|size=64", "cat,96", "cat|96", etc
        private static void ParseSizeParameter(ref string key, ref float size, ref bool sizeSpecified)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            var parts = key.Split(['|', ',', ';', ' '], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) { key = string.Empty; return; }

            key = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                // Try parse size token
                string token = parts[i];
                if (string.IsNullOrWhiteSpace(token)) return;

                string val = token;
                int eq = token.IndexOf('=');
                if (eq >= 0)
                {
                    var name = token[..eq];
                    if (!name.Equals("size", StringComparison.OrdinalIgnoreCase)) return;
                    val = token[(eq + 1)..];
                }

                if (float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                {
                    float max = 100f;
                    size = MathHelper.Clamp(v, 8f, max);
                    sizeSpecified = true;
                }
            }
        }
    }
}
