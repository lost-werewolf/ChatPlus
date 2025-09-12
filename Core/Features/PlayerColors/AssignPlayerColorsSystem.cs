using System;
using System.Collections.Generic;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.Mentions;
using ChatPlus.Core.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.PlayerColors
{
    public class AssignPlayerColorsSystem : ModSystem
    {
        public static Dictionary<int, string> PlayerColors = [];

        public List<string> RandomColors =
        [
            "ff1919", // red
            "32ff82", // green
            "327dff", // blue
            "fff014", // yellow
            "ff00a0", // pink
        ];

        public class PlayerJoinSystem : ModPlayer
        {
            public override void OnEnterWorld()
            {
                var cfg = Conf.C;
                string hex = (cfg.PlayerColor ?? "FFFFFF").Trim().TrimStart('#').ToUpperInvariant();

                PlayerColors[Player.whoAmI] = hex;
                
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    PlayerColorNetHandler.ClientHello(Player.whoAmI, AssignPlayerColorsSystem.PlayerColors[Player.whoAmI]);
                }
            }

        }
    }
}
