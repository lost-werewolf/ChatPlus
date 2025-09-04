using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Mentions;

public class MentionSnippet : TextSnippet
{
    public MentionSnippet(TextSnippet src) : base(src.Text, src.Color, src.Scale) 
    { 

    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size,
        SpriteBatch sb, Vector2 pos = default, Color color = default, float scale = 1f)
    {
        size = default;
        if (justCheckingString)
            return false;

        string t = Text ?? string.Empty;
        Vector2 m = FontAssets.MouseText.Value.MeasureString(t) * scale;

        return false; // let vanilla draw text
    }
}
