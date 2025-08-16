using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AdvancedChatFeatures.Emojis
{
    public class EmojiElement : NavigationElement
    {
        public Emoji Emoji;

        private UIText text;
        private EmojiIconImage image;

        public EmojiElement(Emoji emoji)
        {
            Emoji = emoji;
            image = new(emoji.FilePath);
            text = new(emoji.Tag, 1.0f, false)
            {
                Left = { Pixels = 32 },
                VAlign = 0.45f,
            };

            Append(image);
            Append(text);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            text.VAlign = 0.45f;
        }
    }
}
