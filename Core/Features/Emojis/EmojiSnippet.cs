using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Emojis
{
    public class EmojiSnippet : TextSnippet
    {
        private readonly Asset<Texture2D> emojiAsset;

        public EmojiSnippet(Asset<Texture2D> asset, string tag) : base(tag)
        {
            emojiAsset = asset;
        }

        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch sb, Vector2 pos = default, Color color = default, float scale = 1f)
        {
            var tex = emojiAsset.Value;
            if (tex == null)
            {
                size = new Vector2(0f, 20f * scale);
                return true;
            }

            float h = 20f * scale;
            float s = h / Math.Max(tex.Width, tex.Height);
            Vector2 baseSize = new Vector2(tex.Width * s, h);

            size = baseSize;
            if (justCheckingString) return true;

            float factor = 0.90f;

            float drawScale = s * factor;
            Vector2 drawSize = baseSize * factor;
            Vector2 drawPos = pos + (baseSize - drawSize) * 0.5f;

            Color drawColor = color;
            if (drawColor.Equals(default)) drawColor = Color.White;

            sb.Draw(tex, drawPos, null, drawColor, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 0f);

            return true;
        }

        public override void OnClick()
        {
            base.OnClick();
        }

    }
}
