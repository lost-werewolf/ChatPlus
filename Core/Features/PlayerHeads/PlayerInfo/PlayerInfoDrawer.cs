using System;
using System.Drawing;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.PlayerHeads.PlayerInfo;

public static class PlayerInfoDrawer
{
    /// <summary>
    /// Draws a big tooltip panel with player info, bestiary-style.
    /// // Extra to add maybe later:
    /// red team, death count since he joined 
    /// ammo, coins, 
    /// armor/accessories/equipment/dyes
    /// </summary>
    public static void Draw(SpriteBatch sb, Player player)
    {
        // Dimensions
        const int W = 96*2+6*2+12;
        const int H = 350;
        Vector2 pos = Main.MouseScreen + new Vector2(20, 20);
        pos = new(300, 300); // debug
        pos.X = Math.Clamp(pos.X, 0, Main.screenWidth - W);
        pos.Y = Math.Clamp(pos.Y, 0, Main.screenHeight - H);
        Rectangle rect = new((int)pos.X, (int)pos.Y, W, H);

        // Draw header and player
        DrawPlayer(sb, pos, player);
        DrawFullBGPanel(sb,rect);
        DrawPlayerText(sb, rect, player);
        pos += new Vector2(7, 32);
        DrawHorizontalSeparator(sb, pos, W-14);
        pos += new Vector2(0, 10);
        rect = new((int)pos.X, (int)pos.Y, W-14, 100);
        DrawSeparatorBorder(sb, rect);
        DrawSurfaceBackground(sb, rect);
        DrawTeamText(sb, pos, player);
        pos += new Vector2(0, 110);
        DrawHorizontalSeparator(sb, pos, W - 14);
        pos += new Vector2(0, 10);
        rect = new((int)pos.X, (int)pos.Y, W - 14, 21);

        // Draw stats
        //DrawDebugRect(rect);
        DrawStat_HP(sb, rect, player);
        DrawStat_Mana(sb, rect, player);
        DrawStat_Defense(sb, rect, player);
        DrawStat_DeathCount(sb, rect, player);
    }

