using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.UI.Emojis
{
    public class EmojiElement : NavigationElement
    {
        public Emoji Emoji;

        public EmojiElement(Emoji emoji)
        {
            Emoji = emoji;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Main.chatText = Emoji.Tag; // Paste emoji tag into chat
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            var dims = GetDimensions();
            Vector2 pos = dims.Position();

            // Render the emoji by drawing its tag via ChatManager
            string render = Emoji.Tag + " " + Emoji.Name;
            float scale = 0.9f;
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                render,
                pos + new Vector2(6, 6),
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
