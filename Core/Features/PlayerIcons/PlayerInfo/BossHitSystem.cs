using Terraria;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.PlayerIcons.PlayerInfo;

internal class BossHitSystem : ModPlayer
{
    private static int LastNPCHit;
    public static int GetLastNPCHit()
    {
        if (LastNPCHit < 0 || LastNPCHit >= Main.maxNPCs)
            return -1;
        NPC npc = Main.npc[LastNPCHit];
        if (npc == null || !npc.active)
            return -1;
        return LastNPCHit;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //Main.NewText(target.FullName);
        if (target != null && target.active)
            BossHitSystem.LastNPCHit = target.whoAmI;
    }
}
