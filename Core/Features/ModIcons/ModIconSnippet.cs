using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Features.PlayerHeads.PlayerInfo;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.UI;
using Terraria.UI;
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
        const float BaseIconSize = 22f;
        float px = BaseIconSize * Math.Max(0f, scale);
        size = new Vector2(px, px);

        if (justCheckingString || color == Color.Black)
            return true;

        var dest = new Rectangle((int)pos.X, (int)(pos.Y - 1), (int)px, (int)px);

        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        if (modName.Equals("Terraria", StringComparison.OrdinalIgnoreCase))
        {
            if (Ass.TerrariaIcon?.Value != null)
            {
                sb.Draw(Ass.TerrariaIcon.Value, dest, Color.White);
            }
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

    public override void OnClick()
    {
        base.OnClick();

        var state = ModInfoState.instance;
        if (state == null)
        {
            Main.NewText("Mod info UI not available.", Color.Orange);
            return;
        }

        if (!ModLoader.TryGetMod(modName, out var mod))
        {
            Main.NewText($"Mod \"{modName}\" not found.", Color.Orange);
            return;
        }

        string displayName = mod.DisplayName ?? mod.Name ?? "Unknown Mod";
        string internalName = mod.Name ?? displayName;
        string version = mod.Version?.ToString() ?? "Unknown";
        string description = GetDescriptionForMod(mod) + "\nVersion: " + version;

        var snap = ChatSession.Capture();
        state.SetModInfo(description, displayName, internalName);
        state.SetReturnSnapshot(snap);

        Main.drawingPlayerChat = false;
        IngameFancyUI.OpenUIState(state);
    }

    private string GetDescriptionForMod(Mod mod)
    {
        if (mod == null) return "No description available.";

        // Find the matching LocalMod
        IReadOnlyList<LocalMod> all = ModOrganizer.FindAllMods();
        LocalMod localMod = all?.FirstOrDefault(m => m != null && string.Equals(m.Name, mod.Name, StringComparison.OrdinalIgnoreCase));

        // Get description
        string desc = localMod?.properties?.description;
        if (!string.IsNullOrWhiteSpace(desc))
            return desc;

        return "";
        //return $"No description provided.\n\nVersion: {version}\nAuthor: {author}";
    }

    public override float GetStringLength(DynamicSpriteFont font) => BaseIconSize;
    public override Color GetVisibleColor() => Color.White;

    public override void OnHover()
    {
        Main.LocalPlayer.mouseInterface = true;

        //Main.instance.MouseText(GetDisplayName(modName));
        UICommon.TooltipMouseText(modName);
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

    public static string GetModSource()
    {
        try
        {
            var frames = new StackTrace().GetFrames();
            if (frames == null) return null;

            var terrariaAsm = typeof(Main).Assembly;
            var loaderAsm = typeof(ModLoader).Assembly;
            var chatPlusAsm = typeof(ModIconSnippet).Assembly;

            var pivot = -1;
            for (int i = 0; i < frames.Length; i++)
            {
                var m = frames[i].GetMethod();
                if (m == null) continue;
                var n = m.Name;
                if (n.IndexOf("NewText", StringComparison.Ordinal) >= 0 || n.IndexOf("AddNewMessage", StringComparison.Ordinal) >= 0)
                {
                    pivot = i;
                    break;
                }
            }
            if (pivot < 0) return null;

            for (int i = pivot + 1; i < frames.Length; i++)
            {
                var m = frames[i].GetMethod();
                if (m == null) continue;

                var t = m.DeclaringType;
                if (t == null) continue;

                var asm = t.Assembly;
                var ns = t.Namespace ?? string.Empty;

                // Skip engine, loader, and our own code
                if (asm == terrariaAsm) continue;
                if (asm == chatPlusAsm) continue;
                if (ns.StartsWith("ChatPlus.", StringComparison.OrdinalIgnoreCase)) continue;

                // If the caller is tModLoader (e.g., /playing), treat as ModLoader
                if (asm == loaderAsm)
                {
                    if (ns.StartsWith("Terraria.ModLoader", StringComparison.OrdinalIgnoreCase)) return "ModLoader";
                    continue;
                }

                var mod = ModLoader.Mods.FirstOrDefault(z => z != null && z.Code == asm);
                //if (mod.Name == "DragonLens") return "ModLoader";
                if (mod != null) return mod.Name;
            }

            return "ModLoader";
        }
        catch
        {
            return null;
        }
    }

}
