using Microsoft.Xna.Framework;

namespace AdvancedChatFeatures.Helpers
{
    public static class ColorHelper
    {
        public static Color[] PlayerColors = [
            new Color(255, 112, 193), // Pink
            new Color(255, 192, 8),  // Yellow
            new Color(143, 195, 58),  // Green

            // make 10 backups with varying colors with high saturation but not maximum saturation. so you cant make e.g (255,0,0), thats too much
            new Color(148,39,39), // Dark Red
            new Color(148,39,148), // Dark Purple
            new Color(39,148,39), // Dark Green
            new Color(39,148,148), // Dark Cyan
            new Color(39,39,148), // Dark Blue
            new Color(148,148,39), // Dark Yellow
        ];

        // For blue, purple, hovered link colors, etc, see:
        // https://en.wikipedia.org/wiki/Help:Link_color
        public static Color Blue => new(17, 85, 204);
        public static Color BlueHover => new(7, 55, 99);

        public static Color DarkBlue = new(33, 43, 79);
    }
}