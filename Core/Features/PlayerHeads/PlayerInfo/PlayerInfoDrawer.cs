using System;
using System.Collections.Generic;
using System.Drawing;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.PlayerHeads.PlayerInfo;

public static class PlayerInfoDrawer
{
    /// <summary>
    /// Draws a big tooltip panel with player info, pokémon style.
    /// </summary>
    public static void Draw(SpriteBatch sb, Player player)
    {
        // Dimensions
        const int panelWidth = 96;
        const int gutter = 10;
        const int side = 7;
        const int H = 356;
        const int W = panelWidth * 2 + gutter + side * 2;
        const int rowHeight = 31;
        var pos = Main.MouseScreen + new Vector2(20, 20);
        //pos = new(300, 300); // debug
        pos.X = Math.Clamp(pos.X, 0, Main.screenWidth - W);
        pos.Y = Math.Clamp(pos.Y, 0, Main.screenHeight - H);
        var rect = new Rectangle((int)pos.X, (int)pos.Y, W, H);

        // Draw background and player
        DrawFullBGPanel(sb, rect);
        DrawPlayerText(sb, rect, player);
        var cursor = pos + new Vector2(side, 32);
        DrawHorizontalSeparator(sb, cursor, W - side * 2);
        cursor += new Vector2(0, 10);
        rect = new Rectangle((int)cursor.X, (int)cursor.Y, W - side * 2, 100);
        DrawSeparatorBorder(sb, rect);
        DrawSurfaceBackground(sb, rect);
        DrawPlayer(sb, pos, player);
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            DrawTeamText(sb, cursor, player);
        }
        cursor += new Vector2(0, 110);
        DrawHorizontalSeparator(sb, cursor, W - side * 2);

        // Draw stats rows
        cursor += new Vector2(0, 10);
        int leftColumn = (int)cursor.X;
        int rightColumn = leftColumn + panelWidth + gutter;

        // Draw row 1
        DrawStat_HP(sb, new Rectangle(leftColumn, (int)cursor.Y, panelWidth, rowHeight), player);
        DrawStat_Mana(sb, new Rectangle(rightColumn, (int)cursor.Y, panelWidth, rowHeight), player);

        // Draw row 2
        int rowY2 = (int)cursor.Y + rowHeight + 6;
        DrawStat_Defense(sb, new Rectangle(leftColumn, rowY2, panelWidth, rowHeight), player);
        DrawStat_DeathCount(sb, new Rectangle(rightColumn, rowY2, panelWidth, rowHeight), player);

        // Draw row 3
        int rowY3 = (int)cursor.Y + rowHeight * 2 + 6*2;
        DrawStat_Coins(sb, new Rectangle(leftColumn, rowY3, panelWidth, rowHeight), player);
        DrawStat_Ammo(sb, new Rectangle(rightColumn, rowY3, panelWidth, rowHeight), player);

        // Draw row 4
        int rowY4 = (int)cursor.Y + rowHeight * 3 + 6 * 3;
        DrawStat_Minions(sb, new Rectangle(leftColumn, rowY4, panelWidth, rowHeight), player);
        DrawStat_Sentries(sb, new Rectangle(rightColumn, rowY4, panelWidth, rowHeight), player);

