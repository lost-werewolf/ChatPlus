using ChatPlus.Common.Configs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.PlayerColors;
public class PlayerJoinSystem : ModPlayer
{
    public override void OnEnterWorld()
    {
        var cfg = Conf.C;
        string hex = (cfg.PlayerColor ?? "FFFFFF").Trim().TrimStart('#').ToUpperInvariant();

        PlayerColorSystem.PlayerColors[Player.whoAmI] = hex;

        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            PlayerColorNetHandler.ClientHello(Player.whoAmI, PlayerColorSystem.PlayerColors[Player.whoAmI]);
        }
    }
}