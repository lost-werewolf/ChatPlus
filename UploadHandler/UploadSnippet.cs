// UploadSnippet.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI.Chat;

namespace ChatPlus.UploadHandler
{
    public class UploadSnippet : TextSnippet
    {
        private readonly Texture2D tex;

        // 9 lines * 20px = 180px image; total chat input height elsewhere = 32 + 180 = 212
        private const float TargetH = 180f;

        public UploadSnippet(Texture2D texture) => tex = texture;

        public override bool UniqueDraw(
            bool justCheckingString,
            out Vector2 size,
            SpriteBatch spriteBatch,
            Vector2 position = default,
            Color color = default,
            float scale = 1f)
        {
            float h = TargetH * scale;

            if (tex == null)
            {
                size = new Vector2(0f, h);
                return true;
            }

            float maxDim = tex.Width > tex.Height ? tex.Width : tex.Height;
            float s = h / maxDim;
            float w = tex.Width * s;

            size = new Vector2(w, h);
            if (justCheckingString) return true;

            // small vertical nudge to match input baseline look
            position.Y += 5f;

            spriteBatch.Draw(tex, position, null, Color, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
            return true;
        }
    }
}
