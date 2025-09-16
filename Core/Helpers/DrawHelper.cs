using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI;

namespace ChatPlus.Core.Helpers;
public static class DrawHelper
{
    public static void DrawFill(SpriteBatch sb, UIElement ele=null, Rectangle rect = default)
    {
        Rectangle t = new(0, 0, 0, 0);

        // Dimensions
        if (ele != null)
        {
            CalculatedStyle dims = ele.GetDimensions();
             t = new((int)dims.X + 4, (int)dims.Y + 4, (int)dims.Width - 8, (int)dims.Height - 6);
        }
        if (rect != default)
        {
            t = rect;
        }
        

        Texture2D pixel = TextureAssets.MagicPixel.Value;

        // fill (slightly brighter blue, semi-transparent)
        sb.Draw(pixel, t, new Color(70, 120, 220, 140));

        // white border
        const int b = 0;
        sb.Draw(pixel, new Rectangle(t.X, t.Y, t.Width, b), Color.White);                 // top
        sb.Draw(pixel, new Rectangle(t.X, t.Bottom - b, t.Width, b), Color.White);        // bottom
        sb.Draw(pixel, new Rectangle(t.X, t.Y, b, t.Height), Color.White);                // left
        sb.Draw(pixel, new Rectangle(t.Right - b, t.Y, b, t.Height), Color.White);        // right
    }

    public static void DrawSlices(SpriteBatch sb, UIElement ele=null, Rectangle rect = default)
    {
        Rectangle t = new(0,0,0,0);

        if (ele != null)
            t = ele.GetDimensions().ToRectangle();
        if (rect != default)
            t = rect;

        var tex = Ass.Hitbox.Value;
        int c = 5;
        Rectangle sc = new(0, 0, c, c),
                  eh = new(c, 0, 30 - 2 * c, c),
                  ev = new(0, c, c, 30 - 2 * c),
                  ce = new(c, c, 30 - 2 * c, 30 - 2 * c);

        Color color = Color.White;

        // DrawSystems fill
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
