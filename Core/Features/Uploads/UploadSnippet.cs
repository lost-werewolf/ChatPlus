// UploadSnippet.cs
using System;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Uploads
{
    public class UploadSnippet : TextSnippet
    {
        private readonly Texture2D tex;

        public UploadSnippet(Texture2D texture) => tex = texture;

        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch sb, Vector2 pos = default, Color color = default, float scale = 1f)
        {
            float targetH = 147f * scale;
            if (tex == null || tex.Height <= 0) { size = new Vector2(0f, targetH); return true; }

            float s = targetH / tex.Height;
            float w = tex.Width * s;

            size = new Vector2(w, targetH);
            if (justCheckingString) return true;

            sb.Draw(tex, pos, null, Color, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);

            // Hover
            Rectangle bounds = new((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
            bool hovering = bounds.Contains(Main.MouseScreen.ToPoint());
            if (Conf.C != null && Conf.C.ShowUploadPreviewWhenHovering && hovering)
            {
                if (StateManager.IsAnyStateActive()) return true;

                s *= 2f;
                //sb.Draw(tex, pos, null, Color, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
                // compute preview size
                float previewScale = s *= 1.15f;
                float previewW = tex.Width * previewScale;
                float previewH = tex.Height * previewScale;

                // start at mouse offset
                pos = Main.MouseScreen + new Vector2(20, 20);

                // clamp so the whole image fits
                pos.X = Math.Clamp(pos.X, 0, Main.screenWidth - previewW);
                pos.Y = Math.Clamp(pos.Y, 0, Main.screenHeight - previewH);

                // draw
                sb.Draw(tex, pos, null, Color.White, 0f, Vector2.Zero, previewScale, SpriteEffects.None, 0f);
            }

            // debug
            //sb.Draw(TextureAssets.MagicPixel.Value, bounds, Color.Red*0.5f);

            return true;
        }
    }
}
