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

            // Draw preview box
            tag = tag.Replace("[c/", "");
            tag = tag.Replace(":", "");
            Color previewColor = HexToColor(tag);
            var boxSize = 30;
            var box = new Rectangle((int)pos.X + 0, (int)pos.Y + 0, boxSize, boxSize);
            Color c = Color.Black; //box outline color
            sb.Draw(TextureAssets.MagicPixel.Value, box, previewColor);
            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(box.X, box.Y, box.Width, 1), c);
            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(box.X, box.Bottom - 1, box.Width, 1), c);
            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(box.X, box.Y, 1, box.Height), c);
            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(box.Right - 1, box.Y, 1, box.Height), c);

            // Render color tag name
            TextSnippet[] colorSnippet = [new TextSnippet(color.Tag.ToString())];
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, colorSnippet, pos += new Vector2(37, 4), 0f, Vector2.Zero, Vector2.One, out _);
        }

        private static Color HexToColor(string tag)
        {
            // Expecting format [FF00FF]
            if (tag.Length == 6)
            {
                byte r = Convert.ToByte(tag[..2], 16);
                byte g = Convert.ToByte(tag.Substring(2, 2), 16);
                byte b = Convert.ToByte(tag.Substring(4, 2), 16);
                return new Color(r, g, b, 255);
            }
            return new Color(0, 0, 0) * 0f;
        }
    }
}
