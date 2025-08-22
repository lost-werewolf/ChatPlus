using Microsoft.Xna.Framework;               // Vector2, ColorItem
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI.Chat;

namespace ChatPlus.UploadHandler
{
    public class UploadSnippet : TextSnippet
    {
        private readonly Texture2D uploadedTexture;
        private readonly float targetHeight;

        public UploadSnippet(Texture2D texture, float targetHeight = 20f)
        {
            uploadedTexture = texture;
            this.targetHeight = targetHeight;
        }

        public override bool UniqueDraw(
            bool justCheckingString,
            out Vector2 size,
            SpriteBatch spriteBatch,
            Vector2 position = default,
            Color color = default,
            float scale = 1f)
        {
            float h = targetHeight * scale;

            if (uploadedTexture == null)
            {
                size = new Vector2(0f, h);
                return true;
            }

            float texMax = System.Math.Max(uploadedTexture.Width, uploadedTexture.Height);
            float s = h / texMax;
            float w = uploadedTexture.Width * s;
            size = new Vector2(w, h);

            if (justCheckingString) return true;

            // Custom y offset
            position += new Vector2(0, 5);

            spriteBatch.Draw(uploadedTexture, position, null, Color, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
            return true;
        }
    }
}
