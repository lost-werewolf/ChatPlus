using System;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;

namespace AdvancedChatFeatures.UI.Emojis
{
    public class EmojiUsagePanel : DraggablePanel
    {
        private UIText text;
        private int itemCount;
        public EmojiUsagePanel()
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

        public void UpdateTopPosition()
        {
            itemCount = Conf.C == null ? 10 : Conf.C.autocompleteConfig.CommandsVisible;
            Top.Set(-30 * itemCount - 30 - 6, 0);
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
