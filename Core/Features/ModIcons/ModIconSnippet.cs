using System;
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

    public override void OnClick()
    {
        base.OnClick();

        var plr = Main.player[_playerIndex];
        if (plr == null || !plr.active) return;

        var state = ModInfoState.instance;
        if (state == null)
        {
            Main.NewText("Player info UI not available.", Color.Orange);
            return;
        }

        // Snapshot current chat so the info UI can restore it later
        var snap = ChatSession.Capture();                 // <- your existing helper

        state.SetPlayer(_playerIndex, plr.name);          // tell the UI which player to show
        state.SetReturnSnapshot(snap);                    // so Back can restore chat/session

        Main.drawingPlayerChat = false;                   // hide chat while the modal is open (optional)
        IngameFancyUI.OpenUIState(state);                 // open the "view more" UI
    }

    public override float GetStringLength(DynamicSpriteFont font) => BaseIconSize;
    public override Color GetVisibleColor() => Color.White;

    public override void OnHover()
    {
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
            var trace = new StackTrace();
            var frames = trace.GetFrames();
            if (frames == null) return null;
            var terrariaAssembly = typeof(Main).Assembly;
            var loaderAssembly = typeof(ModLoader).Assembly;
            var pivot = -1;
            for (int k = 0; k < frames.Length; k++)
            {
                var method = frames[k].GetMethod();
                if (method == null) continue;
                var name = method.Name;
                if (name.IndexOf("NewText", StringComparison.Ordinal) >= 0 || name.IndexOf("AddNewMessage", StringComparison.Ordinal) >= 0) { pivot = k; break; }
            }
            if (pivot < 0) return null;
            Type chosenType = null;
            for (int k = pivot + 1; k < frames.Length; k++)
            {
                var method = frames[k].GetMethod();
                if (method == null) continue;
                var type = method.DeclaringType;
                if (type == null || type.Namespace == null) continue;
                var asm = type.Assembly;
                if (asm == terrariaAssembly || asm == loaderAssembly) continue;
                chosenType = type;
                break;
            }
            if (chosenType == null) return "Terraria";
            var mod = ModLoader.Mods.FirstOrDefault(z => z.Name != "ModLoader" && z.Code == chosenType.Assembly);
            if (mod.Name == "DragonLens" || mod.Name == "CheatSheet")
                return null;
            if (mod == null) return "Terraria";
            return mod.Name;
        }
        catch
        {
            return null;
        }
    }
}
