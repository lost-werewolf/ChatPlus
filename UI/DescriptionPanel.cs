using System;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AdvancedChatFeatures.UI
{
    public class DescriptionPanel<TData> : DraggablePanel
    {
        private readonly UIText text;
        private NavigationPanel<TData> owner;

        public DescriptionPanel(NavigationPanel<TData> owner, string initialText)
        {
            // Size
            Width.Set(320, 0);
            Height.Set(60, 0);

            // Style
            BackgroundColor = ColorHelper.DarkBlue * 0.8f;

            // Position
            VAlign = 1f;
            Left.Set(80, 0);
            ResetText();

            text = new(initialText, 0.9f, false);
            Append(text);
            this.owner = owner;
        }

        public override void RightClick(UIMouseEvent evt)
        {
            Conf.C.Open();

            // Expand autocomplete section in config after UI is built
            Main.QueueMainThreadAction(() =>
            {
                var state = Main.InGameUI.CurrentState;
                if (state is not UIElement root) return;

                Conf.C.ExpandSection(root, "Style");
            });
        }

        public void ResetText()
        {
            text?.SetText("List of commands\nPress tab to complete");
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
            //base.Draw(spriteBatch);
        }
    }
}
