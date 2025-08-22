using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.UploadHandler;

public sealed class SpacerSnippet : TextSnippet
{
    private readonly float _width, _height;
    public SpacerSnippet(float width, float height = 0f)
    {
        _width = width; _height = height;
        Text = ""; Color = Color.White; CheckForHover = false; DeleteWhole = false;
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch sb, Vector2 pos = default, Color color = default, float scale = 1f)
    {
        size = new Vector2(_width * scale, _height * scale); // consumes width, draws nothing

        // Debug draw
        Rectangle r = new((int)pos.X, (int)pos.Y, 300, (int)_height);
        //sb.Draw(TextureAssets.MagicPixel.Value, r, Color.Red);

        return true;
    }
}
