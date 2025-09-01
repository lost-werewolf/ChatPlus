using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Features.PlayerIcons
.PlayerInfo;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
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
        CheckForHover = true; // enable OnHover callback so overlay can draw
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch sb,
                                    Vector2 pos = default, Color color = default, float scale = 1f)
    {
        const float BaseIconSize = 24f;
        float px = BaseIconSize * Math.Max(0f, scale);
        size = new Vector2(px-8, px);
        pos.X -= 8f;

        if (justCheckingString || color == Color.Black)
            return true;

        // hover
        var hoverRect = new Rectangle((int)pos.X - 5, (int)pos.Y - 4, 32, (int)size.Y + 5);
        if (hoverRect.Contains(Main.MouseScreen.ToPoint()))
        {
            if (!Conf.C.ShowModPreviewWhenHovering) return false;

            if (ModLoader.TryGetMod(modName, out Mod mod))
            {
                HoveredModOverlay.Set(mod);
            }

            if (Main.mouseLeft && Main.mouseLeftRelease)
                OnClick();
        }
        // hover debug
        //sb.Draw(TextureAssets.MagicPixel.Value, hoverRect, Color.Red * 0.25f);

        // debug force draw info overlay
        //if (ModLoader.TryGetMod("ModLoader", out Mod test)) HoveredModOverlay.Set(test);

        var dest = new Rectangle((int)pos.X, (int)(pos.Y - 2), (int)px, (int)px);

        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        if (modName.Equals("Terraria", StringComparison.OrdinalIgnoreCase))
        {
            if (Ass.TerrariaIcon?.Value != null)
            {
                dest = new Rectangle((int)pos.X + 2, (int)(pos.Y - 1), (int)20, (int)24);
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
        string initials = ModHelper.GetDisplayName(modName);
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

        var state = ModInfoState.Instance;
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
        string description = GetDescriptionForMod(mod);

        var snap = ChatSession.Capture();
        state.SetModInfo(description, displayName, internalName);
        state.SetReturnSnapshot(snap);

        Main.drawingPlayerChat = false;
        IngameFancyUI.OpenUIState(state);
    }

    private string GetDescriptionForMod(Mod mod)
    {
        if (mod == null) return "No description available.";

        LocalMod localMod = ModHelper.GetLocalMod(mod);

        // Get description
        string desc = localMod?.properties?.description;
        if (!string.IsNullOrWhiteSpace(desc))
            return desc;

        return "";
    }

    public override float GetStringLength(DynamicSpriteFont font) => BaseIconSize;
    public override Color GetVisibleColor() => Color.White;

    public override void OnHover()
    {
        Main.LocalPlayer.mouseInterface = true;

        if (ModLoader.TryGetMod(modName, out Mod mod))
        {
            if (!Conf.C.ShowModPreviewWhenHovering) return;
            HoveredModOverlay.Set(mod);
        }
    }

    private static bool TryGetModIcon(string name, out Texture2D tex)
    {
        tex = null;

        if (!ModLoader.TryGetMod(name, out var mod))
            return false;

        // Priority: icon_small.* -> icon.*
        if (mod.FileExists("icon_small.png") || mod.FileExists("icon_small.rawimg"))
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


    public static string GetCallingName()
    {
        string name = string.Empty;

        StackFrame[] frames/* = new StackFrame[1]*/;
        try
        {
            frames = new StackTrace(true).GetFrames();
            Logging.PrettifyStackTraceSources(frames);
            int index = 2;
            while (index < frames.Length && frames[index].GetMethod().Name.Contains("NewText"))
                index++;
            if (index == frames.Length)
                name = string.Empty;
            else
            {
                var frame = frames[index];
                var method = frame.GetMethod();

                Type declaringType = method.DeclaringType;
                if (declaringType != null)
                {
                    name = declaringType.Namespace;
                    name = name.Split('.')[0];
                }
                else
                {
                    //Autogenerated methods (i.e. MonoMod detours) do not have a declaring type, default to Terraria
                    name = "Terraria";
                }

                if (name != "Terraria")
                {
                    Mod mod = ModLoader.GetMod(name);
                    if (mod == null)
                    {
                        mod = ModLoader.GetMod(name + "Mod");
                    }
                    if (mod != null)
                    {
                        name = mod.Name;
                    }
                }
            }
        }
        catch
        {
            //foreach (var frame in frames)
            //{
            //    Log.Info(frame?.ToString() ?? "frame null");
            //}
        }
        if (string.IsNullOrEmpty(name))
            return string.Empty;

        // Success!
        return name;
    }

    public static string GetModSource()
    {
        try
        {
            var frames = new StackTrace().GetFrames();
            if (frames == null || frames.Length == 0)
            {
                return "ModLoader";
            }

            int pivot = -1;
            for (int i = 0; i < frames.Length; i++)
            {
                var m = frames[i].GetMethod();
                if (m == null) continue;

                string n = m.Name;
                if (n.Contains("NewText") || n.Contains("AddNewMessage"))
                {
                    pivot = i;
                    break;
                }
            }

            int start = pivot >= 0 ? pivot + 1 : 0;

            for (int i = start; i < frames.Length; i++)
            {
                var method = frames[i].GetMethod();
                var type = method?.DeclaringType;
                if (type == null) continue;

                var assembly = type.Assembly;
                if (assembly == null) continue;

                // if the caller is this mod’s assembly, return "ChatPlus"
                if (assembly == typeof(ModIconSnippet).Assembly)
                {
                    return "ChatPlus";
                }

                var mod = ModLoader.Mods.FirstOrDefault(z => z != null && z.Code == assembly);
                if (mod == null) continue;

                if (mod.Name == "DragonLens")
                {
                    return "Terraria";
                }

                return mod.Name;
            }

            return "ModLoader";
        }
        catch
        {
            return "ModLoader";
        }
    }

}
