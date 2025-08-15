using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI.Chat;

public class PlayerIconSnippet : TextSnippet
{
    private readonly string playerName;

    public PlayerIconSnippet(string playerName) : base("")
    {
        this.playerName = playerName;
        Color = Color.White;
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1)
    {
        scale = 0.75f;
        size = new Vector2(20, 20) * scale;

        if (!justCheckingString)
        {
            position += new Vector2(-7, 8);
            DrawHelper.DrawPlayerHead(position, color: Color.White*0.5f, scale, spriteBatch);
        }

        return true;
    }
}
