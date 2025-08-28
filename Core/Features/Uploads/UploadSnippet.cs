// UploadSnippet.cs
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Uploads
{
    public class UploadSnippet : TextSnippet
    {
        private readonly Texture2D tex;

        public UploadSnippet(Texture2D texture) => tex = texture;

        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1f)
        {
            float targetH = 147f * scale;
            if (tex == null || tex.Height <= 0) { size = new Vector2(0f, targetH); return true; }

            float s = targetH / tex.Height;
            float w = tex.Width * s;

            size = new Vector2(w, targetH);
            if (justCheckingString) return true;

            spriteBatch.Draw(tex, position, null, Color, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
            return true;
        }
    }
}
