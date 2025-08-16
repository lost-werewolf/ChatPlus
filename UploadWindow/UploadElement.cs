using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
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
            Height.Set(30, 0);
            Width.Set(0, 1);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            var dims = GetDimensions();
            var pos = dims.Position();

            // Draw preview image
            Texture2D tex = null;
            string path = Element.FilePath;
            if (ModContent.HasAsset(path))
                tex = ModContent.Request<Texture2D>(path).Value;

            if (tex != null)
            {
                sb.Draw(tex, pos + new Vector2(3, 4), Microsoft.Xna.Framework.Color.White);
            }

            // Draw file name
            string text = $"{Element.Tag} {Element.Key}";
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, text, pos + new Microsoft.Xna.Framework.Vector2(3, 4), Microsoft.Xna.Framework.Color.White, 0f, Microsoft.Xna.Framework.Vector2.Zero, Microsoft.Xna.Framework.Vector2.One);
        }
    }
}
