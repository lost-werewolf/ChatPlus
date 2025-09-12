using Microsoft.Xna.Framework.Graphics;

namespace ChatPlus.Core.Features.Uploads.UploadInfo
{
    public static class HoveredUploadOverlay
    {
        private static Texture2D hovered;
        private static bool suppressed;

        public static void Set(Texture2D texture)
        {
            hovered = texture;
        }

        public static void SuppressThisFrame()
        {
            suppressed = true;
        }

        internal static Texture2D Consume()
        {
            if (suppressed)
            {
                suppressed = false;
                hovered = null;
                return null;
            }

            var tex = hovered;
            hovered = null;
            return tex;
        }
    }
}
