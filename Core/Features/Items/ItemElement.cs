using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Items
{
    // Shows an item color with its [i:ID] tag and name
    public class ItemElement : BaseElement<ItemEntry>
    {
        public ItemEntry item;
        public ItemElement(ItemEntry item) : base(item)
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
            float scale = 1.1f;
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                tag,
                pos + new Vector2(3, 2),
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(scale),
                -1f,
                1.0f
            );

            // Render display name
            TextSnippet[] snip = [new TextSnippet(item.Tag)];
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, pos += new Vector2(32, 3), 0f, Vector2.Zero, Vector2.One, out _);

            // Draw tooltip on hover
            var hoverRect = new Rectangle((int)pos.X -28, (int)pos.Y, 26, 26);
            //sb.Draw(TextureAssets.MagicPixel.Value, hoverRect, Color.Red * 0.5f);

            if (hoverRect.Contains(Main.MouseScreen.ToPoint()))
            {
                UICommon.TooltipMouseText("");
                Main.LocalPlayer.mouseInterface = true;

                var hoverItem = new Terraria.Item();
                hoverItem.SetDefaults(item.ID); // use the netID
                if (hoverItem.stack <= 0) hoverItem.stack = 1;

                Main.HoverItem = hoverItem;
                Main.hoverItemName = hoverItem.Name;
            }

        }
    }
}
