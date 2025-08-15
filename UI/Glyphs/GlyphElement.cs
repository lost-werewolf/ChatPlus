using AdvancedChatFeatures.Common.Configs;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.UI.Glyphs
{
    public class GlyphElement : NavigationElement
    {
        public Glyph Glyph;
        public GlyphElement(Glyph glyph)
        {
            Glyph = glyph;
            Height.Set(30, 0);
            Width.Set(0, 1);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Main.chatText = Glyph.Tag;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (Conf.C.autocompleteConfig.ShowHoverTooltips && IsMouseHovering && !string.IsNullOrEmpty(Glyph.Description))
            {
                UICommon.TooltipMouseText(Glyph.Description);
            }

            base.Draw(sb);

            var dims = GetDimensions();
            Vector2 pos = dims.Position();
            string render = Glyph.Tag;
            float scale = 1.0f;
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                render,
                pos + new Vector2(0, 0),
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(scale),
                -1f,
                scale
            );

            string name = Glyph.Name;
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                name,
                pos + new Vector2(32, 3),
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(scale),
                -1f,
                scale
            );
        }
    }
}
