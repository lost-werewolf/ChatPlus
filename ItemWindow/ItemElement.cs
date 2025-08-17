using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.ItemWindow
{
    // Shows an item color with its [i:ID] tag and name
    public class ItemElement : NavigationElement<Item>
    {
        public Item item;
        public ItemElement(Item item) : base(item)
        {
            this.item = item;
            Height.Set(30, 0);
            Width.Set(0, 1);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            var dims = GetDimensions();
            Vector2 pos = dims.Position();

            // Render tag
            string tag = item.Tag;
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                tag,
                pos + new Vector2(3, 2),
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(1.0f),
                -1f,
                1.0f
            );

            // Render display name
            var glyphSnippet = new[] { new TextSnippet(item.Tag.ToString()) { Color = Color.White, CheckForHover = false } };
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                glyphSnippet,
                pos + new Vector2(32, 3),
                0f,
                Vector2.Zero,
                new Vector2(1.0f),
                out _
            );
        }
    }
}
