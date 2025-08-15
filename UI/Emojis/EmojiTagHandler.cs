using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.UI.Emojis
{
    public class EmojiTagHandler : ITagHandler
    {
        private const string DefaultModName = "AdvancedChatFeatures";

        public static float EmojiScale = 1f;

        // Tiny cache so we don't request the same texture repeatedly
        private static readonly Dictionary<string, Texture2D> cache = new();

        private static string ResolvePath(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            string k = key.Trim();

            // Remove common extensions if present
            int dot = k.LastIndexOf('.');
            if (dot >= 0 && dot > k.LastIndexOf('/'))
            {
                string ext = k.Substring(dot + 1).ToLowerInvariant();
                if (ext == "png" || ext == "rawimg" || ext == "xnb")
                    k = k.Substring(0, dot);
            }

            // If key already looks fully qualified, use it verbatim
            // e.g., "AdvancedChatFeatures/Assets/Emojis/face_smile"
            if (k.Contains('/'))
            {
                // Allow "Assets/Emojis/face" too; prefix mod name if missing
                if (!k.StartsWith(DefaultModName + "/", StringComparison.OrdinalIgnoreCase) &&
                    k.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
                {
                    return DefaultModName + "/" + k;
                }
                return k;
            }

            // Short form: just the emoji name
            // -> "AdvancedChatFeatures/Assets/Emojis/{name}"
            return $"{DefaultModName}/Assets/Emojis/{k}";
        }

        private static bool TryGetTexture(string path, out Texture2D texture)
        {
            if (cache.TryGetValue(path, out texture))
                return true;

            if (!ModContent.HasAsset(path))
                return false;

            Asset<Texture2D> asset = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad);
            texture = asset.Value;
            cache[path] = texture;
            return true;
        }

        private sealed class EmojiSnippet : TextSnippet
        {
            private readonly Texture2D texture;
            private readonly float scale;

            public EmojiSnippet(Texture2D texture, float scale)
            {
                this.texture = texture;
                this.scale = scale;
                Color = Color.White;
            }

            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1f)
            {
                float finalScale = this.scale * scale;
                if (!justCheckingString && color != Color.Black)
                {
                    spriteBatch.Draw(texture, position, null, color, 0f, Vector2.Zero, finalScale, SpriteEffects.None, 0f);
                }

                size = new Vector2(texture.Width, texture.Height) * finalScale;
                return true;
            }

            public override float GetStringLength(DynamicSpriteFont font) => texture.Width * scale;
        }

        TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
        {
            string path = ResolvePath(text);
            if (path == null || !TryGetTexture(path, out var tex))
                return new TextSnippet(text); // fall back to literal text if not found

            return new EmojiSnippet(tex, EmojiScale)
            {
                DeleteWhole = true,
                Text = $"[emoji:{text}]"
            };
        }

        public static string GenerateTag(string key) => $"[emoji:{key}]";
    }
}
