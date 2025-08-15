using System;
using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.UI.Emojis
{
    // Use in text as: [e:key] or [emoji:key]
    public sealed class EmojiTagHandler : ITagHandler
    {
        private class EmojiSnippet : TextSnippet
        {
            private readonly Asset<Texture2D> _asset;
            private readonly float _basePx;

            public EmojiSnippet(Asset<Texture2D> asset, float basePx)
            {
                _asset = asset;
                _basePx = basePx;
                Color = Color.White;
                DeleteWhole = true;
            }

            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1f)
            {
                Main.NewText("ab");
                float px = _basePx * scale * EmojiScale;
                size = new Vector2(px, px);

                if (justCheckingString || color == Color.Black)
                    return true;

                var tex = _asset?.Value;
                if (tex == null || tex.Width == 0 || tex.Height == 0)
                    return true;

                float s = px / Math.Max(tex.Width, tex.Height);
                spriteBatch.Draw(tex, position, null, color, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
                return true;
            }

            public override float GetStringLength(DynamicSpriteFont font)
            {
                return _basePx * EmojiScale;
            }
        }

        public static float EmojiScale = 1f;

        private static readonly Dictionary<string, Asset<Texture2D>> Registry = new(StringComparer.OrdinalIgnoreCase);

        public static void ClearRegistry()
        {
            Registry.Clear();
        }

        public static bool RegisterEmoji(string key, string texturePath)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(texturePath))
                return false;

            if (!ModContent.RequestIfExists<Texture2D>(texturePath, out var asset, AssetRequestMode.ImmediateLoad))
            {
                Log.Error($"EmojiTagHandler: texture not found '{texturePath}' for key '{key}'");
                return false;
            }

            Registry[key] = asset;
            return true;
        }

        TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
        {
            // 'text' is the key after the colon in [e:key]
            var key = text?.Trim();

            Main.NewText(Registry.Count);

            if (string.IsNullOrEmpty(key) || !Registry.TryGetValue(key, out var asset))
                return new TextSnippet($":{text}"); // fallback as plain text

            // 20px “em square” baseline; scales with chat text scale
            return new EmojiSnippet(asset, basePx: 20f)
            {
                Text = $"[e:{key}]",
                Color = Color.White
            };
        }

        public static string GenerateTag(string key) => $"[e:{key}]";
    }
}