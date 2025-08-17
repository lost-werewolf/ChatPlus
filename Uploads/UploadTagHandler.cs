using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Uploads
{
    public sealed class UploadTagHandler : ITagHandler
    {
        private static readonly Dictionary<string, Texture2D> Registry = new(StringComparer.OrdinalIgnoreCase);
        public static void Clear() => Registry.Clear();
        public static string GenerateTag(string key) => $"[u:{key}]";

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
                float size = 20f; // default
                                  // parse from inline options
                ParseSize(options, ref size);

                return new UploadSnippet(texture, size)
                {
                    Text = GenerateTag(key),
                    Color = Color.White
                };
            }
            else
            {
                return new TextSnippet(key);
            }
        }

        private static void ParseSize(string opt, ref float size)
        {
            if (string.IsNullOrWhiteSpace(opt)) return;

            foreach (var part in opt.Split(new[] { '|', ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var kv = part.Split('=');
                if (kv.Length == 2 && kv[0].Equals("size", StringComparison.OrdinalIgnoreCase))
                {
                    if (float.TryParse(kv[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v))
                        size = MathHelper.Clamp(v, 8f, 1500f); // clamp to reasonable bounds
                }
            }
        }
    }
}
