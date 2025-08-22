using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.UploadHandler
{
    public class UploadElement : BaseElement<Upload>
    {
        public Upload Element;
        public UploadElement(Upload data) : base(data)
        {
            Element = data;
            Height.Set(90, 0);
            Width.Set(0, 1);
        }

        public override void Draw(SpriteBatch sb)
        {
            Height.Set(30, 0);
            base.Draw(sb);

            var dims = GetDimensions();
            Vector2 pos = dims.Position();

            // Draw preview image
            string path = Element.FullFilePath;

            Texture2D tex = Data.Texture;

            if (tex != null)
            {
                Rectangle rect = new(
                    (int)pos.X + 2,
                    (int)pos.Y + 2,
                    (int)dims.Height - 4,
                    (int)dims.Height - 4
                );

                sb.Draw(tex, rect, Color.White);
            }

            // Draw file name
            var imgName = new[] { new TextSnippet(Data.Tag) };
            pos += new Vector2(30, 5);
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, imgName, pos, 0f, Vector2.Zero, Vector2.One, out _);
        }
    }
}
