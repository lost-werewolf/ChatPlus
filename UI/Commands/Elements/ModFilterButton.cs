using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands.Elements
{
    public class ModFilterButton : UIColoredImageButton
    {
        private List<Mod> modsWithCommands = [];
        private int currentIndex = -1; // -1 = All

        public ModFilterButton() : base(Ass.ButtonModFilter, isSmall: true)
        {
            modsWithCommands = CommandsHelper.GetModsWithCommands();

            if (ModLoader.TryGetMod("ModLoader", out var tml) && tml.File != null)
                SetImageWithoutSettingSize(CommandsHelper.GetModIcon(tml.File) ?? Ass.ButtonModFilter);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            CycleIndex(1); // cycle forward on click
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);
            CycleIndex(-1); // cycle backward on right click
        }
        public void CycleIndex(int delta)
        {
            int n = modsWithCommands.Count;
            if (n == 0) return;

            currentIndex += delta;
            if (currentIndex > n - 1) currentIndex = -1;
            if (currentIndex < -1) currentIndex = n - 1;

            Mod selected = currentIndex < 0 ? null : modsWithCommands[currentIndex];

            var sys = ModContent.GetInstance<CommandsSystem>();
            sys?.commandsListState?.commandsPanel?.Repopulate(selected);

            Asset<Texture2D> icon =
                (ModLoader.TryGetMod("ModLoader", out var tml) && tml.File != null)
                    ? (CommandsHelper.GetModIcon(tml.File) ?? Ass.ButtonModFilter)
                    : Ass.ButtonModFilter;

            try
            {
                if (selected != null && selected.Name != "Terraria" && selected.File != null)
                    icon = CommandsHelper.GetModIcon(selected.File) ?? icon;
            }
            catch { }

            SetImageWithoutSettingSize(icon);

            Main.NewText(selected == null
                ? "Filter: All mods"
                : $"Filter: {selected.DisplayName} (i: {currentIndex}/{n})");
        }

        public override void Draw(SpriteBatch sb)
        {
            //base.Draw(spriteBatch);

            // draw the panel bits (same as base, but without the final _texture draw)
            var dims = GetDimensions();
            var pos = dims.Position() + new Vector2(dims.Width, dims.Height) / 2f;

            sb.Draw(_backPanelTexture.Value, pos, null, Color.White * (IsMouseHovering ? _visibilityActive : _visibilityInactive),
                0f, _backPanelTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

            if (_hovered)
                sb.Draw(_backPanelBorderTexture.Value, pos, null, Color.White, 0f, _backPanelBorderTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

            if (_selected)
                sb.Draw(_backPanelHighlightTexture.Value, pos, null, Color.White, 0f, _backPanelHighlightTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

            if (_middleTexture != null)
                sb.Draw(_middleTexture.Value, pos, null, Color.White, 0f, _middleTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

            // scale icon to fit inside the panel with a small padding
            if (_texture?.Value != null)
            {
                const float pad = 4f; // keep off the border
                float availW = dims.Width - pad * 2f;
                float availH = dims.Height - pad * 2f;

                Vector2 texSize = _texture.Size();
                if (texSize.X > 0f && texSize.Y > 0f)
                {
                    float scale = MathHelper.Min(availW / texSize.X, availH / texSize.Y);
                    sb.Draw(_texture.Value, pos, null, _color, 0f, texSize / 2f, scale, SpriteEffects.None, 0f);
                }
            }

            if (IsMouseHovering)
            {
                Mod selected = (currentIndex >= 0 && currentIndex < modsWithCommands.Count)
                    ? modsWithCommands[currentIndex]
                    : null;

                string modName = selected?.DisplayNameClean ?? "All mods";
                UICommon.TooltipMouseText(modName);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
