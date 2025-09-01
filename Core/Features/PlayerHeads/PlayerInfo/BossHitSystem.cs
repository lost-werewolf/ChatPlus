using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.PlayerHeads.PlayerInfo;
internal class BossHitSystem : ModPlayer
{
    public static int LastNPCHit;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //Main.NewText(target.FullName);
        if (target != null && target.active)
            BossHitSystem.LastNPCHit = target.whoAmI;
    }

    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
    {
        //Main.NewText(target.FullName);

        base.OnHitNPCWithItem(item, target, hit, damageDone);
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        //Main.NewText(target.FullName);

        base.OnHitNPCWithProj(proj, target, hit, damageDone);
    }
}
