using System;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Colors
{
    public class ColorElement : BaseElement<ColorItem>
    {
        public ColorItem color;

        public ColorElement(ColorItem color) : base(color)
        {
            this.color = color;
            Height.Set(30, 0);
            Width.Set(0, 1);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            var dims = GetDimensions();
            Vector2 pos = dims.Position();
            string tag = color.Tag;

            // Render preview square
            Rectangle previewColorPos = new Rectangle(
                (int)dims.X + 2,
                (int)dims.Y + 2,
                width: 25,
                height: 25
            );
            Color previewColor = HexToColor(tag);
            sb.Draw(TextureAssets.MagicPixel.Value, previewColorPos, previewColor);

            // Render color tag name
            TextSnippet[] colorSnippet = [new TextSnippet(color.Tag.ToString())];
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, colorSnippet, pos += new Vector2(32, 4), 0f, Vector2.Zero, Vector2.One, out _);
        }

        public static Color HexToColor(string tag)
        {
            string hex = tag[3..].Replace(":", " ").Trim();

            if (hex.Length == 6) // RRGGBB
            {
                byte r = Convert.ToByte(hex[..2], 16);
                byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                return new Color(r, g, b, 255);
            }
            return new Color(0, 0, 0) * 0f;
        }
    }
}
