using Microsoft.Xna.Framework.Graphics;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.PlayerIcons;

public class PlayerIconSnippet : TextSnippet
{
    private TextSnippet snippet;
    private string text;

    public PlayerIconSnippet(TextSnippet snippet) : base(snippet.Text, snippet.Color, snippet.Scale)
    {
        this.snippet = snippet;
        CheckForHover = snippet.CheckForHover;
        DeleteWhole = snippet.DeleteWhole;
        text = snippet.Text.Trim();
    }

    public override void Update()
    {
        snippet.Update();
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1)
    {
        return snippet.UniqueDraw(justCheckingString, out size, spriteBatch, position, color, scale);
    }
}
