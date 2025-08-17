using Microsoft.Xna.Framework.Graphics;

namespace AdvancedChatFeatures.ImageWindow
{
    public readonly record struct Image(string Tag, string FileName, string FullFilePath, Texture2D Texture);
}
