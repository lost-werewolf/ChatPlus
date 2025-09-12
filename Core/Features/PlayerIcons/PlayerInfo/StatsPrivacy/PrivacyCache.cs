using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Helpers;
using Terraria;

namespace ChatPlus.Core.Features.PlayerIcons.PlayerInfo.StatsPrivacy;
public static class PrivacyCache
{
    static readonly Config.UserStatsPrivacy[] values = new Config.UserStatsPrivacy[Main.maxPlayers];

    static PrivacyCache()
    {
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = Config.UserStatsPrivacy.Everyone;
        }
    }

    // debug function
    public static void PrintAll()
    {
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            var p = Main.player[i];
            if (p?.active == true)
            {
                var privacy = Get(i);
                Log.Info(p.name + ": " + privacy);
            }
        }
    }

    public static void Set(int whoAmI, Config.UserStatsPrivacy value)
    {
        if (whoAmI < 0) return;
        if (whoAmI >= values.Length) return;
        values[whoAmI] = value;
    }

    public static Config.UserStatsPrivacy Get(int whoAmI)
    {
        if (whoAmI < 0) return Config.UserStatsPrivacy.Everyone;
        if (whoAmI >= values.Length) return Config.UserStatsPrivacy.Everyone;
        return values[whoAmI];
    }
}
