using System;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.ModIcons;

public sealed class ModIconSnippet : TextSnippet
{
    private const float BaseIconSize = 26f;
    private readonly string modName;

    public ModIconSnippet(string modName)
    {
        this.modName = modName ?? string.Empty;
        Text = string.Empty;
        Color = Color.White;
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch sb,
                                    Vector2 pos = default, Color color = default, float scale = 1f)
    {
        float px = BaseIconSize * Math.Max(0f, scale);
        size = new Vector2(px, px);

        if (justCheckingString || color == Color.Black)
            return true;

        var dest = new Rectangle((int)pos.X, (int)(pos.Y - 2), (int)px, (int)px);

        if (modName.Equals("Terraria", StringComparison.OrdinalIgnoreCase))
        {
            var sheet = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Icon_Tags_Shadow", AssetRequestMode.ImmediateLoad);
            var frame = BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface._filterIconFrame;
            //sb.Draw(sheet.Value, dest, sheet.Frame(16, 5, frame.X, frame.Y), Color.White);
            return true;
        }
        else if (modName.Equals("ModLoader", StringComparison.OrdinalIgnoreCase))
        {
            sb.Draw(Ass.tModLoaderIcon.Value, dest, Color.White);
            return true;
        }

        // Try to resolve icon_small -> icon
        if (TryGetModIcon(modName, out var icon))
        {
            sb.Draw(icon, dest, Color.White);
            return true;
        }

        // Fallback: draw two-letter initials centered
        string initials = GetDisplayName(modName);
        if (!string.IsNullOrWhiteSpace(initials))
        {
            initials = initials.Length >= 2 ? initials[..2] : initials;
            Vector2 center = dest.Center.ToVector2();
            Utils.DrawBorderString(sb, initials, center + new Vector2(0, 4f), Color.White, 0.8f * scale, 0.5f, 0.5f);
        }
        return true;
    }

    public override float GetStringLength(DynamicSpriteFont font) => BaseIconSize;
    public override Color GetVisibleColor() => Color.White;

    public override void OnHover()
    {
        Main.instance.MouseText(GetDisplayName(modName));
    }

    private static bool TryGetModIcon(string name, out Texture2D tex)
    {
        tex = null;

        if (!ModLoader.TryGetMod(name, out var mod))
            return false;

        // Priority: icon_small.rawimg -> icon.png
        if (mod.FileExists("icon_small.rawimg"))
        {
            tex = mod.Assets.Request<Texture2D>("icon_small", AssetRequestMode.ImmediateLoad).Value;
            return tex != null;
        }

        if (mod.FileExists("icon.png"))
        {
            tex = mod.Assets.Request<Texture2D>("icon", AssetRequestMode.ImmediateLoad).Value;
            return tex != null;
        }

        return false;
    }
    private static string GetDisplayName(string name)
    {
        return ModLoader.TryGetMod(name, out var mod) ? (mod.DisplayName ?? name) : name;
    }
}
