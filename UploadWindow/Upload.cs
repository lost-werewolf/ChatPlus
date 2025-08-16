using Microsoft.Xna.Framework.Graphics;

namespace AdvancedChatFeatures.UploadWindow
{
    public readonly record struct Upload(string Tag, string FileName, string FullFilePath, Texture2D Image);
}
