using System;
using System.Collections.Generic;
using System.Text;
using ChatPlus.ModIconHandler;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.ModIcons;

/// <summary>
/// Tag handler for mod icons: [mi:InternalName]
/// </summary>
public sealed class ModIconTagHandler : ITagHandler
{
    private static readonly Dictionary<string, Texture2D> Registry = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, string> DisplayNames = new(StringComparer.OrdinalIgnoreCase);

    public static void Clear()
    {
        Registry.Clear();
        DisplayNames.Clear();
    }

    public static string GenerateTag(string internalName) => $"[mi:{internalName}]";

    public static bool Register(Mod mod)
    {
        if (mod == null) return false;
        string internalName = mod.Name ?? string.Empty;
        if (string.IsNullOrEmpty(internalName)) return false;

        Texture2D tex = null;
        string smallPath = $"{internalName}/icon_small"; // tML conventional small icon
        if (ModContent.HasAsset(smallPath))
        {
            tex = ModContent.Request<Texture2D>(smallPath).Value;
        }
        else
        {
            string normalPath = $"{internalName}/icon";
            if (ModContent.HasAsset(normalPath))
                tex = ModContent.Request<Texture2D>(normalPath).Value;
        }

        if (tex == null) return false; // skip mods without icon assets

        Registry[internalName] = tex;
        DisplayNames[internalName] = mod.DisplayName ?? internalName;
        return true;
    }

    public static bool TryGet(string internalName, out Texture2D tex) => Registry.TryGetValue(internalName, out tex);
    public static bool TryGetDisplay(string internalName, out string display) => DisplayNames.TryGetValue(internalName, out display);

    TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
    {
        string key = text?.Trim() ?? string.Empty;
        if (!Registry.TryGetValue(key, out var tex))
        {
            // fallback: show raw
            return new TextSnippet($"[mi:{key}]");
        }

        string disp = DisplayNames.GetValueOrDefault(key, key);
        return new ModIconSnippet(tex, key, disp)
        {
            Text = GenerateTag(key)
        };
    }
}
