using System;
using System.Collections.Generic;
using System.Reflection;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ChatPlus.Core.Features.PlayerHeads.PlayerInfo
{
    public class PlayerInfoState : UIState, ILoadable
    {
        public static PlayerInfoState instance;

        private int _whoAmI;
        private string _playerName = "Unknown";
        private ChatSession.Snapshot? _returnSnapshot;

        private UITextPanel<string> titlePanel;
        private UIElement _messageBox;
        private UIScrollbar _scrollbar;

        private static Type _messageBoxType;
        private static MethodInfo _setTextMethod;
        private static MethodInfo _setScrollbarMethod;

        // bottom
        private UIElement _bottom;
        private UITextPanel<string> _backBtn;
        private UITextPanel<string> _prevBtn;
        private UITextPanel<string> _nextBtn;
        private int _currentPlayerIndex;

        public void Load(Mod mod)
        {
            instance = this;
            var asm = typeof(UICommon).Assembly;
            _messageBoxType = asm.GetType("Terraria.ModLoader.UI.UIMessageBox");
            _setTextMethod = _messageBoxType?.GetMethod("SetText");
            _setScrollbarMethod = _messageBoxType?.GetMethod("SetScrollbar");
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

            _scrollbar = new UIScrollbar { Height = { Pixels = -12f, Percent = 1f }, VAlign = 0.5f, HAlign = 1f }.WithView(100f, 1000f);
            panel.Append(_scrollbar);
            if (_messageBox != null && _setScrollbarMethod != null) _setScrollbarMethod.Invoke(_messageBox, new object[] { _scrollbar });

            titlePanel = new UITextPanel<string>($"Player: {_playerName}", 0.8f, true)
            { HAlign = 0.5f, Top = { Pixels = -35f }, BackgroundColor = UICommon.DefaultUIBlue }.WithPadding(15f);
            uiContainer.Append(titlePanel);

            _bottom = new UIElement { Width = { Percent = 1f }, Height = { Pixels = 40f }, VAlign = 1f, Top = { Pixels = -60f } };
            uiContainer.Append(_bottom);

            _backBtn = new UITextPanel<string>("Back") { Height = { Pixels = 40f } }.WithFadedMouseOver();
            _backBtn.OnLeftClick += Back_OnLeftClick;
            _bottom.Append(_backBtn);

            _currentPlayerIndex = _whoAmI >= 0 ? _whoAmI : Main.myPlayer;
            SetPlayerFromIndex(_currentPlayerIndex);
        }
        public override void OnActivate()
        {
            base.OnActivate();
            _currentPlayerIndex = (_whoAmI >= 0 && _whoAmI < Main.maxPlayers) ? _whoAmI : Main.myPlayer;
            SetPlayerFromIndex(_currentPlayerIndex);
            RefreshNavButtons(); // ensure correct for SP/MP and active players
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            RefreshNavButtons(); // handles SP → MP, players joining/leaving, etc.
        }

        private void RefreshNavButtons()
        {
            const float NAV_W = 36f;

            var active = GetActivePlayerIndices();           // sorted by index (whoAmI)
            bool multi = Main.netMode != NetmodeID.SinglePlayer && active.Length > 1;

            // Ensure arrow instances exist when needed
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
                    _nextBtn = new UITextPanel<string>(">", 1.0f, false) { Width = { Pixels = NAV_W }, Height = { Pixels = 40f }, HAlign = 1f }.WithFadedMouseOver();
                    _nextBtn.OnLeftClick += (_, __) => CyclePlayer(+1);
                }
            }

            // If not multiplayer or only one player: remove arrows and stretch Back
            if (!multi)
            {
                if (_prevBtn?.Parent != null) _prevBtn.Remove();
                if (_nextBtn?.Parent != null) _nextBtn.Remove();
                _backBtn.Left.Set(0f, 0f);
                _backBtn.Width.Set(0f, 1f);
                return;
            }

            // We are in MP with 2+ active players:
            int pos = Array.IndexOf(active, _currentPlayerIndex);
            if (pos < 0) pos = Array.BinarySearch(active, Main.myPlayer) >= 0 ? Array.BinarySearch(active, Main.myPlayer) : 0;

            bool hasPrev = pos > 0;
            bool hasNext = pos < active.Length - 1;

            // Add/remove the arrow elements based on availability
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

            // Lay out Back to fill remaining space
            float leftW = hasPrev ? NAV_W : 0f;
            float rightW = hasNext ? NAV_W : 0f;
            _backBtn.Left.Set(leftW, 0f);
            _backBtn.Width.Set(-(leftW + rightW), 1f);
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

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            var player = _whoAmI >= 0 && _whoAmI < Main.maxPlayers ? Main.player[_whoAmI] : null;
            if (player == null || !player.active) return;

            // Dimensions
            int panelWidth = 96, gutter = 10, rowHeight = 31;
            int containerW = Math.Min((int)(Main.screenWidth * 0.8f), 1000);
            int containerLeft = (Main.screenWidth - containerW) / 2, top = 120;
            int bgW = 436;
            int bgH = 200;
            var bgRect = new Rectangle(containerLeft+42, top+42, bgW, bgH);
            int leftColumn = containerLeft + containerW - (panelWidth * 2 + gutter)-70;
            int rightColumn = leftColumn + panelWidth + gutter;
            int y0 = bgRect.Y;
            Vector2 cursor = new Vector2(bgRect.X, bgRect.Y); // start at top left

            // Draw background
            PlayerInfoDrawer.DrawSeparatorBorder(sb, bgRect);
            PlayerInfoDrawer.DrawMapFullscreenBackground(sb, bgRect);

            // Draw top left info
            //if (Main.netMode != NetmodeID.SinglePlayer)
            //{
                //PlayerInfoDrawer.DrawTeamText(sb, cursor, player);
                //cursor += new Vector2(0, 21);
                //PlayerInfoDrawer.DrawPlayerID(sb, cursor, player);
            //}

            // Draw player
            var playerPos = new Vector2(bgRect.X + bgRect.Width * 0.5f - 100f, bgRect.Y + 50);
            PlayerInfoDrawer.DrawPlayer(sb, playerPos, player, 1.8f);

            // Stats header
            Vector2 statsHeaderPos = new(leftColumn, y0-4);
            Utils.DrawBorderStringBig(sb, "Stats", statsHeaderPos, Color.White, scale: 0.5f);

            // Bounds
            Rectangle hpBounds = new(leftColumn, y0 + rowHeight + 6, panelWidth, rowHeight);
            Rectangle manaBounds = new(rightColumn, y0 + rowHeight + 6, panelWidth, rowHeight);
            Rectangle defBounds = new(leftColumn, y0 + rowHeight*2 + 12, panelWidth, rowHeight);
            Rectangle deathBounds = new(rightColumn, y0 + rowHeight*2 + 12, panelWidth, rowHeight);
            Rectangle coinBounds = new(leftColumn, y0 + rowHeight * 3 + 18, panelWidth, rowHeight);
            Rectangle ammoBounds = new(rightColumn, y0 + rowHeight * 3 + 18, panelWidth, rowHeight);
            Rectangle minionBounds = new(leftColumn, y0 + rowHeight * 4 + 24, panelWidth, rowHeight);
            Rectangle sentryBounds = new(rightColumn, y0 + rowHeight * 4 + 24, panelWidth, rowHeight);
            Rectangle heldItemBounds = new(leftColumn, y0 + rowHeight * 5 + 30, panelWidth, rowHeight);
            Rectangle lastCreateHitBounds = new(rightColumn, y0 + rowHeight * 5 + 30, panelWidth, rowHeight);


            // Draw stats
            PlayerInfoDrawer.DrawStat_HP(sb, hpBounds, player);
            PlayerInfoDrawer.DrawStat_Mana(sb, manaBounds, player);
            PlayerInfoDrawer.DrawStat_Defense(sb, defBounds, player);
            PlayerInfoDrawer.DrawStat_DeathCount(sb, deathBounds, player);
            PlayerInfoDrawer.DrawStat_Coins(sb, coinBounds, player);
            PlayerInfoDrawer.DrawStat_Ammo(sb, ammoBounds, player);
            PlayerInfoDrawer.DrawStat_Minions(sb, minionBounds, player);
            PlayerInfoDrawer.DrawStat_Sentries(sb, sentryBounds, player);
            PlayerInfoDrawer.DrawStat_HeldItem(sb, heldItemBounds, player);
            PlayerInfoDrawer.DrawStat_LastCreatureHit(sb, lastCreateHitBounds, player);

            // Draw inventory
            DrawInventory(sb, bgRect, player);

            // Draw buffs
            if (HasAnyBuff(player))
                Utils.DrawBorderStringBig(sb, "Buffs", new Vector2(bgRect.X, bgRect.Y+476), Color.White, 0.52f);
            DrawBuffs(sb, bgRect,player);

            // Draw accessories
            Vector2 armorPos = new(leftColumn - 200 + (Main.screenWidth < 1000 ? 225 : 0),y0 + 3 * 44 + 120 - 44 * 5 + (Main.screenWidth < 1000 ? 225 : 0));
            Vector2 accessoriesPos = new(armorPos.X, armorPos.Y + 3 * 44);
            Utils.DrawBorderStringBig(sb, "Accessories", new Vector2(armorPos.X, armorPos.Y-36), Color.White, 0.52f);

            DrawArmor(sb, armorPos, player);
            DrawAccessories(sb, accessoriesPos, player);

            // Handle hovers
            Point p = Main.MouseScreen.ToPoint();
            if (hpBounds.Contains(p)) UICommon.TooltipMouseText("Health");
            if (manaBounds.Contains(p)) UICommon.TooltipMouseText("Mana");
            if (defBounds.Contains(p)) UICommon.TooltipMouseText("Defense");
            if (deathBounds.Contains(p)) UICommon.TooltipMouseText("Deaths");
            if (coinBounds.Contains(p)) UICommon.TooltipMouseText("Coins");
            if (ammoBounds.Contains(p)) UICommon.TooltipMouseText("Ammo");
            if (minionBounds.Contains(p)) UICommon.TooltipMouseText("Minions");
            if (sentryBounds.Contains(p)) UICommon.TooltipMouseText("Sentries");
            if (heldItemBounds.Contains(p)) UICommon.TooltipMouseText("Held Item");
            if (lastCreateHitBounds.Contains(p)) UICommon.TooltipMouseText("Last Creature Hit");
        }
        private static bool HasAnyBuff(Player p) { for (int i = 0; i < p.buffType.Length; i++) if (p.buffType[i] > 0 && p.buffTime[i] > 0) return true; return false; }

        private static void DrawArmor(SpriteBatch sb, Vector2 topLeft, Player player)
        {
            var font = FontAssets.MouseText.Value; 
            int size = 40, pad = 4; 
            float old = Main.inventoryScale; Main.inventoryScale = size / (float)TextureAssets.InventoryBack.Width();

            for (int r = 0; r < 3; r++)
            {
                int x0 = (int)topLeft.X, x1 = (int)topLeft.X + (size + pad), x2 = (int)topLeft.X + 2 * (size + pad), y = (int)topLeft.Y + r * (size + pad);

                // Draw slots that can be clicked on
                DrawLoaderSlot(player.dye, ItemSlot.Context.EquipDye, r, x0, y, player);
                DrawLoaderSlot(player.armor, ItemSlot.Context.EquipArmorVanity, 10 + r, x1, y, player);
                DrawLoaderSlot(player.armor, ItemSlot.Context.EquipArmor, r, x2, y, player);

                // Draw slots ON top of the existing ones that are purely visually correct
                ItemSlot.Draw(sb, player.dye, ItemSlot.Context.EquipDye, r, new Vector2(x0, y));
                ItemSlot.Draw(sb, player.armor, ItemSlot.Context.EquipArmorVanity, 10 + r, new Vector2(x1, y));
                ItemSlot.Draw(sb, player.armor, ItemSlot.Context.EquipArmor, r, new Vector2(x2, y));
            }
            Main.inventoryScale = old;
        }

        private static void DrawBuffs(SpriteBatch sb, Rectangle bgRect, Player player)
        {
            int size = 32, pad = 6, perRow = 10; var start = new Vector2(bgRect.X, bgRect.Bottom + 52 + 5 * (40 + 4) + 35); var font = FontAssets.MouseText.Value;
            int n = 0;
            for (int i = 0; i < player.buffType.Length; i++)
            {
                int id = player.buffType[i]; if (id <= 0) continue; if (!player.HasBuff(id)) continue;
                int row = n / perRow; int col = n % perRow; var r = new Rectangle((int)(start.X + col * (size + pad)), (int)(start.Y + row * (size + pad)), size, size);
                sb.Draw(TextureAssets.Buff[id].Value, r, Color.White);
                int t = player.buffTime[i];
                if (t > 2)
                {
                    int s = t / 60; string label;
                    if (s >= 3600) { label = (s / 3600).ToString() + ":" + ((s % 3600) / 60).ToString("00"); }
                    else if (s >= 60) { label = (s / 60).ToString() + ":" + (s % 60).ToString("00"); }
                    else { label = s.ToString(); }
                    var ls = font.MeasureString(label); Utils.DrawBorderString(sb, label, new Vector2(r.Right - ls.X + 1, r.Bottom - ls.Y + 1), Color.White, 0.7f, 0f, 0f);
                }
                bool hover = r.Contains(Main.MouseScreen.ToPoint()) && !PlayerInput.IgnoreMouseInterface;
                if (hover)
                {
                    UICommon.TooltipMouseText(Lang.GetBuffName(id)); Main.LocalPlayer.mouseInterface = true;
                    if (Main.mouseRight && Main.mouseRightRelease && !Main.debuff[id]) { player.DelBuff(i); Main.mouseRightRelease = false; }
                }
                n++;
            }
        }

        private static void DrawInventory(SpriteBatch sb, Rectangle bgRect, Player player)
        {
            var back = TextureAssets.InventoryBack.Value; var font = FontAssets.MouseText.Value; int size = 40, pad = 4; var start = new Vector2(bgRect.X, bgRect.Bottom + 52); Utils.DrawBorderStringBig(sb, "Inventory", new Vector2(start.X + 2, start.Y - 35), Color.White, 0.5f);

            bool TryEquipFromInventory(Player p, int invIndex)
            {
                // we can only equip items from ourself
                if (player != Main.LocalPlayer) return false; 

                ref Item it = ref p.inventory[invIndex]; if (it.IsAir) return false;

                if (it.accessory)
                {
                    int begin = 3; int end = Math.Min(10, p.armor.Length); int empty = -1;
                    for (int s = begin; s < end; s++) { if (p.armor[s].type == it.type) return false; if (empty < 0 && p.armor[s].IsAir) empty = s; }
                    if (empty >= 0) { p.armor[empty] = it.Clone(); it.TurnToAir(); return true; }
                    for (int s = begin; s < end; s++) { var tmp = p.armor[s]; p.armor[s] = it; p.inventory[invIndex] = tmp; return true; }
                    return false;
                }

                int equip = -1; int vanity = -1;
                if (it.headSlot >= 0) { equip = 0; vanity = 10; } else if (it.bodySlot >= 0) { equip = 1; vanity = 11; } else if (it.legSlot >= 0) { equip = 2; vanity = 12; }
                if (equip >= 0)
                {
                    int target = it.defense > 0 ? equip : vanity;
                    if (p.armor[target].IsAir) { p.armor[target] = it.Clone(); it.TurnToAir(); return true; }
                    var sw = p.armor[target]; p.armor[target] = it; p.inventory[invIndex] = sw; return true;
                }

                return false;
            }

            for (int i = 0; i < 50; i++)
            {
                int row = i / 10, col = i % 10; var r = new Rectangle((int)(start.X + col * (size + pad)), (int)(start.Y + row * (size + pad)), size, size); sb.Draw(back, r, Color.White);
                var it = player.inventory[i];
                if (!it.IsAir)
                {
                    ItemSlot.DrawItemIcon(it, 31, sb, r.Center(), 0.9f, size - 6, Color.White);
                    if (r.Contains(Main.MouseScreen.ToPoint()))
                    {
                        UICommon.TooltipMouseText(""); Main.LocalPlayer.mouseInterface = true; Main.HoverItem = it.Clone(); Main.hoverItemName = it.Name;
                        if (Main.mouseRight && Main.mouseRightRelease && !PlayerInput.IgnoreMouseInterface) { if (TryEquipFromInventory(player, i)) Main.mouseRightRelease = false; }
                    }
                }
                if (i < 10) { string label = i == 9 ? "0" : (i + 1).ToString(); var ls = font.MeasureString(label); var lp = new Vector2(r.Right - ls.X - 28f, r.Bottom - ls.Y - 12f); Utils.DrawBorderString(sb, label, lp, Color.White, 0.75f, 0f, 0f); }
            }
        }

        private static void DrawLoaderSlot(Item[] items, int context, int slot, int x, int y, Player player)
        {
            float old = Main.inventoryScale;
            Main.inventoryScale = 40f / TextureAssets.InventoryBack.Width();

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
                    //ItemSlot.LeftClick(items, context, slot);
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

        private static void DrawAccessories(SpriteBatch sb, Vector2 topLeft, Player player)
        {
            var bar = TextureAssets.MagicPixel.Value;
            var font = FontAssets.MouseText.Value;

            int size = 40, pad = 4;
            int rows = Math.Min(7, Math.Min(player.dye.Length - 3, Math.Min(player.armor.Length - 3, player.armor.Length - 13)));

            for (int r = 0; r < rows; r++)
            {
                int dyeIndex = 3 + r;  
                int vanityIndex = 13 + r;  
                int equipIndex = 3 + r;

                int x0 = (int)topLeft.X + 0 * (size + pad);
                int x1 = (int)topLeft.X + 1 * (size + pad);
                int x2 = (int)topLeft.X + 2 * (size + pad);
                int y = (int)topLeft.Y + r * (size + pad);

                DrawLoaderSlot(player.dye, 12, dyeIndex, x0, y, player);
                DrawLoaderSlot(player.armor, 11, vanityIndex, x1, y, player);
                DrawLoaderSlot(player.armor, 10, equipIndex, x2, y, player);
            }
        }

        private void Back_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            IngameFancyUI.Close();
            if (_returnSnapshot.HasValue) { ChatSession.Restore(_returnSnapshot.Value); _returnSnapshot = null; }
        }
    }
}
