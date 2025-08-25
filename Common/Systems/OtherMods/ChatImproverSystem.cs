using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ChatPlus.Common.Systems.OtherMods
{
    internal class ChatImproverSystem : ModSystem
    {
        public override void Load()
        {
            if (ModLoader.TryGetMod("ChatImprover", out Mod _))
            {
            }
        }

        public override void Unload()
        {
            if (ModLoader.TryGetMod("ChatImprover", out Mod _))
            {
            }
        }
    }
}
