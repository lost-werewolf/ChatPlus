using AdvancedChatFeatures.Glyphs;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.UploadWindow
{
    public class UploadElement : NavigationElement<Upload>
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
            Height.Set(30*4, 0);
            base.Draw(sb);
            var dims = GetDimensions();

            // Draw preview image
            string path = Element.FullFilePath;

            Texture2D tex = Data.Image;

            if (tex != null)
            {
                Rectangle target = dims.ToRectangle();
                DrawHelper.DrawTextureScaledToFit(sb, tex, target);
            }

            // Draw file name
            var imgName = new[] { new TextSnippet(Data.FileName) { Color = Color.White, CheckForHover = false } };
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, imgName, dims.Position(), 0f, Vector2.Zero, Vector2.One, out _);
        }
    }
}