        // Draw row 5
        int rowY5 = (int)cursor.Y + rowHeight * 4 + 6 * 4;
        DrawStat_HeldItem(sb, new Rectangle(leftColumn, rowY5, panelWidth, rowHeight), player);
        DrawStat_DPS(sb, new Rectangle(rightColumn, rowY5, panelWidth, rowHeight), player);
    }


    #region Stats
    public static void DrawStat_HeldItem(SpriteBatch sb, Rectangle rect, Player player)
    {
        var tex = Ass.StatPanel; rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height()); sb.Draw(tex.Value, rect, Color.White);

        Item held = player.HeldItem;
        if (held == null || held.IsAir)
            held = new Item(ItemID.None);

        var pos = new Vector2(rect.X + 15, rect.Y + 15);
        ItemSlot.DrawItemIcon(held, 31, sb, pos, 0.8f, 32f, Color.White);

        string t = held.IsAir ? "(none)" : held.Name;
        if (t.Length > 8) t = t.Substring(0, 8) + "...";
        var tp = new Vector2(rect.X + 10, rect.Y + 4);
        if (t.Length <= 7)
        {
            tp += new Vector2(16, 0);
        }

        Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, t, tp.X, tp.Y, Color.White, Color.Black, Vector2.Zero, 1f);
    }
    public static void DrawStat_DPS(SpriteBatch sb, Rectangle rect, Player player)
    {
        var tex = Ass.StatPanel; rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height()); sb.Draw(tex.Value, rect, Color.White);

        // DPS meter uses a 60 tick rolling window in vanilla
        int dps = player.dpsDamage;
        string t = $"{dps}";

        // Icon: DPS Meter
        var pos = new Vector2(rect.X + 15, rect.Y + 15);
        ItemSlot.DrawItemIcon(new Item(ItemID.DPSMeter), 31, sb, pos, 0.8f, 32f, Color.White);

        var tp = new Vector2(rect.X + 52, rect.Y + 4);
        Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, t, tp.X, tp.Y, Color.White, Color.Black, Vector2.Zero, 1f);
    }

    public static void DrawStat_Minions(SpriteBatch sb, Rectangle rect, Player player)
    {
        var tex = Ass.StatPanel; rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height()); sb.Draw(tex.Value, rect, Color.White);
        int max = player.maxMinions; int cur = (int)Math.Round(player.slotsMinions); int bestProj = -1; float bestScore = 0f; var owned = player.ownedProjectileCounts; int limit = owned.Length;
        for (int t = 0; t < limit; t++)
        {
            int c = owned[t]; if (c <= 0) continue; var proj = ContentSamples.ProjectilesByType[t]; if (!proj.minion) continue; if (!(proj.minionSlots > 0f)) continue; float score = c * proj.minionSlots; if (score > bestScore) { bestScore = score; bestProj = t; }
        }
        Item icon = new(ItemID.SlimeStaff);
        if (bestProj >= 0)
        {
            foreach (var kv in ContentSamples.ItemsByType)
            {
                var it = kv.Value; if (it == null) continue; if (it.summon || it.DamageType == DamageClass.Summon) { if (it.shoot == bestProj) { icon = new Item(it.type); break; } }
            }
        }
        var pos = new Vector2(rect.X + 15, rect.Y + 15); ItemSlot.DrawItemIcon(icon, 31, sb, pos, 0.8f, 32f, Color.White);
        var tp = new Vector2(rect.X + 52, rect.Y + 4); Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, $"{cur}/{max}", tp.X, tp.Y, Color.White, Color.Black, Vector2.Zero, 1f);
    }

    public static void DrawStat_Sentries(SpriteBatch sb, Rectangle rect, Player player)
    {
        var tex = Ass.StatPanel; rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height()); sb.Draw(tex.Value, rect, Color.White);
        int max = player.maxTurrets; int cur = 0; int bestProj = -1; int bestCount = 0;
        for (int t = 0; t < player.ownedProjectileCounts.Length; t++)
        {
            int c = player.ownedProjectileCounts[t]; if (c <= 0) continue;
            var proj = ContentSamples.ProjectilesByType[t]; if (!proj.sentry) continue;
            cur += c;
            if (c > bestCount) { bestCount = c; bestProj = t; }
        }
        Item icon = new(ItemID.QueenSpiderStaff); // fallback
        if (bestProj >= 0)
        {
            foreach (var kv in ContentSamples.ItemsByType)
            {
                var it = kv.Value; if (it == null) continue;
                if (it.summon || it.DamageType == DamageClass.Summon) { if (it.shoot == bestProj) { icon = new Item(it.type); break; } }
            }
        }
        var pos = new Vector2(rect.X + 15, rect.Y + 15); ItemSlot.DrawItemIcon(icon, 31, sb, pos, 0.8f, 32f, Color.White);
        var tp = new Vector2(rect.X + 52, rect.Y + 4); Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, $"{cur}/{max}", tp.X, tp.Y, Color.White, Color.Black, Vector2.Zero, 1f);
    }
    public static void DrawStat_Defense(SpriteBatch sb, Rectangle rect, Player player)
    {
        // Draw background
        var tex = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Stat_Defense");
        rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height());
        sb.Draw(tex.Value, rect, Color.White);

        // Draw defense text
        var defenseText = $"{player.statDefense}";
        var snippets = ChatManager.ParseMessage(defenseText, Color.White).ToArray();
        var pos = new Vector2(rect.X + 52, rect.Y + 4);
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snippets, pos, 0f, Vector2.Zero, Vector2.One, out _);
    }

    public static void DrawStat_HP(SpriteBatch sb, Rectangle rect, Player player)
    {
        // Draw background
        var tex = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Stat_HP");
        var rect2 = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height());
        sb.Draw(tex.Value, rect2, Color.White);

        // Draw HP text
        var lifeText = $"{player.statLife}/{player.statLifeMax2}";
        var snippets = ChatManager.ParseMessage(lifeText, Color.White).ToArray();
        var pos = new Vector2(rect2.X + 32, rect2.Y + 4);
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snippets, pos, 0f, Vector2.Zero, Vector2.One, out _);
    }

    public static void DrawStat_Mana(SpriteBatch sb, Rectangle rect, Player player)
    {
        // Draw background
        var tex = Ass.StatPanel;
        rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height());
        sb.Draw(tex.Value, rect, Color.White);

        // Draw mana texture
        var manaTex = TextureAssets.Mana;
        var manaRect = new Rectangle(rect.X + 4, rect.Y + 2, manaTex.Width(), manaTex.Height());
        sb.Draw(TextureAssets.Mana.Value, manaRect, Color.White);
        
        // Draw mana text
        var manaText = $"{player.statMana}/{player.statManaMax2}";
        var size = FontAssets.MouseText.Value.MeasureString(manaText);
        var textPos = new Vector2(rect.X + rect.Width - size.X - 5, rect.Y + 5);
        Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, manaText, textPos.X, textPos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
    }

    public static void DrawStat_DeathCount(SpriteBatch sb, Rectangle rect, Player player)
    {
        // Draw background
        var tex = Ass.StatPanel;
        rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height());
        sb.Draw(tex.Value, rect, Color.White);

        // Draw plat
        var item = new Item(ItemID.Tombstone);
        var pos = new Vector2(rect.X + 15, rect.Y + 15);
        ItemSlot.DrawItemIcon(item, 31, sb, pos, 0.8f, 32f, Color.White);

        // Draw death count
        var deathCount = player.numberOfDeathsPVE.ToString();
        var size = FontAssets.MouseText.Value.MeasureString(deathCount);
        var textPos = new Vector2(rect.X + 52, rect.Y + 4);
        Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, deathCount, textPos.X, textPos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
    }
    public static void DrawStat_Coins(SpriteBatch sb, Rectangle rect, Player player)
    {
        // Draw background
        var tex = Ass.StatPanel;
        rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height());
        sb.Draw(tex.Value, rect, Color.White);

        long total = 0; 
        for (int i = 50; i <= 53; i++) { 
            var it = player.inventory[i]; 
            if (it.IsAir) continue; 
            if (it.type == ItemID.PlatinumCoin) 
                total += it.stack * 1_000_000L; 
            else if (it.type == ItemID.GoldCoin)
                total += it.stack * 10_000L; 
            else if (it.type == ItemID.SilverCoin) 
                total += it.stack * 100L; 
            else if (it.type == ItemID.CopperCoin) 
                total += it.stack; 
        }

        long rem = total; 
        int p = (int)(rem / 1_000_000L); 
        rem %= 1_000_000L; 
        int g = (int)(rem / 10_000L); 
        rem %= 10_000L; 
        int s = (int)(rem / 100L); 
        int c = (int)(rem % 100L);

        var font = FontAssets.MouseText.Value; 
        var pos = new Vector2(rect.X+6, rect.Y + 6); 
        int iconSize = 32; 
        float scale = 1.0f; 
        float dx = 28f;

        if (p > 99)
        {
            dx = 40f;
        }

        ItemSlot.DrawItemIcon(new Item(ItemID.PlatinumCoin), 31, sb, pos, scale, iconSize, Color.White); 
        Utils.DrawBorderStringFourWay(sb, font, p.ToString() ?? p.ToString() ?? p.ToString().ToString(), pos.X-5, pos.Y + 6, Color.White, Color.Black, Vector2.Zero, scale);
        pos.X += dx; 
        ItemSlot.DrawItemIcon(new Item(ItemID.GoldCoin), 31, sb, pos, scale, iconSize, Color.White); 
        Utils.DrawBorderStringFourWay(sb, font, g.ToString(), pos.X -8, pos.Y + 6, Color.White, Color.Black, Vector2.Zero, scale);
        pos.X += dx; 
        ItemSlot.DrawItemIcon(new Item(ItemID.SilverCoin), 31, sb, pos, scale, iconSize, Color.White); 
        Utils.DrawBorderStringFourWay(sb, font, s.ToString(), pos.X -8, pos.Y + 6, Color.White, Color.Black, Vector2.Zero, scale);
        pos.X += dx;

        if (p <= 99)
        {
            ItemSlot.DrawItemIcon(new Item(ItemID.CopperCoin), 31, sb, pos, scale, iconSize, Color.White);
            Utils.DrawBorderStringFourWay(sb, font, c.ToString(), pos.X - 8, pos.Y + 6, Color.White, Color.Black, Vector2.Zero, scale);
        }
    }
    public static void DrawStat_Ammo(SpriteBatch sb, Rectangle rect, Player player)
    {
        // Draw background
        var tex = Ass.StatPanel;
        rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height());
        sb.Draw(tex.Value, rect, Color.White);

        // Summarize stacks for each ammo slot and draw the one with the most
        int highestStack = 0;
        Item highestStackItem = new(ItemID.WoodenArrow);
        for (int i = 54; i <= 57; i++)
        {
            var item = player.inventory[i];
            if (item.IsAir) continue;

            if (item.stack > highestStack)
            {
                highestStack = item.stack;
                highestStackItem = item;
            }
        }

        // Draw item
        var pos = new Vector2(rect.X + 15, rect.Y + 15);
        ItemSlot.DrawItemIcon(highestStackItem, 31, sb, pos, 0.8f, 32f, Color.White);

        // Draw ammo count
        string highestAmmoStack = highestStack.ToString();
        var size = FontAssets.MouseText.Value.MeasureString(highestAmmoStack);
        var textPos = new Vector2(rect.X + 52, rect.Y + 4);
        Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, highestAmmoStack, textPos.X, textPos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
    }
    #endregion Stats

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
    public static void DrawTeamText(SpriteBatch sb, Vector2 pos, Player player)
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

    public static void DrawPlayer(SpriteBatch sb, Vector2 pos, Player player)
    {
        // Restart spritebatch to make player draw on top
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

        // Make player walk
        //int frame = (int)(Main.GlobalTimeWrappedHourly / 0.07f) % 14 + 6;
        //int y = frame * 56;
        //player.bodyFrame.Y = player.legFrame.Y = player.headFrame.Y = y;

        // Make player stand still and be boring
        player.heldProj = -1;
        player.itemAnimation = 0;
        player.itemTime = 0;
        player.PlayerFrame();
        //player.WingFrame(false);

        // Draw player
        pos += Main.screenPosition + new Vector2(100, 88);
        Main.PlayerRenderer.DrawPlayer(Main.Camera, player, pos, 0f, Vector2.Zero, scale: 1.2f);
    }

    public static void DrawSurfaceBackground(SpriteBatch sb, Rectangle rect)
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
    public static void DrawSeparatorBorder(SpriteBatch sb, Rectangle rect, int edgeWidth = 2)
    {
        var tex = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Separator1").Value;
        Color color = new Color(89, 116, 213, 255) * 0.9f;

        DrawPanel(tex, edgeWidth, 0, sb, new Vector2(rect.X, rect.Y), rect.Width, color);
        DrawPanel(tex, edgeWidth, 0, sb, new Vector2(rect.X, rect.Bottom - edgeWidth), rect.Width, color);
        DrawPanelVertical(tex, edgeWidth, 0, sb, new Vector2(rect.X, rect.Y), rect.Height, color);
        DrawPanelVertical(tex, edgeWidth, 0, sb, new Vector2(rect.Right - edgeWidth, rect.Y), rect.Height, color);
    }

    public static void DrawPanel(Texture2D texture, int edgeWidth, int edgeShove, SpriteBatch spriteBatch, Vector2 position, float width, Color color)
    {
        spriteBatch.Draw(texture, 
            new Vector2(position.X + edgeWidth, position.Y), new Rectangle(edgeWidth + edgeShove, 0, texture.Width - (edgeWidth + edgeShove) * 2, texture.Height),
            color,
            0f,
            Vector2.Zero,
            new Vector2((width - (edgeWidth * 2)) / (float)(texture.Width - (edgeWidth + edgeShove) * 2), 1f),
            SpriteEffects.None,
            0f);
    }

    public static void DrawPanelVertical(Texture2D texture,int edgeWidth,int edgeShove,SpriteBatch spriteBatch,Vector2 position, float height,Color color)
    {
        spriteBatch.Draw(texture,new Rectangle((int)position.X, (int)position.Y + edgeWidth, edgeWidth, (int)height - edgeWidth * 2),
            new Rectangle(texture.Width / 2, 0, 1, texture.Height),color);
    }

    public static void DrawFullBGPanel(SpriteBatch sb, Rectangle rect)
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

        DrawNineSlice(sb, BG.Value, rect, new Color(63, 82, 151) * 1.0f);
        DrawNineSlice(sb, Border.Value, rect, Color.Black);
    }

    public static void DrawHorizontalSeparator(SpriteBatch sb, Vector2 pos, float width, int edgeWidth = 2)
    {
        var tex = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Separator1").Value;
        Color color = new Color(89, 116, 213, 255) * 0.9f;
        DrawPanel(tex, edgeWidth, 0, sb, pos, width, color);
    }
    #endregion
    private static void DrawDebugRect(Rectangle r) => Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, r, Color.Red * 0.5f);
}
