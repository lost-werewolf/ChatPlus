using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Emojis;

public class EmojiElement : BaseElement<Emoji>
{
    private readonly Emoji emoji;
    private readonly EmojiIconImage image;

    public EmojiElement(Emoji emoji) : base(emoji)
    {
        this.emoji = emoji;
        image = new EmojiIconImage(emoji.FilePath);
        Append(image);
        Height.Set(30, 0);
        Width.Set(0, 1f);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        var dims = GetDimensions();
        Vector2 pos = dims.Position() + new Vector2(32, 4);

        // If the panel was triggered via ":" → also show display name after the tag
        if (EmojiSystem.OpenedFromColon)
        {
            string name = emoji.Description ?? emoji.Tag;
            var nameSnip = new TextSnippet[] { new TextSnippet(":" + name) };
            ChatManager.DrawColorCodedStringWithShadow(
                sb, FontAssets.MouseText.Value,
                nameSnip, pos - new Vector2(0, 0), // offset to the right of tag
                0f, Vector2.Zero, Vector2.One, out _
            );
        }
        else
        {
            // draw the tag, e.g. [e:smile]
            var tagSnip = new TextSnippet[] { new TextSnippet(emoji.Tag) };
            ChatManager.DrawColorCodedStringWithShadow(
                sb, FontAssets.MouseText.Value,
                tagSnip, pos, 0f, Vector2.Zero, Vector2.One, out _
            );
        }
    }
}
