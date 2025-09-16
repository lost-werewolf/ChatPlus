using Microsoft.Xna.Framework.Graphics;

namespace ChatPlus.Core.Features.Stats.UploadStats
{
    public static class HoveredUploadOverlay
    {
        private static Texture2D hovered;

        public static void Set(Texture2D texture)
        {
            hovered = texture;
        }

        internal static Texture2D Consume()
        {
            var tex = hovered;
            hovered = null;
            return tex;
        }
    }
}
