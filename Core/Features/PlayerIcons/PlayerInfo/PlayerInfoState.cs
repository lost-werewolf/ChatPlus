using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ChatPlus.Core.Features.PlayerIcons.PlayerInfo;

public class PlayerInfoState : UIState, ILoadable
{
    public static PlayerInfoState instance;

    private UITextPanel<string> titlePanel;
    private UIElement _messageBox;
    private static Type _messageBoxType;

    // bottom
    private UIElement _bottom;
    private UITextPanel<string> _backBtn;
    private UITextPanel<string> _prevBtn;
    private UITextPanel<string> _nextBtn;
    private int _currentPlayerIndex;
    private int _whoAmI;
    private string _playerName = "Unknown";
    private ChatSession.Snapshot? _returnSnapshot;

    public void Load(Mod mod)
    {
        instance = this;
        _messageBoxType = typeof(UICommon).Assembly.GetType("Terraria.ModLoader.UI.UIMessageBox");
    }

    public void Unload() { instance = null; }
    public override void OnInitialize()
    {
        var uiContainer = new UIElement { Width = { Percent = 0.8f }, MaxWidth = new StyleDimension(1000f, 0f), Top = { Pixels = 120f }, Height = { Pixels = -120f, Percent = 1f }, HAlign = 0.5f };
        Append(uiContainer);

        var panel = new UIPanel { Width = { Percent = 1f }, Height = { Pixels = -110f, Percent = 1f }, BackgroundColor = UICommon.MainPanelBackground };
        uiContainer.Append(panel);

        var body = new UIPanel { Width = { Pixels = -25f, Percent = 1f }, Height = { Percent = 1f }, BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
        panel.Append(body);

        if (_messageBoxType != null)
        {
            _messageBox = (UIElement)Activator.CreateInstance(_messageBoxType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { "" }, null);
            _messageBox.Width.Set(0, 1f); _messageBox.Height.Set(0, 1f); body.Append(_messageBox);
        }

        titlePanel = new UITextPanel<string>(Loc.Get("PlayerInfo.Headers.Player", _playerName), 0.8f, true)
        { HAlign = 0.5f, Top = { Pixels = -35f }, BackgroundColor = UICommon.DefaultUIBlue }.WithPadding(15f);
        uiContainer.Append(titlePanel);

        _bottom = new UIElement { Width = { Percent = 1f }, Height = { Pixels = 40f }, VAlign = 1f, Top = { Pixels = -60f } };
        _bottom.PaddingLeft = 0f;
        _bottom.PaddingRight = 0f;
        uiContainer.Append(_bottom);

        _backBtn = new UITextPanel<string>("Back") { Height = { Pixels = 40f } }.WithFadedMouseOver();
        _backBtn.OnLeftClick += Back_OnLeftClick;
        _bottom.Append(_backBtn);

        _currentPlayerIndex = _whoAmI >= 0 ? _whoAmI : Main.myPlayer;
        SetPlayerFromIndex(_currentPlayerIndex);
    }

    #region Cycle players
    public static bool Active { get; private set; }
    public override void OnActivate() => Active = true;
    public override void OnDeactivate() => Active = false;
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        RefreshNavButtons();
    }

