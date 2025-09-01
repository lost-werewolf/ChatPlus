using Terraria.ModLoader;

namespace ChatPlus.Common.Compat;
internal class ModReloaderSystem : ModSystem
{
    public static bool ModReloaderFound = false;
    public override void PostSetupContent()
    {
        if (ModLoader.TryGetMod("ModReloader", out Mod _))
        {
            ModReloaderFound = true;
        }
    }
}
