using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Emojis
{
    public class EmojiIconImage : UIImage
    {
        private Asset<Texture2D> textureAsset;
        private bool requested;
        private string filePath;

        public EmojiIconImage(string filePath) : base(TextureAssets.MagicPixel.Value)
        {
            this.filePath = filePath;
            Width.Set(24, 0);
            Height.Set(24, 0);
            Top.Set(4, 0);
            Left.Set(4, 0);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Top.Set(2, 0);

            if (!requested)
            {
                requested = true;
                if (ModContent.HasAsset(filePath))
                    textureAsset = ModContent.Request<Texture2D>(filePath);
            }

            var tex = textureAsset?.Value;
            if (tex != null)
            {
                Rectangle target = GetDimensions().ToRectangle();
                int widest = tex.Width > tex.Height ? tex.Width : tex.Height;
                spriteBatch.Draw(tex, target.Center.ToVector2(), null, Color.White, 0,
                    tex.Size() / 2f, target.Width / (float)widest, SpriteEffects.None, 0);
            }
            else
            {
                base.Draw(spriteBatch); 
            }
        }
    }
}