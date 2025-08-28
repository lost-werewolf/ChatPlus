using System;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace ChatPlus.Core.Features.Scrollbar;

public class ChatScrollState : UIState
{
    public ChatScrollbar chatScrollbar;
    public ChatScrollList chatScrollList;

    public ChatScrollState()
    {
        // Initialize chat scroll UI elements
        chatScrollList = new ChatScrollList();
        chatScrollbar = new ChatScrollbar();
        chatScrollList.SetScrollbar(chatScrollbar);
        Append(chatScrollList);
        Append(chatScrollbar);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        //DrawHelper.DrawSlices(sb, ele: chatScrollList);
        //DrawChatMonitorBackground(sb, 2);
    }

    private static void DrawChatMonitorBackground(SpriteBatch sb, int padding = 0)
    {
        const int Thickness = 2;
        var px = TextureAssets.MagicPixel.Value;

        // base chat rect
        Rectangle r = new(82, Main.screenHeight - 247, Main.screenWidth - 300 - 7, 210);

        // expand outwards from center
        r.Inflate(padding, padding);

        Color edge = Color.Black * 0.8f;
        Color fill = Color.White * 0.04f;

        // draw center fill
        sb.Draw(px, r, fill);

        // draw edges
        sb.Draw(px, new Rectangle(r.X, r.Y, r.Width, Thickness), edge);
        sb.Draw(px, new Rectangle(r.X, r.Bottom - Thickness, r.Width, Thickness), edge);
        sb.Draw(px, new Rectangle(r.X, r.Y, Thickness, r.Height), edge);
        sb.Draw(px, new Rectangle(r.Right - Thickness, r.Y, Thickness, r.Height), edge);
    }
}
