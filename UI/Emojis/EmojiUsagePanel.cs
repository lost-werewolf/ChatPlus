using System;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Emojis
{
    public class EmojiUsagePanel : DraggablePanel
    {
        private UIText text;
        public EmojiUsagePanel()
        {
            // Size
            Width.Set(220, 0);
            Height.Set(60, 0);

            // Style
            BackgroundColor = ColorHelper.DarkBlue * 0.8f;

            // Position
            VAlign = 1f;
            Left.Set(80, 0);
            UpdateTopPosition();

            text = new("", 0.9f, false)
            {
            };
            Append(text);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            Conf.C.Open();

            // Expand autocomplete section in config after UI is built
            Main.QueueMainThreadAction(() =>
            {
                var state = Main.InGameUI.CurrentState;
                if (state is not UIElement root) return;

                Conf.C.ExpandSection(root, "Emojis");
            });
        }

        public void UpdateText(string rawText)
        {
            float scale = 0.9f;
            string tooltip = rawText;

            // Measure the text size
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(tooltip);
            float scaledWidth = textSize.X * scale / Main.UIScale;

            // If the text is too wide, insert a line break at the last space before it overflows
            if (scaledWidth > Width.Pixels)
            {
                // Estimate the max number of characters that fit
                int maxChars = (int)(Width.Pixels / scale / textSize.X * tooltip.Length);
                int breakIndex = tooltip.LastIndexOf(' ', Math.Min(tooltip.Length - 1, maxChars));
                if (breakIndex > 0)
                {
                    tooltip = tooltip[..breakIndex] + "\n" + tooltip[(breakIndex + 1)..];
                }
            }

            text.SetText(tooltip);
        }

        public void UpdateTopPosition()
        {
            var sys = ModContent.GetInstance<EmojiSystem>();
            var cp = sys?.emojiState?.emojiPanel;
            const float gap = 0f;

            if (cp != null)
            {
                // Place directly above emoji panel
                Top.Set(cp.Top.Pixels - Height.Pixels - gap, 0f);
                Left.Set(cp.Left.Pixels, 0f);
            }
            else
            {
                // Fallback before panel exists: compute from config
                int visible = Conf.C == null ? 10 : Conf.C.emojisConfig.EmojisVisible;
                int panelHeight = 30 * visible;
                Top.Set(-panelHeight - Height.Pixels - gap, 0f);
                Left.Set(80, 0f);
            }

            Recalculate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
