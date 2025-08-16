using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.ItemWindow
{
    // Shows an item Entry with its [i:ID] tag and name
    public class ItemElement : NavigationElement<Item>
    {
        public Item Entry;
        public ItemElement(Item entry) : base(entry)
        {
            Entry = entry;
            Height.Set(30, 0);
            Width.Set(0, 1);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            var dims = GetDimensions();
            Vector2 pos = dims.Position();

            // Render tag + name using chat pipeline so [i:ID] draws the icon
            string text = $"{Entry.Tag} {Entry.Name}";
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                text,
                pos + new Vector2(3, 4),
                Color.White,
                0f,
                Vector2.Zero,
                Vector2.One
            );
        }
    }
}
