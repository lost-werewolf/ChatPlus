﻿using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.PlayerIcons
.PlayerInfo;

public class ModifyPlayerDrawInfo : ModPlayer
{
    public static bool ForceFullBrightOnce;
    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (!ForceFullBrightOnce)
            return;

        drawInfo.shadow = 0f;
        drawInfo.stealth = 1f;

        var p = drawInfo.drawPlayer;
        p.socialIgnoreLight = true;

        drawInfo.colorEyeWhites = Color.White;
        drawInfo.colorEyes = p.eyeColor;
        drawInfo.colorHair = p.GetHairColor(useLighting: false);
        drawInfo.colorHead = p.skinColor;
        drawInfo.colorBodySkin = p.skinColor;
        drawInfo.colorLegs = p.skinColor;

        drawInfo.colorShirt = p.shirtColor;
        drawInfo.colorUnderShirt = p.underShirtColor;
        drawInfo.colorPants = p.pantsColor;
        drawInfo.colorShoes = p.shoeColor;

        drawInfo.colorArmorHead = Color.White;
        drawInfo.colorArmorBody = Color.White;
        drawInfo.colorArmorLegs = Color.White;
        drawInfo.colorMount = Color.White;

        drawInfo.colorDisplayDollSkin = PlayerDrawHelper.DISPLAY_DOLL_DEFAULT_SKIN_COLOR;

        drawInfo.headGlowColor = new Color(drawInfo.headGlowColor.R, drawInfo.headGlowColor.G, drawInfo.headGlowColor.B, 0);
        drawInfo.bodyGlowColor = new Color(drawInfo.bodyGlowColor.R, drawInfo.bodyGlowColor.G, drawInfo.bodyGlowColor.B, 0);
        drawInfo.armGlowColor = new Color(drawInfo.armGlowColor.R, drawInfo.armGlowColor.G, drawInfo.armGlowColor.B, 0);
        drawInfo.legsGlowColor = new Color(drawInfo.legsGlowColor.R, drawInfo.legsGlowColor.G, drawInfo.legsGlowColor.B, 0);
    }
}