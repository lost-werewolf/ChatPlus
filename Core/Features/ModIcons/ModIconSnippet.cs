using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.ModIcons;

/// <summary>
/// Renders a mod icon inline as a snippet.
/// </summary>
public class ModIconSnippet : TextSnippet
{
    public readonly Texture2D Texture;
    public readonly string InternalName;
    public readonly string DisplayName;

    public ModIconSnippet(Texture2D tex, string internalName, string displayName)
    {
        Texture = tex;
        InternalName = internalName;
        DisplayName = displayName;
    }

    public override void OnHover()
    {
        base.OnHover();
        if (!string.IsNullOrEmpty(DisplayName))
        {
            Main.instance.MouseText(DisplayName);
        }
    }
}
