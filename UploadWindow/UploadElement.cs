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
            Height.Set(30*8, 0);
            base.Draw(sb);
            var dims = GetDimensions();

            // Draw preview image
            string path = Element.FullFilePath;

            Texture2D tex = Data.Image;

            Main.NewText(UploadInitializer.Uploads.Count);

            if (tex != null)
            {
                Rectangle target = dims.ToRectangle();
                DrawHelper.DrawTextureScaledToFit(sb, tex, target);
            }

            // Draw file name
            string text = $"{Element.Tag} {Element.Tag}";
            ChatManager.DrawColorCodedStringWithShadow(
                sb, 
                FontAssets.MouseText.Value, 
                text, 
                dims.Position() + new Microsoft.Xna.Framework.Vector2(32, 4), 
                Microsoft.Xna.Framework.Color.White, 0f, 
                Microsoft.Xna.Framework.Vector2.Zero,
                Microsoft.Xna.Framework.Vector2.One);
        }
    }
}
