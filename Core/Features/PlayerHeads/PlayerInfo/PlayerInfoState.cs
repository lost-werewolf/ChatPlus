using System;
using System.Collections.Generic;
using System.Text;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ChatPlus.Core.Features.PlayerHeads.PlayerInfo
{
    public class PlayerInfoState : UIState, ILoadable
    {
        public static PlayerInfoState instance;

        // Header fields
        private string _playerName = "Unknown";
        private int _whoAmI = -1;

        // Return-to-chat snapshot
        private ChatSession.Snapshot? _returnSnapshot;

        // UI
        private UIElement _messageBox;
        private UIScrollbar _scrollbar;
        public UITextPanel<string> titlePanel;

        // cached reflection (same pattern you used for ModInfo)
        private static Type _messageBoxType;
        private static System.Reflection.MethodInfo _setTextMethod;
        private static System.Reflection.MethodInfo _setScrollbarMethod;

        public void Load(Mod mod)
        {
            instance = this;

            var asm = typeof(UICommon).Assembly;
            _messageBoxType = asm.GetType("Terraria.ModLoader.UI.UIMessageBox");
            _setTextMethod = _messageBoxType?.GetMethod("SetText");
            _setScrollbarMethod = _messageBoxType?.GetMethod("SetScrollbar");
        }

        public void Unload()
        {
            instance = null;
        }

        public override void OnInitialize()
        {
            // container
            var uiContainer = new UIElement
            {
                Width = { Percent = 0.8f },
                MaxWidth = new StyleDimension(800f, 0f),
                Top = { Pixels = 220f },
                Height = { Pixels = -220f, Percent = 1f },
                HAlign = 0.5f
            };
            Append(uiContainer);

            // main panel
            var panel = new UIPanel
            {
                Width = { Percent = 1f },
                Height = { Pixels = -110f, Percent = 1f },
                BackgroundColor = UICommon.MainPanelBackground
            };
            uiContainer.Append(panel);

            // message box host
            var body = new UIPanel
            {
                Width = { Pixels = -25f, Percent = 1f },
                Height = { Percent = 1f },
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            panel.Append(body);

            if (_messageBoxType != null)
            {
                _messageBox = (UIElement)Activator.CreateInstance(
                    _messageBoxType,
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic,
                    binder: null,
                    args: new object[] { "" },
                    culture: null
                );
                _messageBox.Width.Set(0, 1f);
                _messageBox.Height.Set(0, 1f);
                body.Append(_messageBox);
            }

            _scrollbar = new UIScrollbar
            {
                Height = { Pixels = -12f, Percent = 1f },
                VAlign = 0.5f,
                HAlign = 1f
            }.WithView(100f, 1000f);
            panel.Append(_scrollbar);

            if (_messageBox != null && _setScrollbarMethod != null)
                _setScrollbarMethod.Invoke(_messageBox, new object[] { _scrollbar });

            titlePanel = new UITextPanel<string>($"Player: {_playerName}", 0.8f, true)
            {
                HAlign = 0.5f,
                Top = { Pixels = -35f },
                BackgroundColor = UICommon.DefaultUIBlue
            }.WithPadding(15f);
            uiContainer.Append(titlePanel);

            // bottom bar with Back
            var bottom = new UIElement
            {
                Width = { Percent = 1f },
                Height = { Pixels = 40f },
                VAlign = 1f,
                Top = { Pixels = -60f }
            };
            uiContainer.Append(bottom);

            var back = new UITextPanel<string>("Back")
            {
                Width = { Percent = 1f },
                Height = { Pixels = 40f }
            }.WithFadedMouseOver();
            back.OnLeftClick += Back_OnLeftClick;
            bottom.Append(back);
        }

        public void SetReturnSnapshot(ChatSession.Snapshot snap) => _returnSnapshot = snap;

        public void SetPlayer(int whoAmI, string nameOverride = null)
        {
            _whoAmI = whoAmI;
            _playerName = nameOverride ?? Main.player?[whoAmI]?.name ?? $"Player {whoAmI}";
        }

        public override void OnActivate()
        {
            base.OnActivate();

            // header
            titlePanel?.SetText($"Player: {_playerName}");

            // body
            if (_messageBox != null && _setTextMethod != null)
            {
                var p = _whoAmI >= 0 && _whoAmI < Main.maxPlayers ? Main.player[_whoAmI] : null;
                string text = p?.active == true ? BuildSummary(p) : "Player not available.";
                _setTextMethod.Invoke(_messageBox, new object[] { text });
            }
        }

        private void Back_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            IngameFancyUI.Close();
            if (_returnSnapshot.HasValue)
            {
                ChatSession.Restore(_returnSnapshot.Value);
                _returnSnapshot = null;
            }
        }

        private static string BuildSummary(Player p)
        {
            // Health / Mana / Defense
            int life = p.statLife;
            int lifeMax = p.statLifeMax2;
            int mana = p.statMana;
            int manaMax = p.statManaMax2;
            int defense = p.statDefense;
            int regen = p.lifeRegen; // raw regen; game uses lifeRegenTime too

            // DR/endurance (0..1)
            float dr = MathHelper.Clamp(p.endurance, 0f, 1f);

            // Position (tile coords)
            int tileX = (int)(p.position.X / 16f);
            int tileY = (int)(p.position.Y / 16f);

            // Team
            string team = p.team switch
            {
                1 => "Red",
                2 => "Green",
                3 => "Blue",
                4 => "Yellow",
                5 => "Pink",
                _ => "None"
            };

            // Coins
            var (plat, gold, silver, copper) = CountCoins(p);
            string coins = ToCoinString(plat, gold, silver, copper);

            // Ammo (group by ammo ID family)
            string ammo = "";

            // Deaths: Terraria doesn't store lifetime deaths by default -> display N/A
            // You can wire your own counter elsewhere and read it here if needed.
            string deaths = "N/A";

            var sb = new StringBuilder();
            sb.AppendLine($"Name: {p.name} (whoAmI: {p.whoAmI})");
            sb.AppendLine($"Health: {life}/{lifeMax}   Mana: {mana}/{manaMax}");
            sb.AppendLine($"Defense: {defense}   DR: {dr * 100f:0.#}%   Regen: {regen}");
            sb.AppendLine($"Team: {team}   Pos: {tileX},{tileY}");
            sb.AppendLine($"Coins: {coins}");
            if (!string.IsNullOrEmpty(ammo)) sb.AppendLine($"Ammo: {ammo}");
            sb.AppendLine($"Deaths: {deaths}");
            return sb.ToString();
        }

        private static (int plat, int gold, int silver, int copper) CountCoins(Player p)
        {
            int plat = 0, gold = 0, silver = 0, copper = 0;
            for (int i = 0; i < p.inventory.Length; i++)
            {
                var it = p.inventory[i];
                if (it == null || it.stack <= 0) continue;
                switch (it.type)
                {
                    case ItemID.CopperCoin: copper += it.stack; break;
                    case ItemID.SilverCoin: silver += it.stack; break;
                    case ItemID.GoldCoin: gold += it.stack; break;
                    case ItemID.PlatinumCoin: plat += it.stack; break;
                }
            }
            // normalize (100 copper = 1 silver, etc.)
            copper += silver % 100 * 100;
            silver /= 100;
            silver += gold % 100 * 100;
            gold /= 100;
            gold += plat % 100 * 100;
            plat /= 100;

            // re-normalize final
            gold += copper / 10000; copper %= 10000;     // 10k copper -> 1 gold
            silver += copper % 10000 / 100; copper %= 100; // remain to silver
            gold += silver / 100; silver %= 100;
            plat += gold / 100; gold %= 100;
            return (plat, gold, silver, copper);
        }

        private static string ToCoinString(int plat, int gold, int silver, int copper)
        {
            var parts = new List<string>(4);
            if (plat > 0) parts.Add($"{plat}p");
            if (gold > 0) parts.Add($"{gold}g");
            if (silver > 0) parts.Add($"{silver}s");
            if (copper > 0 || parts.Count == 0) parts.Add($"{copper}c");
            return string.Join(" ", parts);
        }
    }
}
