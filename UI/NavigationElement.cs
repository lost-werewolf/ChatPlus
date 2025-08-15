using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI;

namespace AdvancedChatFeatures.UI
{
    /// <summary>
    /// An element that can be navigated in a <see cref="NavigationPanel"/>.
    /// </summary>
    public class NavigationElement : UIElement
    {
        // Variables
        private bool isSelected;
        public bool SetSelected(bool value) => isSelected = value;

        public NavigationElement()
        {
            Height.Set(30, 0);
            Width.Set(0, 1);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            SetSelected(true);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (isSelected)
            {
                DrawSlices(sb, this);
                DrawFill(sb, this);
            }

            base.Draw(sb);
        }
        private static void DrawFill(SpriteBatch sb, UIElement ele)
        {
            CalculatedStyle dims = ele.GetDimensions();
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Rectangle r = new((int)dims.X + 4, (int)dims.Y + 4, (int)dims.Width - 8, (int)dims.Height - 6);

            // fill (slightly brighter blue, semi-transparent)
            sb.Draw(pixel, r, new Color(70, 120, 220, 140));

            // white border
            const int b = 0;
            sb.Draw(pixel, new Rectangle(r.X, r.Y, r.Width, b), Color.White);                 // top
            sb.Draw(pixel, new Rectangle(r.X, r.Bottom - b, r.Width, b), Color.White);        // bottom
            sb.Draw(pixel, new Rectangle(r.X, r.Y, b, r.Height), Color.White);                // left
            sb.Draw(pixel, new Rectangle(r.Right - b, r.Y, b, r.Height), Color.White);        // right
        }

        private static void DrawSlices(SpriteBatch sb, UIElement ele, bool fill = true, float fillOpacity = 0.3f)
        {
            Rectangle t = ele.GetDimensions().ToRectangle();

            var tex = Ass.Hitbox.Value;
            int c = 5;
            Rectangle sc = new(0, 0, c, c),
                      eh = new(c, 0, 30 - 2 * c, c),
                      ev = new(0, c, c, 30 - 2 * c),
                      ce = new(c, c, 30 - 2 * c, 30 - 2 * c);

            Color color = Color.White;

            if (fill)
                sb.Draw(tex, new Rectangle(t.X + c, t.Y + c, t.Width - 2 * c, t.Height - 2 * c), ce, color * 0.3f);

            sb.Draw(tex, new Rectangle(t.X + c, t.Y, t.Width - 2 * c, c), eh, color);                                       // top
            sb.Draw(tex, new Rectangle(t.X + c, t.Bottom - c, t.Width - 2 * c, c), eh, color, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0); // bottom
            sb.Draw(tex, new Rectangle(t.X, t.Y + c, c, t.Height - 2 * c), ev, color);                                       // left
            sb.Draw(tex, new Rectangle(t.Right - c, t.Y + c, c, t.Height - 2 * c), ev, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0); // right

            sb.Draw(tex, new Rectangle(t.X, t.Y, c, c), sc, color);                                                          // TL
            sb.Draw(tex, new Rectangle(t.Right - c, t.Y, c, c), sc, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0); // TR
            sb.Draw(tex, new Rectangle(t.Right - c, t.Bottom - c, c, c), sc, color, 0, Vector2.Zero, SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0); // BR
            sb.Draw(tex, new Rectangle(t.X, t.Bottom - c, c, c), sc, color, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0); // BL
        }
    }
}
