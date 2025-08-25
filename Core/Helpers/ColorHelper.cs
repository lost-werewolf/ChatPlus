namespace ChatPlus.Core.Helpers
{
    public static class ColorHelper
    {
        // For blue, purple, hovered link colors, etc, see:
        // https://en.wikipedia.org/wiki/Help:Link_color
        public static Color Blue => new(17, 85, 204);
        public static Color BlueUnderline => new Color(10, 15, 154);
        public static Color BlueHover => new(7, 55, 99);

        public static Color DarkBlue = new(33, 43, 79);
        public static Color UIPanelBlue => new Color(63, 82, 151) * 0.7f;
    }
}