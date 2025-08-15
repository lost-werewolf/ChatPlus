using System;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands
{
    public class CommandUsagePanel : DraggablePanel
    {
        private readonly UIText text;
        private int itemCount;
        public CommandUsagePanel()
        {
            // Size
            Width.Set(220, 0);
            Height.Set(60, 0);

            // Style
            BackgroundColor = ColorHelper.DarkBlue * 0.8f;

            // Position
            UpdateTopPosition();
            Left.Set(80, 0);
            VAlign = 1f;

            text = new("", 0.9f, false)
            {
            };
            Append(text);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            Conf.C.Open();

            // Expand autocomplete
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
            var sys = ModContent.GetInstance<CommandSystem>();
            var cp = sys?.commandState?.commandPanel;
            if (cp == null) return;

            const float gap = 0f; 
            Top.Set(cp.Top.Pixels - cp.Height.Pixels - gap, 0f);

            // keep aligned horizontally with the command panel
            Left.Set(cp.Left.Pixels, 0f);

            Recalculate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
