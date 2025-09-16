using Terraria.ModLoader;

namespace ChatPlus.Core.Features.Stats.ModStats;

/// <summary>
/// Global static storage for hovered mod to draw its info overlay last.
/// </summary>
public static class HoveredModOverlay
{
    private static Mod _hovered;

    public static void Set(Mod mod)
    {
        if (mod != null)
            _hovered = mod;
    }

    internal static Mod Consume()
    {
        var m = _hovered;
        _hovered = null;
        return m;
    }
}
