using Terraria;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.PlayerIcons.PlayerInfo;

internal class BossTracker : ModPlayer
{
    private static int LastNPCHit = -1;

    public static int GetLastNPCHit()
    {
        return LastNPCHit;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (target != null && target.active && target.boss)
        {
            LastNPCHit = target.whoAmI;
        }
    }
}
