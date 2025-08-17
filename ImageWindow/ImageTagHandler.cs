using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.ImageWindow
{
    // Draws [u:key] where key maps to a user-uploaded texture
    public sealed class ImageTagHandler : ITagHandler
    {
        private class UploadImageSnippet : TextSnippet
        {
            private readonly Texture2D tex;
            private readonly float size;

            public UploadImageSnippet(Texture2D texture, float size)
            {
                tex = texture;
                this.size = size;
                DeleteWhole = true;
                Color = Color.White;
            }

            // Report a tall size so the chat line grows to the image height.
            // Draw the image BELOW the text baseline so it appears on the next "row".
            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch,
                                            Vector2 position = default, Color color = default, float scale = 1f)
            {
                float px = this.size * scale;
                float line = FontAssets.MouseText.Value.LineSpacing * scale;

                // Height = one text line + image height; Width = 0 so we don't shift following text horizontally.
                size = new Vector2(0f, line + px);

                if (justCheckingString || color == Color.Black || tex == null)
                    return true;

                float s = px / Math.Max(tex.Width, tex.Height);
                // draw one full line *below* the baseline so it visually sits under the text
                var drawPos = new Vector2(position.X, position.Y + line);

                spriteBatch.Draw(tex, drawPos, null, color, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
                return true;
            }

            // Return 0 so this snippet doesn't consume horizontal space on the text line.
            public override float GetStringLength(ReLogic.Graphics.DynamicSpriteFont font) => 0f;
        }

        private static readonly Dictionary<string, Texture2D> Registry = new(StringComparer.OrdinalIgnoreCase);
        public static void Clear() => Registry.Clear();
        public static string Tag(string key) => $"[u:{key}]";

        public static bool Register(string key, Texture2D texture)
        {
            if (texture == null || string.IsNullOrWhiteSpace(key)) return false;
            Registry[key] = texture;
            return true;
        }

        TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
        {
            // default size
            float px = 128f;
            string key = text ?? string.Empty;

            // inline syntax: [u:key|size=NN] or [u:key|s=NN]
            int pipe = key.IndexOf('|');
            if (pipe >= 0)
            {
                string opt = key[(pipe + 1)..];
                key = key[..pipe];
                ParseSize(opt, ref px);
            }
            // also honor ChatManager 'options' if provided
            ParseSize(options, ref px);

            if (!Registry.TryGetValue(key, out var texture))
                return new TextSnippet($"[u:{key}]"); // fallback text

            return new UploadImageSnippet(texture, MathHelper.Clamp(px, 8f, 1500f))
            {
                Text = Tag(key),
                Color = Color.White
            };
        }

        private static void ParseSize(string opt, ref float px)
        {
            if (string.IsNullOrWhiteSpace(opt)) return;
            foreach (var part in opt.Split(new[] { '|', ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var kv = part.Split('=');
                if (kv.Length != 2) continue;
                if (kv[0].Equals("size", StringComparison.OrdinalIgnoreCase) || kv[0].Equals("s", StringComparison.OrdinalIgnoreCase))
                {
                    if (float.TryParse(kv[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                        px = v;
                }
            }
        }
    }
}
