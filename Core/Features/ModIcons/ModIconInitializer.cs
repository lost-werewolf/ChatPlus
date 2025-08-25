using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.ModIconHandler;

internal class ModIconInitializer : ModSystem
{
    public static List<ModIcon> ModIcons { get; private set; } = [];

    public override void PostSetupContent()
    {
        BuildModIconList();
    }

    public override void PostAddRecipes() => BuildModIconList();

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
            if (ModIconTagHandler.Register(mod))
            {
                string internalName = mod.Name;
                string displayName = mod.DisplayName ?? internalName;
                ModIcons.Add(new ModIcon(ModIconTagHandler.GenerateTag(internalName), internalName, displayName));
            }
        }
        ModIcons.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, System.StringComparison.OrdinalIgnoreCase));
    }
}
