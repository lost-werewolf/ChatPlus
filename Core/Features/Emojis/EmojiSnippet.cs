﻿using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Emojis
{
    public class EmojiSnippet : TextSnippet
    {
        private readonly Asset<Texture2D> emojiAsset;
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

            if (justCheckingString) return true;

            // Draw emoji
            sb.Draw(tex, position, null, color, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);

            return true;
        }
    }
}