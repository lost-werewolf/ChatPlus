using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Emojis
{
    public class EmojiSnippet : TextSnippet
    {
        private readonly Asset<Texture2D> emojiAsset;

        private static readonly Lazy<Effect?> GrayscaleEffect = new(() =>
        {
            try
            {
                return ModContent.Request<Effect>("ChatPlus/Assets/Shaders/Grayscale", AssetRequestMode.ImmediateLoad).Value;
            }
            catch
            {
                return null; // Graceful fallback if asset missing
            }
        });

        // 0 = off, 1 = full grayscale
        public static float GrayscaleIntensity = 1f;

        public EmojiSnippet(Asset<Texture2D> asset)
        {
            emojiAsset = asset;
        }

        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch sb, Vector2 position = default, Color color = default, float scale = 1f)
        {
            var tex = emojiAsset?.Value;
            float h = 20f * scale;
            if (tex == null)
            {
                size = new Vector2(0f, h);
                return true;
            }

            float s = h / Math.Max(tex.Width, tex.Height);
            float w = tex.Width * s;
            size = new Vector2(w, h);

            if (justCheckingString)
                return true;

            var fx = GrayscaleEffect.Value;
            if (fx != null && GrayscaleIntensity > 0f)
            {
                fx.Parameters["Intensity"]?.SetValue(MathHelper.Clamp(GrayscaleIntensity, 0f, 1f));
                fx.CurrentTechnique.Passes[0].Apply();
            }

            sb.Draw(tex, position, null, color == default ? Color.White : color, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
            return true;
        }
    }
}