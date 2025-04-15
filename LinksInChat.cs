using Terraria.ModLoader;

namespace LinksInChat
{
    // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public class LinksInChat : Mod
    {

        public static Mod Instance;
        public override void Load()
        {
            Instance = this;
            base.Load();
        }
    }
}
