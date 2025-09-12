using ChatPlus.Core.Features.Uploads.UploadInfo;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Uploads
{
    public class UploadElement : BaseElement<Upload>
    {
        public Upload Element;

        private UIText img;

        public UploadElement(Upload data) : base(data)
        {
            Element = data;
            Height.Set(90, 0);
            Width.Set(0, 1);

            img = new(Element.Tag);
            img._textScale = 0.3f;
            img.Left.Set(5, 0);
            img.Top.Set(8, 0);
            Append(img);
        }

        public override void Draw(SpriteBatch sb)
        {
            Height.Set(60, 0);
            base.Draw(sb);

            var dims = GetDimensions();
            Vector2 pos = dims.Position();

            // Draw image
            img._textScale = 0.3f;
            img.Left.Set(5, 0);
            img.Top.Set(8, 0);

            // Draw image hover tooltip
            Rectangle bounds = new((int)pos.X, (int)pos.Y, (int)60, (int)60);
            if (bounds.Contains(Main.MouseScreen.ToPoint()))
            {
                UICommon.TooltipMouseText($"Shift+click to delete {Data.FileName}");
                HoveredUploadOverlay.SuppressThisFrame();
                //HoveredUploadOverlay.Set(Data);
            }
            // debug
            //sb.Draw(TextureAssets.MagicPixel.Value, bounds, Color.Red*0.5f);

            // Draw file name
            TextSnippet[] snip = [new TextSnippet(Data.Tag)];
            pos += new Vector2(65, 5);
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, pos, 0f, Vector2.Zero, Vector2.One, out _);
        }
    }
}
