using AdvancedChatFeatures.Glyphs;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace AdvancedChatFeatures.Glyphs
{
    public class GlyphState : UIState
    {
        public GlyphPanel glyphPanel;
        public DescriptionPanel<Glyph> glyphDescPanel;

        public GlyphState()
        {
            glyphPanel = new();
            Append(glyphPanel);

            glyphDescPanel = new(centerText: true);
            Append(glyphDescPanel);

            glyphPanel.ConnectedPanel = glyphDescPanel;
            glyphDescPanel.ConnectedPanel = glyphPanel;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
