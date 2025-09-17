using System;
using ChatPlus.Core.Systems;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;

namespace ChatPlus.Core.Features.UnderlineText;
public class UnderlineTagHandler : ITagHandler
{
    private class UnderlineSnippet : TextSnippet
    {
        public UnderlineSnippet(string text) : base(text) { }

        public override bool UniqueDraw(bool justCheckingString, out Vector2 size,
            SpriteBatch sb, Vector2 pos = default, Color color = default, float scale = 1f)
        {
            DynamicSpriteFont boldFont = FontSystem.Bold;

            if (justCheckingString)
            {
                size = boldFont.MeasureString(Text) * scale;
                return true;
            }

            sb.DrawString(boldFont, Text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            size = boldFont.MeasureString(Text) * scale;

            // draw a single 1px underline for this visible segment
            var font = FontAssets.MouseText.Value;
            Vector2 p = new((float)Math.Floor(pos.X), (float)Math.Floor(pos.Y));
            int width = (int)System.Math.Ceiling(size.X);
            int lineHeight = (int)System.Math.Ceiling(font.LineSpacing * scale);
            int underlineY = (int)Math.Floor(p.Y + lineHeight - 10f);
            var underlineRect = new Rectangle((int)p.X, underlineY, width, 2);
            sb.Draw(TextureAssets.MagicPixel.Value, underlineRect, GetVisibleColor());



            return true;
        }
    }

    public static string GenerateTag(string text) => $"[underline:{text}]";

    public TextSnippet Parse(string text, Color baseColor = default, string options = null)
    {
        return new UnderlineSnippet(text) { Color = baseColor };
    }
}

