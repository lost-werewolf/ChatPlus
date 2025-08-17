using System;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Colors
{
    public class ColorElement : NavigationElement<ColorItem>
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
            Microsoft.Xna.Framework.Color previewColor = HexToColor(tag);
            sb.Draw(TextureAssets.MagicPixel.Value, previewColorPos, previewColor);

            // Render color tag name
            var glyphSnippet = new[] { new TextSnippet(tag) { Color = Microsoft.Xna.Framework.Color.White, CheckForHover = false } };
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                glyphSnippet,
                pos += new Microsoft.Xna.Framework.Vector2(32, 4),
                0f,
                Vector2.Zero,
                new Vector2(1.0f),
                out _
            );
        }

        public static Microsoft.Xna.Framework.Color HexToColor(string tag)
        {
            string hex = tag[3..].Replace(":", " ").Trim();

            if (hex.Length == 6) // RRGGBB
            {
                byte r = Convert.ToByte(hex[..2], 16);
                byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                return new Microsoft.Xna.Framework.Color(r, g, b, 255);
            }
            return new Microsoft.Xna.Framework.Color(0, 0, 0) * 0f;
        }
    }
}
