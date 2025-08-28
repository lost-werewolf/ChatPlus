using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Links;

public class LinkSnippet : TextSnippet
{
    private Rectangle hoverRect;

    public LinkSnippet(TextSnippet src)
        : base(src.Text, src.Color, src.Scale) { }

    public static bool IsLink(string t) =>
        !string.IsNullOrWhiteSpace(t) &&
        Regex.IsMatch(t, @"^(https?://|www\.)\S+\.\S+$", RegexOptions.IgnoreCase);

    public override Color GetVisibleColor()
    {
        var baseC = new Color(32, 160, 255); // link blue
        if (hoverRect.Contains(Main.MouseScreen.ToPoint()))
            return new Color(72, 190, 255);  // hover blue
        return baseC;
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size,
        SpriteBatch sb, Vector2 pos = default, Color color = default, float scale = 1f)
    {
        // Let vanilla draw the text; we only add the underline & hover rect.
        size = default;

        if (justCheckingString) return false;

        string t = Text ?? "";
        Vector2 m = FontAssets.MouseText.Value.MeasureString(t) * scale;
        hoverRect = new Rectangle((int)pos.X, (int)pos.Y, (int)m.X, (int)m.Y);

        // underline
        int y = (int)(pos.Y + m.Y - 9);
        sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)pos.X, y, (int)m.X, 1), GetVisibleColor());

        return false; // return false so vanilla draws the glyphs normally
    }

    public override void OnClick()
    {
        string url = Text;
        if (string.IsNullOrWhiteSpace(url)) return;
        if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            url = "https://" + url;

        try
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        catch (Exception e)
        {
            Main.NewText($"Failed to open URL: {url}, {e.Message}", Color.Red);
        }
    }
}
