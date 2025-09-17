using System.Collections.Generic;
using ChatPlus.Core.Features.Uploads;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.ModIcons;

internal class ModIconManager : ModSystem
{
    public static List<ModIcon> ModIcons { get; private set; } = [];

    public override void PostSetupContent()
    {
        BuildModIconList();
    }

    public override void Unload()
    {
        ModIcons = null;
        ModIconTagHandler.Clear();
    }

    private static void BuildModIconList()
    {
        ModIcons.Clear();
        foreach (var mod in ModLoader.Mods)
        {
            if (mod == null) continue;

            string internalName = mod.Name;
            string displayName = mod.DisplayName ?? internalName;

            if (ModIconTagHandler.Register(mod))
            {
                // Successfully registered
                ModIcons.Add(new ModIcon(ModIconTagHandler.GenerateTag(internalName), mod));
            }
        }

        // Sort alphabetically (a-z).
        ModIcons.Sort((a, b) => string.Compare(a.mod.DisplayName, b.mod.DisplayName, System.StringComparison.OrdinalIgnoreCase));
    }
}
