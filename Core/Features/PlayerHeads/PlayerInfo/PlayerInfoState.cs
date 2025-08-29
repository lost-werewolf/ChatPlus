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
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.PlayerHeads.PlayerInfo
{
    public class PlayerInfoState : UIState, ILoadable
    {
        public static PlayerInfoState instance;

        private string _playerName = "Unknown";
        private int _whoAmI = -1;
        private ChatSession.Snapshot? _returnSnapshot;

        private UIElement _messageBox;
        private UIScrollbar _scrollbar;
        public UITextPanel<string> titlePanel;

        private static Type _messageBoxType;
        private static MethodInfo _setTextMethod;
        private static MethodInfo _setScrollbarMethod;

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
            var uiContainer = new UIElement { Width = { Percent = 0.8f }, MaxWidth = new StyleDimension(1000f, 0f), Top = { Pixels = 120f }, Height = { Pixels = -120f, Percent = 1f }, HAlign = 0.5f }; Append(uiContainer);
            var panel = new UIPanel { Width = { Percent = 1f }, Height = { Pixels = -110f, Percent = 1f }, BackgroundColor = UICommon.MainPanelBackground }; uiContainer.Append(panel);
            var body = new UIPanel { Width = { Pixels = -25f, Percent = 1f }, Height = { Percent = 1f }, BackgroundColor = Color.Transparent, BorderColor = Color.Transparent }; panel.Append(body);
            if (_messageBoxType != null)
            {
                _messageBox = (UIElement)Activator.CreateInstance(_messageBoxType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { "" }, null);
                _messageBox.Width.Set(0, 1f); _messageBox.Height.Set(0, 1f); body.Append(_messageBox);
            }
            _scrollbar = new UIScrollbar { Height = { Pixels = -12f, Percent = 1f }, VAlign = 0.5f, HAlign = 1f }.WithView(100f, 1000f); panel.Append(_scrollbar);
            if (_messageBox != null && _setScrollbarMethod != null) _setScrollbarMethod.Invoke(_messageBox, new object[] { _scrollbar });
            titlePanel = new UITextPanel<string>($"Player: {_playerName}", 0.8f, true) { HAlign = 0.5f, Top = { Pixels = -35f }, BackgroundColor = UICommon.DefaultUIBlue }.WithPadding(15f); uiContainer.Append(titlePanel);
            var bottom = new UIElement { Width = { Percent = 1f }, Height = { Pixels = 40f }, VAlign = 1f, Top = { Pixels = -60f } }; uiContainer.Append(bottom);
            var back = new UITextPanel<string>("Back") { Width = { Percent = 1f }, Height = { Pixels = 40f } }.WithFadedMouseOver(); back.OnLeftClick += Back_OnLeftClick; bottom.Append(back);
        }

        public void SetReturnSnapshot(ChatSession.Snapshot snap) => _returnSnapshot = snap;

        public void SetPlayer(int whoAmI, string nameOverride = null) { _whoAmI = whoAmI; _playerName = nameOverride ?? Main.player?[whoAmI]?.name ?? $"Player {whoAmI}"; }

        public override void OnActivate()
        {
            base.OnActivate();
            titlePanel?.SetText($"Player: {_playerName}");
            if (_messageBox != null && _setTextMethod != null) _setTextMethod.Invoke(_messageBox, new object[] { "" });
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

            // Draw background
            DrawSeparatorBorder(sb, bgRect);
            DrawSurfaceBackground(sb, bgRect);

            // Draw player
            var playerPos = new Vector2(bgRect.X + bgRect.Width * 0.5f - 100f, bgRect.Y + 50);
            DrawPlayer(sb, playerPos, player);

            // Stats header
            Vector2 statsHeaderPos = new(leftColumn, y0-4);
            var bar = TextureAssets.MagicPixel.Value;
            Utils.DrawBorderStringBig(sb, "Stats", statsHeaderPos, Color.White, scale: 0.5f);

            // Row 1
            Rectangle hpBounds = new(leftColumn, y0 + rowHeight + 6, panelWidth, rowHeight);
            Rectangle manaBounds = new(rightColumn, y0 + rowHeight + 6, panelWidth, rowHeight);

            // Row 2
            Rectangle defBounds = new(leftColumn, y0 + rowHeight*2 + 12, panelWidth, rowHeight);
            Rectangle deathBounds = new(rightColumn, y0 + rowHeight*2 + 12, panelWidth, rowHeight);

            // Row 3
            Rectangle coinBounds = new(leftColumn, y0 + rowHeight * 3 + 18, panelWidth, rowHeight);
            Rectangle ammoBounds = new(rightColumn, y0 + rowHeight * 3 + 18, panelWidth, rowHeight);

            // Row 4
            Rectangle minionBounds = new(leftColumn, y0 + rowHeight * 4 + 24, panelWidth, rowHeight);
            Rectangle sentryBounds = new(rightColumn, y0 + rowHeight * 4 + 24, panelWidth, rowHeight);

            // Draw stats
            DrawStat_HP(sb, hpBounds, player);
            DrawStat_Mana(sb, manaBounds, player);
            DrawStat_Defense(sb, defBounds, player);
            DrawStat_DeathCount(sb, deathBounds, player);
            DrawStat_Coins(sb, coinBounds, player);
            DrawStat_Ammo(sb, ammoBounds, player);
            DrawStat_Minions(sb, minionBounds, player);
            DrawStat_Sentries(sb, sentryBounds, player);

            // Draw inventory
            DrawInventory(sb, bgRect, player);

            // Draw accessories
            Vector2 armorPos = new(leftColumn-200, y0+3*44+120-44*5);
            if (Main.screenWidth < 1000)
            {
                armorPos += new Vector2(225, 225);
            }
            Vector2 accessoriesPos = new(armorPos.X, armorPos.Y+3*44);

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
            if (minionBounds.Contains(p)) UICommon.TooltipMouseText("Minions (Current / Max)");
            if (sentryBounds.Contains(p)) UICommon.TooltipMouseText("Sentries (Current / Max)");
        }

        private static void DrawStat_Minions(SpriteBatch sb, Rectangle rect, Player player)
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

        private static void DrawStat_Sentries(SpriteBatch sb, Rectangle rect, Player player)
        {
            var tex = Ass.StatPanel;
            rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height());
            sb.Draw(tex.Value, rect, Color.White);

            int cur = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                var pr = Main.projectile[i];
                if (pr.active && pr.owner == player.whoAmI && pr.sentry)
                    cur++;
            }
            int max = player.maxTurrets;

            // Icon: Queen Spider Staff (any sentry icon)
            var pos = new Vector2(rect.X + 15, rect.Y + 15);
            ItemSlot.DrawItemIcon(new Item(ItemID.QueenSpiderStaff), 31, sb, pos, 0.8f, 32f, Color.White);

            string t = $"{cur}/{max}";
            var tp = new Vector2(rect.X + 52, rect.Y + 4);
            Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, t, tp.X, tp.Y, Color.White, Color.Black, Vector2.Zero, 1f);
        }

        private static void DrawInventory(SpriteBatch sb, Rectangle bgRect, Player player)
        {
            var back = TextureAssets.InventoryBack.Value; var font = FontAssets.MouseText.Value; int size = 40, pad = 4; var start = new Vector2(bgRect.X, bgRect.Bottom + 52); Utils.DrawBorderStringBig(sb, "Inventory", new Vector2(start.X + 2, start.Y - 35), Color.White, 0.5f);

            bool TryEquipFromInventory(Player p, int invIndex)
            {
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
                    if (target < 0) target = equip;
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

        private static void DrawLoaderSlot(Item[] items, int context, int slot, int x, int y)
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
                    ItemSlot.LeftClick(items, context, slot);
                    Main.mouseLeftRelease = false;
                }
                if (Main.mouseRight && Main.mouseRightRelease)
                {
                    ItemSlot.RightClick(items, context, slot);
                    Main.mouseRightRelease = false;
                }

                ItemSlot.MouseHover(items, context, slot);
            }

            ItemSlot.Draw(Main.spriteBatch, ref it, context, new Vector2(x, y));

            Main.inventoryScale = old;
        }

        private static void DrawArmor(SpriteBatch sb, Vector2 topLeft, Player player)
        {
            var font = FontAssets.MouseText.Value; int size = 40, pad = 4; const string title = "Armor"; var hs = font.MeasureString(title); float hy = topLeft.Y - hs.Y - 10f; Utils.DrawBorderStringBig(sb, title, new Vector2(topLeft.X, hy), Color.White, 0.52f);
            float old = Main.inventoryScale; Main.inventoryScale = size / (float)TextureAssets.InventoryBack.Width();
            for (int r = 0; r < 3; r++)
            {
                int x0 = (int)topLeft.X, x1 = (int)topLeft.X + (size + pad), x2 = (int)topLeft.X + 2 * (size + pad), y = (int)topLeft.Y + r * (size + pad);
                DrawLoaderSlot(player.dye, ItemSlot.Context.EquipDye, r, x0, y);
                DrawLoaderSlot(player.armor, ItemSlot.Context.EquipArmorVanity, 10 + r, x1, y);
                DrawLoaderSlot(player.armor, ItemSlot.Context.EquipArmor, r, x2, y);
            }
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

                DrawLoaderSlot(player.dye, 12, dyeIndex, x0, y);
                DrawLoaderSlot(player.armor, 11, vanityIndex, x1, y);
                DrawLoaderSlot(player.armor, 10, equipIndex, x2, y);
            }
        }

        private void Back_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            IngameFancyUI.Close();
            if (_returnSnapshot.HasValue) { ChatSession.Restore(_returnSnapshot.Value); _returnSnapshot = null; }
        }

        private static void DrawStat_Defense(SpriteBatch sb, Rectangle rect, Player player)
        {
            var tex = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Stat_Defense"); rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height()); sb.Draw(tex.Value, rect, Color.White);
            var defenseText = $"{player.statDefense}"; var snippets = ChatManager.ParseMessage(defenseText, Color.White).ToArray(); var pos = new Vector2(rect.X + 52, rect.Y + 4);
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snippets, pos, 0f, Vector2.Zero, Vector2.One, out _);
        }

        private static void DrawStat_HP(SpriteBatch sb, Rectangle rect, Player player)
        {
            var tex = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Stat_HP"); var rect2 = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height()); sb.Draw(tex.Value, rect2, Color.White);
            var lifeText = $"{player.statLife}/{player.statLifeMax2}"; var snippets = ChatManager.ParseMessage(lifeText, Color.White).ToArray(); var pos = new Vector2(rect2.X + 32, rect2.Y + 4);
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snippets, pos, 0f, Vector2.Zero, Vector2.One, out _);
        }

        private static void DrawStat_Mana(SpriteBatch sb, Rectangle rect, Player player)
        {
            var tex = Ass.StatPanel; rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height()); sb.Draw(tex.Value, rect, Color.White);
            var manaTex = TextureAssets.Mana; var manaRect = new Rectangle(rect.X + 4, rect.Y + 2, manaTex.Width(), manaTex.Height()); sb.Draw(TextureAssets.Mana.Value, manaRect, Color.White);
            var manaText = $"{player.statMana}/{player.statManaMax2}"; var size = FontAssets.MouseText.Value.MeasureString(manaText); var textPos = new Vector2(rect.X + rect.Width - size.X - 5, rect.Y + 5);
            Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, manaText, textPos.X, textPos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
        }

        private static void DrawStat_DeathCount(SpriteBatch sb, Rectangle rect, Player player)
        {
            var tex = Ass.StatPanel; rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height()); sb.Draw(tex.Value, rect, Color.White);
            var item = new Item(ItemID.Tombstone); var pos = new Vector2(rect.X + 15, rect.Y + 15); ItemSlot.DrawItemIcon(item, 31, sb, pos, 0.8f, 32f, Color.White);
            var deathCount = player.numberOfDeathsPVE.ToString(); var size = FontAssets.MouseText.Value.MeasureString(deathCount); var textPos = new Vector2(rect.X + 52, rect.Y + 4);
            Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, deathCount, textPos.X, textPos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
        }

        private static void DrawStat_Coins(SpriteBatch sb, Rectangle rect, Player player)
        {
            var tex = Ass.StatPanel; rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height()); sb.Draw(tex.Value, rect, Color.White);
            long total = 0; for (int i = 50; i <= 53; i++) { var it = player.inventory[i]; if (it.IsAir) continue; if (it.type == ItemID.PlatinumCoin) total += it.stack * 1_000_000L; else if (it.type == ItemID.GoldCoin) total += it.stack * 10_000L; else if (it.type == ItemID.SilverCoin) total += it.stack * 100L; else if (it.type == ItemID.CopperCoin) total += it.stack; }
            long rem = total; int p = (int)(rem / 1_000_000L); rem %= 1_000_000L; int g = (int)(rem / 10_000L); rem %= 10_000L; int s = (int)(rem / 100L); int c = (int)(rem % 100L);
            var font = FontAssets.MouseText.Value; var pos = new Vector2(rect.X + 6, rect.Y + 6); int icon = 32; float sc = 1f; float dx = p > 99 ? 40f : 28f;
            ItemSlot.DrawItemIcon(new Item(ItemID.PlatinumCoin), 31, sb, pos, sc, icon, Color.White); Utils.DrawBorderStringFourWay(sb, font, p.ToString(), pos.X - 5, pos.Y + 6, Color.White, Color.Black, Vector2.Zero, sc);
            pos.X += dx; ItemSlot.DrawItemIcon(new Item(ItemID.GoldCoin), 31, sb, pos, sc, icon, Color.White); Utils.DrawBorderStringFourWay(sb, font, g.ToString(), pos.X - 8, pos.Y + 6, Color.White, Color.Black, Vector2.Zero, sc);
            pos.X += dx; ItemSlot.DrawItemIcon(new Item(ItemID.SilverCoin), 31, sb, pos, sc, icon, Color.White); Utils.DrawBorderStringFourWay(sb, font, s.ToString(), pos.X - 8, pos.Y + 6, Color.White, Color.Black, Vector2.Zero, sc);
            if (p <= 99) { pos.X += dx; ItemSlot.DrawItemIcon(new Item(ItemID.CopperCoin), 31, sb, pos, sc, icon, Color.White); Utils.DrawBorderStringFourWay(sb, font, c.ToString(), pos.X - 8, pos.Y + 6, Color.White, Color.Black, Vector2.Zero, sc); }
        }

        private static void DrawStat_Ammo(SpriteBatch sb, Rectangle rect, Player player)
        {
            var tex = Ass.StatPanel; rect = new Rectangle(rect.X, rect.Y, tex.Width(), tex.Height()); sb.Draw(tex.Value, rect, Color.White);
            int highestStack = 0; Item highest = new(ItemID.WoodenArrow); for (int i = 54; i <= 57; i++) { var it = player.inventory[i]; if (it.IsAir) continue; if (it.stack > highestStack) { highestStack = it.stack; highest = it; } }
            var pos = new Vector2(rect.X + 15, rect.Y + 15); ItemSlot.DrawItemIcon(highest, 31, sb, pos, 0.8f, 32f, Color.White);
            var t = highestStack.ToString(); var size = FontAssets.MouseText.Value.MeasureString(t); var textPos = new Vector2(rect.X + 52, rect.Y + 4);
            Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, t, textPos.X, textPos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
        }

        private static void DrawPlayer(SpriteBatch sb, Vector2 pos, Player player)
        {
            // Restart spritebatch to make player draw on top
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            // Make player walk
            int frame = (int)(Main.GlobalTimeWrappedHourly / 0.07f) % 14 + 6;
            int y = frame * 56;
            player.bodyFrame.Y = player.legFrame.Y = player.headFrame.Y = y;

            // Make player stand still and be boring
            //player.direction = 1; // force to face right
            player.heldProj = -1;
            player.itemAnimation = 0;
            player.itemTime = 0;
            player.PlayerFrame();
            //player.WingFrame(false);

            // Draw player
            pos += Main.screenPosition + new Vector2(100, 88);
            Main.PlayerRenderer.DrawPlayer(Main.Camera, player, pos, 0f, Vector2.Zero, scale: 1.0f);
        }

        private static void DrawSurfaceBackground(SpriteBatch sb, Rectangle rect)
        {
            var tex = Main.Assets.Request<Texture2D>("Images/MapBG1").Value; 
            int d = 4; 
            rect = new Rectangle(rect.X + d, rect.Y + d, rect.Width - d * 2, rect.Height - d * 2); 
            sb.Draw(tex, rect, Color.White);
        }

        private static void DrawSeparatorBorder(SpriteBatch sb, Rectangle rect, int edgeWidth = 2)
        {
            var tex = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Separator1").Value; var color = new Color(89, 116, 213, 255) * 0.9f;
            DrawPanel(tex, edgeWidth, 0, sb, new Vector2(rect.X, rect.Y), rect.Width, color); 
            DrawPanel(tex, edgeWidth, 0, sb, new Vector2(rect.X, rect.Bottom - edgeWidth), rect.Width, color);
            DrawPanelVertical(tex, edgeWidth, 0, sb, new Vector2(rect.X, rect.Y), rect.Height, color); DrawPanelVertical(tex, edgeWidth, 0, sb, new Vector2(rect.Right - edgeWidth, rect.Y), rect.Height, color);
        }

        private static void DrawPanel(Texture2D texture, int edgeWidth, int edgeShove, SpriteBatch spriteBatch, Vector2 position, float width, Color color)
        {
            spriteBatch.Draw(texture, new Vector2(position.X + edgeWidth, position.Y), new Rectangle(edgeWidth + edgeShove, 0, texture.Width - (edgeWidth + edgeShove) * 2, texture.Height), color, 0f, Vector2.Zero, new Vector2((width - (edgeWidth * 2)) / (float)(texture.Width - (edgeWidth + edgeShove) * 2), 1f), SpriteEffects.None, 0f);
        }

        private static void DrawPanelVertical(Texture2D texture, int edgeWidth, int edgeShove, SpriteBatch spriteBatch, Vector2 position, float height, Color color)
        {
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y + edgeWidth, edgeWidth, (int)height - edgeWidth * 2), new Rectangle(texture.Width / 2, 0, 1, texture.Height), color);
        }
    }
}
