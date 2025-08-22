using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI;
using AdvancedChatFeatures.UploadHandler;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.EmojiHandler
{
    public class EmojiElement : BaseElement<Emoji>
    {
        public Emoji Emoji;

        private UIText text;
        private EmojiIconImage image;
        public EmojiElement(Emoji emoji) : base(emoji)
        {
            Emoji = emoji;
            image = new(emoji.FilePath);
            text = new(emoji.Tag, 1.0f, false)
            {
                Left = { Pixels = 32 },
                VAlign = 0.45f,
            };

            Append(image);
            //Append(text);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);


            if (text != null) RemoveChild(text);

            var dims = GetDimensions();
            Vector2 pos = dims.Position();

            var nameSnips = new[] { new TextSnippet(Emoji.Tag.ToString()) { Color = Color.White, CheckForHover = false } };
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                nameSnips,
                pos + new Vector2(32, 3),
                0f,
                Vector2.Zero,
                new Vector2(1.0f),
                out _
            );

            text.VAlign = 0.45f;
        }
    }
}
