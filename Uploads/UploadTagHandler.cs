using System;
using System.Collections.Generic;
using System.Globalization;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;             
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Uploads
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
            $"{GetPrefixTag}:{key}|size={size.ToString("0.##", CultureInfo.InvariantCulture)}]";

        public static bool Register(string key, Texture2D texture)
        {
            if (texture == null || string.IsNullOrWhiteSpace(key))
                return false;

            Registry[key] = texture;
            return true;
        }

        TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
        {
            // text may include inline options (e.g., "cat size=64" or "cat|96").
            // options parameter may also carry them (depending on the chat parser).
            string key = text ?? string.Empty;
            float size = 20f;
            bool sizeSpecified = false;

            ParseInlineKeyAndOptions(ref key, ref size, ref sizeSpecified);
            ParseOptionsString(options, ref size, ref sizeSpecified);

            if (Registry.TryGetValue(key, out var texture))
            {
                // Preserve the size option in the snippet text so it round-trips.
                string renderedTag = sizeSpecified ? GenerateTag(key, size) : GenerateTag(key);

                return new UploadSnippet(texture, size)
                {
                    Text = renderedTag,
                    Color = Color.White // don�t tint the image
                };
            }

            // Fallback: show raw key if not registered
            return new TextSnippet(key);
        }

        // Accepts "cat size=64", "cat|size=64", "cat,96", "cat|96" 
        private static void ParseInlineKeyAndOptions(ref string key, ref float size, ref bool sizeSpecified)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            var parts = key.Split(['|', ',', ';', ' '], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) { key = string.Empty; return; }

            key = parts[0];

            for (int i = 1; i < parts.Length; i++)
                TryParseSizeToken(parts[i], ref size, ref sizeSpecified);
        }

        // Accepts options string like "size=64" (from the parser, if provided)
        private static void ParseOptionsString(string options, ref float size, ref bool sizeSpecified)
        {
            if (string.IsNullOrWhiteSpace(options)) return;

            foreach (var token in options.Split(['|', ',', ';', ' '], StringSplitOptions.RemoveEmptyEntries))
                TryParseSizeToken(token, ref size, ref sizeSpecified);
        }

        private static void TryParseSizeToken(string token, ref float size, ref bool sizeSpecified)
        {
            if (string.IsNullOrWhiteSpace(token)) return;

            // Allow "size=96" or just "96"
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
                size = MathHelper.Clamp(v, 8f, 1500f);
                sizeSpecified = true;
            }
        }
    }
}
