using System;
using System.Buffers.Text;
using System.Diagnostics;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Links;

public class LinkSnippet : TextSnippet
{
    private Rectangle hoverRect;
    private bool isHovered;

    public LinkSnippet(TextSnippet src) : base(src.Text, src.Color, src.Scale) { }

    public override Color GetVisibleColor()
    {
        if (isHovered)
        {
            return new Color(72, 190, 255);
        }
        else
        {
            return new Color(32, 160, 255); 
        }
    }

    public override void OnHover()
    {
        // Called when mouse is inside hoverRect
        if (!isHovered)
        {
            isHovered = true;
        }
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size,
        SpriteBatch sb, Vector2 pos = default, Color color = default, float scale = 1f)
    {
        size = default;
        if (justCheckingString)
            return false;

        string t = Text ?? string.Empty;
        Vector2 m = FontAssets.MouseText.Value.MeasureString(t) * scale;
        hoverRect = new Rectangle((int)pos.X, (int)pos.Y, (int)m.X, (int)m.Y);

        // update hover state
        bool inside = hoverRect.Contains(Main.MouseScreen.ToPoint());
        if (inside)
        {
            OnHover();
        }
        else if (isHovered)
        {
            isHovered = false;
        }

        // underline
        int y = (int)(pos.Y + m.Y - 9);
        //sb.Draw(TextureAssets.MagicPixel.Value,new Rectangle((int)pos.X, y, (int)m.X, 1),GetVisibleColor());

        return false; // let vanilla draw text
    }
}
