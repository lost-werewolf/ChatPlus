using System;
using System.Collections.Generic;
using ChatPlus.Common.Configs;
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
                // Decide my local color (use config if set, else pick and save)
                var cfg = Conf.C;
                string hex = (cfg.PlayerColor ?? "FFFFFF").Trim().TrimStart('#');
                if (hex.Equals("FFFFFF", StringComparison.OrdinalIgnoreCase))
                {
                    var list = ModContent.GetInstance<AssignPlayerColorsSystem>().RandomColors;
                    hex = list[Main.rand.Next(list.Count)];
                    cfg.PlayerColor = hex;
                    try { cfg.SaveChanges(); } catch { Log.Error("Failed to save config!!"); }
                }

                AssignPlayerColorsSystem.PlayerColors[Player.whoAmI] = hex;

                // In MP, announce to server and request current table
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    PlayerColorNetHandler.ClientHello(Player.whoAmI, hex);
            }
        }
    }
}
