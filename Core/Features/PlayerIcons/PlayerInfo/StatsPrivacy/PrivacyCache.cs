using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatPlus.Common.Configs;
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
