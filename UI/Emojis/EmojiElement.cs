using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Emojis
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
            text = new(emoji.DisplayName)
            {
                Left = { Pixels = 32 }
            };

            Append(image);
            Append(text);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Main.chatText = Emoji.FilePath; // Paste emoji tag into chat
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            text.VAlign = 0.5f;

            if (IsMouseHovering && !string.IsNullOrEmpty(Emoji.Tag))
            {
                UICommon.TooltipMouseText(Emoji.DisplayName);
            }
        }
    }
}
