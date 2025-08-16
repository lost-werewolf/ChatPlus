using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Emojis
{
    // Use in text as: [e:key] or [emoji:key]
    public sealed class EmojiTagHandler : ITagHandler
    {
        private class EmojiSnippet : TextSnippet
        {
            private readonly Asset<Texture2D> asset;
            private readonly float baseSize;

            public EmojiSnippet(Asset<Texture2D> asset, float basePx)
            {
                this.asset = asset;
                baseSize = basePx;
                Color = Color.White;
                DeleteWhole = true;
            }

            /// <summary>
            /// Actually draws the emoji here
            /// </summary>
            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1f)
            {
                float px = baseSize * scale;
                size = new Vector2(px, px);

                if (justCheckingString || color == Color.Black)
                    return true;

                var tex = asset?.Value;
                if (tex == null || tex.Width == 0 || tex.Height == 0)
                    return true;

                float s = px / Math.Max(tex.Width, tex.Height); // size
                spriteBatch.Draw(tex, position, null, color, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
                return true;
            }
        }

        private static readonly Dictionary<string, Asset<Texture2D>> Registry = new(StringComparer.OrdinalIgnoreCase);

        public static bool RegisterEmoji(string key, string texturePath)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(texturePath))
                return false;
            try
            {
                var asset = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.AsyncLoad);
                Registry[key] = asset;
                return true;
            }
            catch
            {
                Log.Error($"Failed to find texturePath: '{texturePath}' for key: '{key}'");
                return false;
            }
        }

        TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
        {
            var key = text?.Trim();

            if (string.IsNullOrEmpty(key) || !Registry.TryGetValue(key, out var asset))
                //return new TextSnippet($"[e:{text}]"); // fallback as plain text
                //return new TextSnippet(""); // remove input if user entered non-existant emoji
                return new TextSnippet(text); // remove input if user entered non-existant emoji

                return new EmojiSnippet(asset, basePx: 20f)
            {
                Text = $"[e:{key}]",
                Color = Color.White
            };
        }

        public static string GenerateTag(string key) => $"[e:{key}]";

        #region Helpers
        public static bool IsTypingEmojiTag(string text)
        {
            int i = text.LastIndexOf('[');
            if (i < 0)
                return false;

            string tail = text.Substring(i);
            return tail.IndexOf(']') < 0 &&
                   (tail.StartsWith("[e", StringComparison.OrdinalIgnoreCase) ||
                    tail.StartsWith("[emoji", StringComparison.OrdinalIgnoreCase));
        }
        #endregion
    }
}