    private void RefreshNavButtons()
    {
        const float NAV_W = 36f;

        var active = GetActivePlayerIndices();
        bool multi = Main.netMode != NetmodeID.SinglePlayer && active.Length > 1;

        if (multi)
        {
            if (_prevBtn == null)
            {
                _prevBtn = new UITextPanel<string>("<", 1.0f, false) { Width = { Pixels = NAV_W }, Height = { Pixels = 40f } }.WithFadedMouseOver();
                _prevBtn.Left.Set(0f, 0f);
                _prevBtn.OnLeftClick += (_, __) => CyclePlayer(-1);
            }
            if (_nextBtn == null)
            {
                _nextBtn = new UITextPanel<string>(">", 1.0f, false) { Width = { Pixels = NAV_W }, Height = { Pixels = 40f } }.WithFadedMouseOver();
                _nextBtn.HAlign = 1f;                 // make it stick to the right edge
                _nextBtn.Left.Set(0f, 0f);            // no extra offset
                _nextBtn.OnLeftClick += (_, __) => CyclePlayer(+1);
            }
        }

        if (!multi)
        {
            if (_prevBtn?.Parent != null) _prevBtn.Remove();
            if (_nextBtn?.Parent != null) _nextBtn.Remove();
            _backBtn.Left.Set(0f, 0f);
            _backBtn.Width.Set(0f, 1f);

            _backBtn.Recalculate();
            _bottom.Recalculate();
            Recalculate();
            return;
        }

        int pos = Array.IndexOf(active, _currentPlayerIndex);
        if (pos < 0) pos = 0;

        bool hasPrev = pos > 0;
        bool hasNext = pos < active.Length - 1;

        if (hasPrev)
        {
            if (_prevBtn.Parent == null) _bottom.Append(_prevBtn);
        }
        else if (_prevBtn?.Parent != null) _prevBtn.Remove();

        if (hasNext)
        {
            if (_nextBtn.Parent == null) _bottom.Append(_nextBtn);
        }
        else if (_nextBtn?.Parent != null) _nextBtn.Remove();

        float leftW = hasPrev ? NAV_W : 0f;
        float rightW = hasNext ? NAV_W : 0f;

        _backBtn.Left.Set(leftW, 0f);
        _backBtn.Width.Set(-(leftW + rightW), 1f);

        _prevBtn?.Recalculate();
        _nextBtn?.Recalculate();
        _backBtn.Recalculate();
        _bottom.Recalculate();
        Recalculate();
    }
    private void CyclePlayer(int dir)
    {
        var list = GetActivePlayerIndices();
        if (list.Length == 0) return;

        int pos = Array.IndexOf(list, _currentPlayerIndex);
        if (pos < 0) pos = 0;

        if (dir < 0 && pos > 0) SetPlayerFromIndex(list[pos - 1]);
        else if (dir > 0 && pos < list.Length - 1) SetPlayerFromIndex(list[pos + 1]);
    }
    private void SetPlayerFromIndex(int idx)
    {
        if (idx < 0 || idx >= Main.maxPlayers) return;
        var p = Main.player[idx];
        if (p == null || !p.active) return;

        _currentPlayerIndex = idx;
        _whoAmI = idx;

        _playerName = p.name;
        titlePanel?.SetText($"Player: {_playerName}");
    }

