using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Glyphs
{
    public class GlyphUsagePanel : DraggablePanel
    {
        private readonly UIText text;
        public GlyphUsagePanel()
        {
            Width.Set(220, 0);
            Height.Set(60, 0);
            BackgroundColor = ColorHelper.DarkBlue * 0.8f;
            VAlign = 1f;
            Left.Set(80, 0);
            UpdateTopPosition();

            text = new("List of glyphs\nPress tab to complete", 0.9f, false) { };
            Append(text);
        }

        public void UpdateText(string rawText)
        {
            text.SetText(rawText ?? "");
        }

        public void UpdateTopPosition()
        {
            var sys = ModContent.GetInstance<GlyphSystem>();
            var gp = sys?.glyphState?.glyphPanel;
            const float gap = 0f;

            if (gp != null)
            {
                Top.Set(gp.Top.Pixels - Height.Pixels - gap, 0f);
                Left.Set(gp.Left.Pixels, 0f);
            }
            else
            {
                int visible = Conf.C == null ? 10 : Conf.C.autocompleteConfig.CommandsVisible;
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
