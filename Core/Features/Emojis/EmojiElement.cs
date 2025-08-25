using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Emojis
{
    public class EmojiElement : BaseElement<Emoji>
    {
        public Emoji Emoji;

        private EmojiIconImage image;
        public EmojiElement(Emoji emoji) : base(emoji)
        {
            Emoji = emoji;
            image = new(emoji.FilePath);
            Append(image);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            var dims = GetDimensions();
            Vector2 pos = dims.Position();

            TextSnippet[] snip = [new TextSnippet(Emoji.Tag)];
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, pos += new Vector2(32, 4), 0f, Vector2.Zero, Vector2.One, out _);
        }
    }
}