    private static int[] GetActivePlayerIndices()
    {
        var list = new List<int>(Main.maxPlayers);
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            var p = Main.player[i];
            if (p != null && p.active) list.Add(i);
        }
        return list.ToArray();
    }

    public void SetReturnSnapshot(ChatSession.Snapshot snap) => _returnSnapshot = snap;

    public void SetPlayer(int whoAmI, string nameOverride = null)
    {
        _whoAmI = whoAmI;
        _playerName = nameOverride ?? Main.player?[whoAmI]?.name ?? $"Player {whoAmI}";
    }
    #endregion

    #region Draw
    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        var player = _whoAmI >= 0 && _whoAmI < Main.maxPlayers ? Main.player[_whoAmI] : null;
        if (player == null || !player.active) return;

        int panelWidth = 96, gutter = 10, rowHeight = 31;
        int containerW = Math.Min((int)(Main.screenWidth * 0.8f), 1000);
        int containerLeft = (Main.screenWidth - containerW) / 2, top = 120;
        int bgW = 436, bgH = 200;
        var bgRect = new Rectangle(containerLeft + 42, top + 42, bgW, bgH);
        int leftColumn = containerLeft + containerW - (panelWidth * 2 + gutter) - 60;
        int rightColumn = leftColumn + panelWidth + gutter;
        int y0 = bgRect.Y;
        bool smallWidth = Main.screenWidth < 1200;
        if (Main.screenWidth < 940) bgRect.Width = bgW - 110;

        int panelTop = top;
        int panelBottom = Main.screenHeight - 110;

        int invSlot = smallWidth ? 36 : 40;
        int invPad = 4;
        int invRowH = invSlot + invPad;
        int invRows = 5;
        int invHeight = invRows * invRowH;

        int buffSize = 32, buffPad = 6, buffPerRow = 10;
        int buffCount = CountActiveBuffs(player);
        int buffRows = (buffCount + buffPerRow - 1) / buffPerRow;
        int buffRowH = buffSize + buffPad;
        int buffHeight = buffRows * buffRowH;

        int accSlot = smallWidth ? 36 : 40;
        int accPad = 4;
        int accRowH = accSlot + accPad;
        int dyeRows = Math.Max(0, player.dye.Length - 3);
        int equipRows = Math.Max(0, player.armor.Length - 3);
        int vanityRows = Math.Max(0, player.armor.Length - 13);
        int accRows = Math.Min(dyeRows, Math.Min(equipRows, vanityRows));
        int accHeight = accRows * accRowH;

        var playerPos = new Vector2(bgRect.X + bgRect.Width * 0.5f - 100f, bgRect.Y + 50);

        Vector2 invStart = new(bgRect.X, bgRect.Bottom + 52);
        Vector2 buffStart = new(bgRect.X, invStart.Y + invHeight + 35);

        int xOffset = 0, yOffset = 0;
        if (smallWidth) { xOffset = 275; yOffset = 220; }

        Vector2 accPos = new(
            x: leftColumn - 200 + xOffset,
            y: y0 + 30 + yOffset
        );

        PlayerInfoDrawer.DrawSeparatorBorder(sb, bgRect);
        PlayerInfoDrawer.DrawMapFullscreenBackground(sb, bgRect);

        // Draw stats to the left of player
        Vector2 bgTopLeft = new(bgRect.X, bgRect.Y);
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            PlayerInfoDrawer.DrawPlayerIDText(sb, bgTopLeft, player);
            bgTopLeft += new Vector2(0, 21);
            PlayerInfoDrawer.DrawTeamText(sb, bgTopLeft, player);
        }

        // Draw player
        PlayerInfoDrawer.DrawPlayer(sb, playerPos, player, 1.8f);

        Vector2 statsHeaderPos = new(leftColumn, y0 - 4);
        y0 -= 8;
        if (!smallWidth) Utils.DrawBorderStringBig(sb, Loc.Get("PlayerInfo.Headers.Stats"), statsHeaderPos, Color.White, scale: 0.5f);
        if (smallWidth) y0 -= 30;

        Rectangle hpBounds = new(leftColumn, y0 + rowHeight + 6, panelWidth, rowHeight);
        Rectangle manaBounds = new(rightColumn, y0 + rowHeight + 6, panelWidth, rowHeight);
        Rectangle defBounds = new(leftColumn, y0 + rowHeight * 2 + 12, panelWidth, rowHeight);
        Rectangle deathBounds = new(rightColumn, y0 + rowHeight * 2 + 12, panelWidth, rowHeight);
        Rectangle coinBounds = new(leftColumn, y0 + rowHeight * 3 + 18, panelWidth, rowHeight);
        Rectangle ammoBounds = new(rightColumn, y0 + rowHeight * 3 + 18, panelWidth, rowHeight);
        Rectangle minionBounds = new(leftColumn, y0 + rowHeight * 4 + 24, panelWidth, rowHeight);
        Rectangle sentryBounds = new(rightColumn, y0 + rowHeight * 4 + 24, panelWidth, rowHeight);
        Rectangle timeInSessionBounds = new(leftColumn, y0 + rowHeight * 5 + 30, panelWidth, rowHeight);
        Rectangle daysInSessionBounds = new(rightColumn, y0 + rowHeight * 5 + 30, panelWidth, rowHeight);
        Rectangle lastEnemyBounds = new(leftColumn, y0 + rowHeight * 6 + 36, panelWidth, rowHeight);
        Rectangle lastBossBounds = new(rightColumn, y0 + rowHeight * 6 + 36, panelWidth, rowHeight);

        PlayerInfoDrawer.DrawStat_HP(sb, hpBounds, player);
        PlayerInfoDrawer.DrawStat_Mana(sb, manaBounds, player);
        PlayerInfoDrawer.DrawStat_Defense(sb, defBounds, player);
        PlayerInfoDrawer.DrawStat_Attack(sb, deathBounds, player);
        PlayerInfoDrawer.DrawStat_Coins(sb, coinBounds, player);
        PlayerInfoDrawer.DrawStat_Ammo(sb, ammoBounds, player);
        PlayerInfoDrawer.DrawStat_Minions(sb, minionBounds, player);
        PlayerInfoDrawer.DrawStat_Sentries(sb, sentryBounds, player);
        PlayerInfoDrawer.DrawStat_TimeInSession(sb, timeInSessionBounds, player);
        PlayerInfoDrawer.DrawStat_DaysInSession(sb, daysInSessionBounds, player);
        PlayerInfoDrawer.DrawStat_LastEnemyHit(sb, lastEnemyBounds, player);
        PlayerInfoDrawer.DrawStat_LastBossHit(sb, lastBossBounds, player);

        Rectangle viewport = new(containerLeft + 20, top + 20, containerW - 20 * 4, panelBottom - top - 20 * 2);
        //DrawDebugRect(viewport);
        Utils.DrawBorderStringBig(sb, Loc.Get("PlayerInfo.Headers.Inventory"), new Vector2(invStart.X + 2, invStart.Y - 35), Color.White, 0.5f);
        DrawInventory(sb, invStart, player, viewport);
        DrawAccessories(sb, accPos, player, viewport);

        if (buffCount > 0 && viewport.Bottom > buffStart.Y + 20)
        {
            Utils.DrawBorderStringBig(sb, Loc.Get("PlayerInfo.Headers.Buffs"), new Vector2(buffStart.X, buffStart.Y - 36), Color.White, 0.52f);
            DrawBuffs(sb, buffStart, player, viewport);
        }

        Point p = Main.MouseScreen.ToPoint();
        if (hpBounds.Contains(p)) UICommon.TooltipMouseText(Loc.Get("PlayerInfo.Stats.Health"));
        if (manaBounds.Contains(p)) UICommon.TooltipMouseText(Loc.Get("PlayerInfo.Stats.Mana"));
        if (defBounds.Contains(p)) UICommon.TooltipMouseText(Loc.Get("PlayerInfo.Stats.Defense"));
        if (deathBounds.Contains(p)) UICommon.TooltipMouseText(Loc.Get("PlayerInfo.Stats.Attack"));
        if (coinBounds.Contains(p)) UICommon.TooltipMouseText(Loc.Get("PlayerInfo.Stats.Coins"));
        if (ammoBounds.Contains(p)) UICommon.TooltipMouseText(Loc.Get("PlayerInfo.Stats.Ammo"));
        if (minionBounds.Contains(p)) UICommon.TooltipMouseText(Loc.Get("PlayerInfo.Stats.Minions"));
        if (sentryBounds.Contains(p)) UICommon.TooltipMouseText(Loc.Get("PlayerInfo.Stats.Sentries"));
        if (timeInSessionBounds.Contains(p)) UICommon.TooltipMouseText(Loc.Get("PlayerInfo.Stats.TimeInSession"));
        if (daysInSessionBounds.Contains(p)) UICommon.TooltipMouseText(Loc.Get("PlayerInfo.Stats.DaysInSession"));
        if (lastEnemyBounds.Contains(p)) UICommon.TooltipMouseText(Loc.Get("PlayerInfo.Stats.LastEnemyHit"));
        if (lastBossBounds.Contains(p)) UICommon.TooltipMouseText(Loc.Get("PlayerInfo.Stats.LastBossHit"));
    }

    private static void DrawInventory(SpriteBatch sb, Vector2 start, Player player, Rectangle viewport)
    {
        int size = Main.screenWidth < 1200 ? 36 : 40;
        int pad = 4;

        for (int i = 0; i < 50; i++)
        {
            int row = i / 10, col = i % 10;

            var r = new Rectangle(
                (int)(start.X + col * (size + pad)),
                (int)(start.Y + row * (size + pad)),
                size, size
            );

            if (r.Bottom > viewport.Bottom - 10) continue;

            var item = player.inventory[i];

            if (item == player.HeldItem)
                sb.Draw(TextureAssets.InventoryBack14.Value, r, Color.White);
            else
                sb.Draw(TextureAssets.InventoryBack.Value, r, Color.White);

            if (!item.IsAir)
            {
                var center = new Vector2(r.X + r.Width / 2f, r.Y + r.Height / 2f);
                ItemSlot.DrawItemIcon(item, 31, sb, center, 0.9f, size - 6, Color.White);

                if (r.Contains(Main.MouseScreen.ToPoint()))
                {
                    UICommon.TooltipMouseText("");
                    Main.LocalPlayer.mouseInterface = true;
                    Main.HoverItem = item.Clone();
                    Main.hoverItemName = item.Name;

                    if (Main.mouseRight && Main.mouseRightRelease && !PlayerInput.IgnoreMouseInterface)
                    {
                        bool TryEquipFromInventory(Player p, int invIndex)
                        {
                            if (player != Main.LocalPlayer) return false;
                            ref Item it = ref p.inventory[invIndex]; if (it.IsAir) return false;

                            if (it.accessory)
                            {
                                int begin = 3;
                                int end = Math.Min(10, p.armor.Length);
                                int empty = -1;
                                for (int s = begin; s < end; s++)
                                {
                                    if (p.armor[s].type == it.type) return false;
                                    if (empty < 0 && p.armor[s].IsAir) empty = s;
                                }
                                if (empty >= 0) { p.armor[empty] = it.Clone(); it.TurnToAir(); return true; }
                                for (int s = begin; s < end; s++)
                                {
                                    if (!p.armor[s].IsAir)
                                    {
                                        var tmp = p.armor[s];
                                        p.armor[s] = it;
                                        p.inventory[invIndex] = tmp;
                                        return true;
                                    }
                                }
                                return false;
                            }

                            int equip = -1, vanity = -1;
                            if (it.headSlot >= 0) { equip = 0; vanity = 10; }
                            else if (it.bodySlot >= 0) { equip = 1; vanity = 11; }
                            else if (it.legSlot >= 0) { equip = 2; vanity = 12; }

                            if (equip >= 0)
                            {
                                int target = it.defense > 0 ? equip : vanity;
                                if (p.armor[target].IsAir) { p.armor[target] = it.Clone(); it.TurnToAir(); return true; }
                                var sw = p.armor[target]; p.armor[target] = it; p.inventory[invIndex] = sw; return true;
                            }
                            return false;
                        }

                        if (TryEquipFromInventory(player, i))
                            Main.mouseRightRelease = false;
                    }
                }
            }

            if (i < 10)
            {
                string label = i == 9 ? "0" : (i + 1).ToString();
                Vector2 numberPos = new(r.Right - 35f, r.Bottom - 38f);
                Utils.DrawBorderString(sb, label, numberPos, Color.White, 0.75f, 0f, 0f);
            }
        }
    }

    private static void DrawLoaderSlot(Item[] items, int context, int slot, int x, int y, Player player)
    {
        float old = Main.inventoryScale;
        Main.inventoryScale = 40f / TextureAssets.InventoryBack.Width();
        bool smallWidth = Main.screenWidth < 1200;
        if (smallWidth) Main.inventoryScale = 36f / TextureAssets.InventoryBack.Width();

        int w = (int)(TextureAssets.InventoryBack.Width() * Main.inventoryScale);
        int h = (int)(TextureAssets.InventoryBack.Height() * Main.inventoryScale);
        bool hover = new Rectangle(x, y, w, h).Contains(Main.MouseScreen.ToPoint()) && !PlayerInput.IgnoreMouseInterface;

        ref Item it = ref items[slot];

        if (!it.IsAir && hover)
        {
            UICommon.TooltipMouseText("");
            Main.LocalPlayer.mouseInterface = true;
            ItemSlot.OverrideHover(items, context, slot);

            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                Main.mouseLeftRelease = false;
            }
            if (Main.mouseRight && Main.mouseRightRelease && player == Main.LocalPlayer)
            {
                ItemSlot.RightClick(items, context, slot);
                Main.mouseRightRelease = false;
            }

            ItemSlot.MouseHover(items, context, slot);
        }

        ItemSlot.Draw(Main.spriteBatch, ref it, context, new Vector2(x, y));

        Main.inventoryScale = old;
    }

    private static void DrawAccessories(SpriteBatch sb, Vector2 topLeft, Player player, Rectangle viewport)
    {
        int size = Main.screenWidth < 1200 ? 36 : 40;
        int pad = 4;
        int rowStep = size + pad;

        Utils.DrawBorderStringBig(sb, "Accessories", new Vector2(topLeft.X, topLeft.Y - 36), Color.White, 0.52f);

        for (int r = 0; r < 3; r++)
        {
            int y = (int)topLeft.Y + r * rowStep;
            var rowRect = new Rectangle((int)topLeft.X, y, 3 * rowStep, rowStep);

            int x0 = (int)topLeft.X + 0 * rowStep;
            int x1 = (int)topLeft.X + 1 * rowStep;
            int x2 = (int)topLeft.X + 2 * rowStep;

            // Draw top 3 armor dye slots
            DrawLoaderSlot(player.dye, ItemSlot.Context.EquipDye, r, x0, y, player);

            // Draw middle 3 armor vanity slots (no visual)
            DrawLoaderSlot(player.armor, ItemSlot.Context.InWorld, 10 + r, x1, y, player);

            // Draw 3 armor slots (no visual)
            DrawLoaderSlot(player.armor, ItemSlot.Context.InWorld, r, x2, y, player);

            // Draw backup visual slots with accurate ItemSlot.Context,
            // which, for some reason, works...
            float scaleBackup = Main.inventoryScale;
            Main.inventoryScale = size / (float)TextureAssets.InventoryBack.Width();
            ItemSlot.Draw(sb, player.armor, ItemSlot.Context.EquipArmorVanity, 10 + r, new Vector2(x1, y));
            ItemSlot.Draw(sb, player.armor, ItemSlot.Context.EquipArmor, r, new Vector2(x2, y));
            Main.inventoryScale = scaleBackup;
        }

        Vector2 accTopLeft = new(topLeft.X, topLeft.Y + 3 * rowStep);

        int dyeRows = Math.Max(0, player.dye.Length - 3);
        int equipRows = Math.Max(0, player.armor.Length - 3);
        int vanityRows = Math.Max(0, player.armor.Length - 13);
        int totalRows = Math.Min(dyeRows, Math.Min(equipRows, vanityRows));

        for (int r = 0; r < totalRows; r++)
        {
            int y = (int)accTopLeft.Y + r * rowStep;
            var rowRect = new Rectangle((int)accTopLeft.X, y, 3 * rowStep, rowStep);

            if (y + size > viewport.Bottom) break;

            int dyeIndex = 3 + r;
            int vanityIndex = 13 + r;
            int equipIndex = 3 + r;

            int x0 = (int)accTopLeft.X + 0 * rowStep;
            int x1 = (int)accTopLeft.X + 1 * rowStep;
            int x2 = (int)accTopLeft.X + 2 * rowStep;

            // Draw accessory dye slots
            DrawLoaderSlot(player.dye, ItemSlot.Context.EquipDye, dyeIndex, x0, y, player);

            // Draw accessory vanity slots
            DrawLoaderSlot(player.armor, ItemSlot.Context.EquipAccessoryVanity, vanityIndex, x1, y, player);

            // Draw accessory slots
            DrawLoaderSlot(player.armor, ItemSlot.Context.EquipAccessory, equipIndex, x2, y, player);
        }
    }
    private static void DrawBuffs(SpriteBatch sb, Vector2 start, Player player, Rectangle viewport)
    {
        int size = 32;
        int pad = 6;
        int perRow = 10;
        float textScale = 0.75f;
        var font = FontAssets.MouseText.Value;
        int n = 0;

        for (int i = 0; i < player.buffType.Length; i++)
        {
            int id = player.buffType[i];
            if (id <= 0) continue;
            if (!player.HasBuff(id)) continue;
            if (id >= TextureAssets.Buff.Length) continue;

            int row = n / perRow;
            int col = n % perRow;

            int x = (int)(start.X + col * (size + pad));
            int y = (int)(start.Y + row * (size + pad));

            if (y + size > viewport.Bottom) break;

            var iconRect = new Rectangle(x, y, size, size);

            bool showTime = player.buffTime[i] > 2 && !Main.buffNoTimeDisplay[id];
            string label = string.Empty;
            Vector2 timePos = default;
            Rectangle textRect = Rectangle.Empty;

            if (showTime)
            {
                int seconds = player.buffTime[i] / 60;
                if (seconds >= 60)
                {
                    int minutes = seconds / 60;
                    label = minutes + " m";
                }
                else
                {
                    label = seconds + " s";
                }

                int xOffset = 0;
                if (seconds <= 600 && seconds >= 60) xOffset = 4;
                if (seconds <= 60) xOffset = 2;
                if (seconds <= 10) xOffset = 3;
                if (seconds <= 9) xOffset = 6;

                timePos = new Vector2(iconRect.X + xOffset, iconRect.Bottom + 2);

                Vector2 textSize = font.MeasureString(label) * textScale;
                textRect = new Rectangle((int)timePos.X, (int)timePos.Y, (int)Math.Ceiling(textSize.X), (int)Math.Ceiling(textSize.Y));
            }

            Point mouse = Main.MouseScreen.ToPoint();
            bool hoverIcon = iconRect.Contains(mouse) && !PlayerInput.IgnoreMouseInterface;
            bool hoverText = showTime && textRect.Contains(mouse) && !PlayerInput.IgnoreMouseInterface;
            bool hover = hoverIcon || hoverText;

            float alpha = hover ? 1f : 0.75f;

            sb.Draw(TextureAssets.Buff[id].Value, iconRect, Color.White * alpha);

            if (showTime)
            {
                sb.DrawString(font, label, timePos, Color.White * alpha, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
            }

            if (hover)
            {
                string name = Lang.GetBuffName(id);
                string desc = Lang.GetBuffDescription(id);
                string tooltip = string.IsNullOrEmpty(desc) ? name : name + "\n" + desc;

                Main.instance.MouseText(tooltip);
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseRight && Main.mouseRightRelease && !Main.debuff[id])
                {
                    player.DelBuff(i);
                    Main.mouseRightRelease = false;
                }
            }

            n++;
        }
    }

    #endregion

    #region Helpers
    private static int CountActiveBuffs(Player p)
    {
        int n = 0;
        for (int i = 0; i < p.buffType.Length; i++)
            if (p.buffType[i] > 0 && p.buffTime[i] > 0) n++;
        return n;
    }
    #endregion
    private void Back_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
    {
        IngameFancyUI.Close();
        if (_returnSnapshot.HasValue) { ChatSession.Restore(_returnSnapshot.Value); _returnSnapshot = null; }
    }

    private static void DrawDebugRect(Rectangle r) => Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, r, Color.Red * 0.5f);
}
