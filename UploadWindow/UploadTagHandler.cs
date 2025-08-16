using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.UploadWindow
{
    // Draws [u:key] where key maps to a user-uploaded texture path
    public sealed class UploadTagHandler : ITagHandler
    {
        private class UploadImageSnippet : TextSnippet
        {
            private readonly Texture2D _texture;
            private readonly float _basePx;

            public UploadImageSnippet(Texture2D texture, float basePx)
            {
                _texture = texture;
                _basePx = basePx;
                DeleteWhole = true;
                Color = Color.White;
            }

            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1f)
            {
                float px = _basePx * scale;
                size = new Vector2(px, px);
                if (justCheckingString || color == Color.Black)
                    return true;
                var tex = _texture;
                if (tex == null) return true;
                float s = px / Math.Max(tex.Width, tex.Height);
                spriteBatch.Draw(tex, position, null, color, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
                return true;
            }

            public override float GetStringLength(ReLogic.Graphics.DynamicSpriteFont font) => _basePx;
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
            if (!Registry.TryGetValue(text, out var texture))
                return new TextSnippet($"[u:{text}]");
            return new UploadImageSnippet(texture, 20f) { Text = Tag(text), Color = Color.White };
        }
    }
}
