using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Glyphs
{
    public class GlyphState : UIState
    {
        public GlyphPanel glyphPanel;
        public DescriptionPanel<Glyph> glyphUsagePanel;

        public GlyphState()
        {
            glyphPanel = new();
            Append(glyphPanel);

            glyphUsagePanel = new(owner: glyphPanel, "List of glyphs");
            Append(glyphUsagePanel);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
