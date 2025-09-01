using System;
using ChatPlus.Core.Features.PlayerColors;
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
            Color previewColor = PlayerColorHandler.HexToColor(tag);
            sb.Draw(TextureAssets.MagicPixel.Value, previewColorPos, previewColor);

            // Render color tag name
            TextSnippet[] colorSnippet = [new TextSnippet(color.Tag.ToString())];
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, colorSnippet, pos += new Vector2(32, 4), 0f, Vector2.Zero, Vector2.One, out _);
        }
    }
}
