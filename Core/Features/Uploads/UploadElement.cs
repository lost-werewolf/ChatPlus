using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Uploads
{
    public class UploadElement : BaseElement<Upload>
    {
        public Upload Element;

        private UIText img;
        private UIText imgTagName;

        public UploadElement(Upload data) : base(data)
        {
            Element = data;
            Height.Set(90, 0);
            Width.Set(0, 1);

            img = new(Element.Tag);
            Append(img);
        }

        public override void Draw(SpriteBatch sb)
        {
            Height.Set(60, 0);
            base.Draw(sb);

            img._textScale = 0.3f;
            img.Left.Set(5, 0);
            img.Top.Set(8, 0);
            //imgTagName.SetText(Element.Tag-="", 0.1f, true);

            var dims = GetDimensions();
            Vector2 pos = dims.Position();

            // Draw file name
            TextSnippet[] snip = [new TextSnippet(Data.Tag)];
            pos += new Vector2(65, 5);
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, pos, 0f, Vector2.Zero, Vector2.One, out _);
        }
    }
}
