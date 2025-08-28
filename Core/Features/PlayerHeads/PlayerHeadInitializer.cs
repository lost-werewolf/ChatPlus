using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.PlayerHeads;

/// <summary>
/// Maintains a refreshed list of active players for the PlayerIcons UI.
/// </summary>
internal class PlayerHeadInitializer : ModSystem
{
    public static List<PlayerHead> PlayerIcons { get; private set; } = new();
    private static readonly Dictionary<string, int> NameToIndex = new(StringComparer.OrdinalIgnoreCase);

    public static string GenerateTag(string name) => $"[p:{name}]";

    public override void Load()
    {
        ChatManager.Register<PlayerHeadTagHandler>("p");
    }

    public override void PostUpdatePlayers()
    {
        Refresh();
    }

    public override void OnWorldUnload()
    {
        PlayerIcons.Clear();
    }

    private static void Refresh()
    {
        try
        {
            PlayerIcons.Clear();
            NameToIndex.Clear();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var plr = Main.player[i];
                if (plr == null || !plr.active) continue;
                string name = plr.name ?? $"Player{i}";
                PlayerIcons.Add(new PlayerHead(GenerateTag(name), i, name));
                if (!NameToIndex.ContainsKey(name))
                    NameToIndex[name] = i;
            }
        }
        catch
        {
            // swallow; UI will just show previous list
        }
    }

    public static bool TryGetIndex(string name, out int idx) => NameToIndex.TryGetValue(name, out idx);
}
