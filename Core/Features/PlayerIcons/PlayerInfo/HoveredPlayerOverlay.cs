using Terraria;

namespace ChatPlus.Core.Features.PlayerIcons
.PlayerInfo;

/// <summary>
/// Global static storage for the currently hovered player index whose info panel should be drawn last.
/// </summary>
public static class HoveredPlayerOverlay
{
    private static int _hoveredIndex = -1;

    public static void Set(int idx)
    {
        if (idx >= 0 && idx < Main.maxPlayers)
            _hoveredIndex = idx;
    }

    internal static int Consume()
    {
        int v = _hoveredIndex;
        _hoveredIndex = -1; // consume so it only draws for this frame unless re-set
        return v;
    }
}
