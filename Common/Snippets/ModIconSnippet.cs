using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI.Chat;

public class ModIconSnippet : TextSnippet
{
    private readonly string modName;

    public ModIconSnippet(string modName) : base("")
    {
        this.modName = modName;
        Color = Color.White;
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1)
    {
        size = new Vector2(20, 20) * scale;

        if (!justCheckingString)
        {
            //DrawHelper.DrawPlayerHead(position, scale, spriteBatch);
        }

        return true;
    }
}
