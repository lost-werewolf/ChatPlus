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
            text = new(emoji.DisplayName, 1.0f, false)
            {
                Left = { Pixels = 32 },
                VAlign = 0.5f,
            };

            Append(image);
            Append(text);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);


            // Insert tag into chat
            string tag = Emoji.Tag ?? "";
            if (string.IsNullOrEmpty(tag)) return;

            string chat = Main.chatText ?? string.Empty;
            int colon = chat.LastIndexOf(':');
            if (colon >= 0)
                Main.chatText = chat[..colon] + tag + " ";
            else
                Main.chatText = chat + tag + " ";
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            text.VAlign = 0.45f;

            if (IsMouseHovering && !string.IsNullOrEmpty(Emoji.Tag))
            {
                UICommon.TooltipMouseText(Emoji.Tag);
            }
        }
    }
}