    #region Stats
    private static void DrawStat_Defense(SpriteBatch sb, Rectangle rect, Player player)
    {
        // Draw background
        var tex = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Stat_Defense");
        rect = new(rect.X, rect.Y + tex.Height() + 4, tex.Width(), tex.Height());
        sb.Draw(tex.Value, rect, Color.White);

        string defenseText = $"{player.statDefense}";
        var snippets = ChatManager.ParseMessage(defenseText, Color.White).ToArray();
        Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, defenseText, Vector2.One);
        float textWidth = textSize.X;
        Vector2 pos = new(rect.X + 52, rect.Y + 4);
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snippets, pos, 0f, Vector2.Zero, Vector2.One, out _);
    }
    private static void DrawStat_HP(SpriteBatch sb, Rectangle rect, Player player)
    {
        var tex = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Stat_HP");
        Rectangle rect2 = new((int)rect.X, rect.Y, (int)tex.Width(), (int)tex.Height());
        sb.Draw(tex.Value, rect2, Color.White);

        string lifeText = $"{player.statLife}/{player.statLifeMax2}";
        var snippets = ChatManager.ParseMessage(lifeText, Color.White).ToArray();
        Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, lifeText, Vector2.One);
        float textWidth = textSize.X;
        Vector2 pos = new(rect.X + 32, rect.Y + 4);
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snippets, pos, 0f, Vector2.Zero, Vector2.One, out _);
    }
    private static void DrawStat_Mana(SpriteBatch sb, Rectangle rect, Player player)
    {
        // Draw background
        var tex = Ass.StatPanel;
        rect = new(rect.X + rect.X / 2 - 54, rect.Y, tex.Width(), tex.Height());
        sb.Draw(tex.Value, rect, Color.White);

        // Draw mana texture
        var manaTex = TextureAssets.Mana;
        Rectangle manaRect = new(rect.X + 4, rect.Y + 2, manaTex.Width(), manaTex.Height());
        sb.Draw(TextureAssets.Mana.Value, manaRect, Color.White);

        // Draw mana text
        string manaText = $"{player.statMana}/{player.statManaMax2}";
        var size = FontAssets.MouseText.Value.MeasureString(manaText);
        Vector2 textPos = new Vector2(rect.X + rect.Width - size.X - 5, rect.Y + 5);
        Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, manaText, textPos.X, textPos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
    }
    private static void DrawStat_DeathCount(SpriteBatch sb, Rectangle rect, Player player)
    {
        // Draw background
        var tex = Ass.StatPanel;
        rect = new(rect.X + rect.X / 2 - 54, rect.Y + tex.Height() + 4, tex.Width(), tex.Height());
        sb.Draw(tex.Value, rect, Color.White);

        // Draw item
        Item item = new(321);
        var manaTex = TextureAssets.Mana;
        Vector2 pos = new(rect.X + 15, rect.Y + 15);
        ItemSlot.DrawItemIcon(item, 31, sb, pos, 0.8f, 32f, Color.White);

        // Draw mana
        string deathCount = player.numberOfDeathsPVE.ToString();
        var size = FontAssets.MouseText.Value.MeasureString(deathCount);
        Vector2 textPos = new Vector2(rect.X + 32, rect.Y + 4);
        Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, deathCount, textPos.X, textPos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
    }
    #endregion

    #region Helpers
    private static void DrawPlayerText(SpriteBatch sb, Rectangle rect, Player player)
    {
        string playerText = $"Player: {player.name}";

        var snippets = ChatManager.ParseMessage(playerText, Color.White).ToArray();
        Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, playerText, Vector2.One);
        float textWidth = textSize.X;
        Vector2 pos = new(rect.X + (rect.Width - textWidth) / 2f, rect.Y + 6);
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snippets, pos, 0f, Vector2.Zero, Vector2.One, out _);
    }
    private static void DrawTeamText(SpriteBatch sb, Vector2 pos, Player player)
    {
        string teamText = player.team switch
        {
            0 => "[c/ffffff:No team]",
            1 => "[c/DA3B3B:(Team\nRed)]",
            2 => "[c/3bda55:(Team\nGreen)]",
            3 => "[c/3b95da:(Team\nBlue)]",
            4 => "[c/f2dd64:(Team\nYellow)]",
            5 => "[c/e064f2:(Team\nPink)]",
            _ => "(Unknown)"
        };

        var snippets = ChatManager.ParseMessage(teamText, Color.White).ToArray();
        pos += new Vector2(7, 5);
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snippets, pos, 0f, Vector2.Zero, new Vector2(0.7f), out _);
    }

    private static void DrawPlayer(SpriteBatch sb, Vector2 pos, Player player)
    {
        Vector2 uiCenter = pos + new Vector2(90, 200);

        using (new Main.CurrentPlayerOverride(player))
        {
            player.bodyFrame.Y = player.legFrame.Y = player.headFrame.Y = 0;
            player.heldProj = -1;
            player.itemAnimation = 0;
            player.itemTime = 0;
            player.mount?.Dismount(player);
            player.PlayerFrame();
            Vector2 worldDrawPos = uiCenter + Main.screenPosition;

            Main.PlayerRenderer.DrawPlayer(Main.Camera, player, worldDrawPos, 0f, Vector2.Zero);
        }
    }

    private static void DrawSurfaceBackground(SpriteBatch sb, Rectangle rect)
    {
        var tex = Main.Assets.Request<Texture2D>("Images/MapBG1").Value;
        int deflate = 4;
        rect = new Rectangle(
            rect.X + deflate,
            rect.Y + deflate,
            rect.Width - deflate * 2,
            rect.Height - deflate * 2
        );
        sb.Draw(tex, rect, Color.White);
    }
    #endregion

    #region Panels & Separators
    private static void DrawSeparatorBorder(SpriteBatch sb, Rectangle rect, int edgeWidth = 2)
    {
        var tex = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Separator1").Value;
        Color color = new Color(89, 116, 213, 255) * 0.9f;

        DrawPanel(tex, edgeWidth, 0, sb, new Vector2(rect.X, rect.Y), rect.Width, color);
        DrawPanel(tex, edgeWidth, 0, sb, new Vector2(rect.X, rect.Bottom - edgeWidth), rect.Width, color);
        DrawPanelVertical(tex, edgeWidth, 0, sb, new Vector2(rect.X, rect.Y), rect.Height, color);
        DrawPanelVertical(tex, edgeWidth, 0, sb, new Vector2(rect.Right - edgeWidth, rect.Y), rect.Height, color);
    }

    private static void DrawPanel(Texture2D texture, int edgeWidth, int edgeShove, SpriteBatch spriteBatch, Vector2 position, float width, Color color)
    {
        spriteBatch.Draw(
            texture, new Vector2(position.X + edgeWidth, position.Y), new Rectangle(edgeWidth + edgeShove, 0, texture.Width - (edgeWidth + edgeShove) * 2, texture.Height),
            color,
            0f,
            Vector2.Zero,
            new Vector2((width - (edgeWidth * 2)) / (float)(texture.Width - (edgeWidth + edgeShove) * 2), 1f),
            SpriteEffects.None,
            0f);
    }

    private static void DrawPanelVertical(Texture2D texture,int edgeWidth,int edgeShove,SpriteBatch spriteBatch,Vector2 position, float height,Color color)
    {
        spriteBatch.Draw(texture,new Rectangle((int)position.X, (int)position.Y + edgeWidth, edgeWidth, (int)height - edgeWidth * 2),
            new Rectangle(texture.Width / 2, 0, 1, texture.Height),color);
    }

    private static void DrawFullBGPanel(SpriteBatch sb, Rectangle rect)
    {
        var BG = Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");
        var Border = Main.Assets.Request<Texture2D>("Images/UI/PanelBorder");

        const int corner = 12;
        const int bar = 4;

        static void DrawNineSlice(SpriteBatch s, Texture2D tex, Rectangle dst, Color col)
        {
            int w = Math.Max(dst.Width, corner * 2 + 1);
            int h = Math.Max(dst.Height, corner * 2 + 1);
            int x0 = dst.X, y0 = dst.Y;
            int x1 = x0 + corner, y1 = y0 + corner;
            int x2 = x0 + w - corner, y2 = y0 + h - corner;
            s.Draw(tex, new Rectangle(x0, y0, corner, corner), new Rectangle(0, 0, corner, corner), col);
            s.Draw(tex, new Rectangle(x2, y0, corner, corner), new Rectangle(corner + bar, 0, corner, corner), col);
            s.Draw(tex, new Rectangle(x0, y2, corner, corner), new Rectangle(0, corner + bar, corner, corner), col);
            s.Draw(tex, new Rectangle(x2, y2, corner, corner), new Rectangle(corner + bar, corner + bar, corner, corner), col);
            s.Draw(tex, new Rectangle(x1, y0, w - corner * 2, corner), new Rectangle(corner, 0, bar, corner), col);
            s.Draw(tex, new Rectangle(x1, y2, w - corner * 2, corner), new Rectangle(corner, corner + bar, bar, corner), col);
            s.Draw(tex, new Rectangle(x0, y1, corner, h - corner * 2), new Rectangle(0, corner, corner, bar), col);
            s.Draw(tex, new Rectangle(x2, y1, corner, h - corner * 2), new Rectangle(corner + bar, corner, corner, bar), col);
            s.Draw(tex, new Rectangle(x1, y1, w - corner * 2, h - corner * 2), new Rectangle(corner, corner, bar, bar), col);
        }

        DrawNineSlice(sb, BG.Value, rect, new Color(63, 82, 151) * 0.70f);
        DrawNineSlice(sb, Border.Value, rect, Color.Black);
    }

    private static void DrawHorizontalSeparator(SpriteBatch sb, Vector2 pos, float width, int edgeWidth = 2)
    {
        var tex = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Separator1").Value;
        Color color = new Color(89, 116, 213, 255) * 0.9f;
        DrawPanel(tex, edgeWidth, 0, sb, pos, width, color);
    }
    #endregion

    private static void DrawDebugRect(Rectangle r) => Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, r, Color.Red * 0.5f);
}
