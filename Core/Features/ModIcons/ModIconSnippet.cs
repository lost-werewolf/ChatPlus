using System;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.ModIcons;

/// <summary>
/// Renders a mod icon inline as a snippet.
/// </summary>
public class ModIconSnippet : TextSnippet
{
    public readonly Texture2D tex;
    public readonly Mod mod;

    public ModIconSnippet(Texture2D tex, Mod mod)
    {
        this.tex = tex;
        this.mod = mod;

        var pixel = TextureAssets.MagicPixel.Value;
        //sb.Draw(pixel, position, Color.Gray);
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch sb, Vector2 position = default, Color color = default, float scale = 1f)
    {
        float h = 20 * scale;

        if (tex == null)
        {
            size = new Vector2(0f, h);
            return true;
        }
        color = Color.White;

        float maxDim = tex.Width > tex.Height ? tex.Width : tex.Height;
        float s = h / maxDim;
        float w = tex.Width * s;

        size = new Vector2(w, h);
        if (justCheckingString) return true;

        // small vertical nudge to match input baseline look
        position.Y += 5f;

        DrawSmallModIcon(sb, mod, position, 26);

        return true;
    }
    private static void DrawSmallModIcon(SpriteBatch sb, Mod mod, Vector2 pos, int size)
    {
        Texture2D tex = null;
        if (mod == null)
            tex = Ass.TerrariaIcon.Value;
        else if (mod.Name == "ModLoader")
            tex = Ass.tModLoaderIcon.Value;
        else
        {
            string smallPath = $"{mod.Name}/icon_small";
            string normalPath = $"{mod.Name}/icon";

            if (ModContent.HasAsset(smallPath))
                tex = ModContent.Request<Texture2D>(smallPath, AssetRequestMode.ImmediateLoad).Value;
            else if (ModContent.HasAsset(normalPath))
                tex = ModContent.Request<Texture2D>(normalPath, AssetRequestMode.ImmediateLoad).Value;
        }

        var target = new Rectangle((int)pos.X - 3, (int)pos.Y - 2, size, size);
        if (tex != null)
        {
            DrawTextureScaledToFit(sb, tex, target);
            return;
        }

        if (mod != null)
        {
            string initials = string.IsNullOrEmpty(mod.DisplayName) ? mod.Name : mod.DisplayName;
            initials = initials.Length >= 2 ? initials[..2] : initials;
            Vector2 p = target.Center.ToVector2() + new Vector2(0, 5);
            Utils.DrawBorderString(sb, initials, p, Color.White, 1f, 0.5f, 0.5f);
        }
    }

    private static void DrawTextureScaledToFit(SpriteBatch sb, Texture2D tex, Rectangle target)
    {
        if (tex == null)
            return;

        float scale = Math.Min(
            target.Width / (float)tex.Width,
            target.Height / (float)tex.Height
        );

        sb.Draw(tex, target.Center.ToVector2(), null, Color.White, 0f, tex.Size() / 2f, scale, SpriteEffects.None, 0f);
    }

    public override void OnHover()
    {
        base.OnHover();

        UICommon.TooltipMouseText(mod.DisplayName);
    }
}
