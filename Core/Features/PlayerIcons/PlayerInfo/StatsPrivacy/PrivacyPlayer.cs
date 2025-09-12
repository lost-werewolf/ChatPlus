using ChatPlus.Common.Configs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.PlayerIcons.PlayerInfo.StatsPrivacy
{
    public class PrivacyPlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            var privacy = Conf.C.StatsPrivacy;
            PrivacyCache.Set(Player.whoAmI, privacy);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                StatsPrivacyNetHandler.Instance.SendLocalPrivacy();
            }

            if (Main.netMode == NetmodeID.Server)
            {
                StatsPrivacyNetHandler.Instance.ServerSyncTo(Player.whoAmI);
            }
        }
    }
}
