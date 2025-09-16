using System.Collections.Generic;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.PlayerColors;

public class PlayerColorSystem : ModSystem
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

    public override void OnWorldUnload()
    {
        // needed! had a bug where color was set to white but would show as blue for some reason when doing MP testing
        PlayerColors.Clear(); 
    }
}
