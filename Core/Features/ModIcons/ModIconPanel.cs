using System;
using System.Collections.Generic;
using System.Linq;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace ChatPlus.Core.Features.ModIcons;

public class ModIconPanel : BasePanel<ModIcon>
{
    protected override IEnumerable<ModIcon> GetSource() => ModIconInitializer.ModIcons;
    protected override BaseElement<ModIcon> BuildElement(ModIcon data) => new ModIconElement(data);
    protected override string GetDescription(ModIcon data)
    {
        return $"{data.mod.Name}\nClick to view more";
    }
    protected override string GetTag(ModIcon data) => data.Tag;

    public override void Update(GameTime gt)
    {
        if (items.Count == 0)
            PopulatePanel();
        base.Update(gt);
    }

    public void OpenModInfoForSelectedMod()
    {
        // Need the selected ModIcon from the connected BasePanel<ModIcon>
        if (TryGetSelected(out var entry))
        {
            var mod = entry.mod;
            if (ModInfoState.Instance == null)
            {
                Main.NewText("Mod info UI not available.", Color.Orange);
                return;
            }
            string displayName = mod?.DisplayName ?? mod?.Name ?? "Unknown Mod";
            string internalName = mod?.Name ?? displayName;

            string version = mod?.Version?.ToString() ?? "Unknown";
            string description = GetDescriptionForMod(mod);

            // Populate and open ModInfoState
            var s = ModInfoState.Instance;

            // 1) Snapshot current chat
            var snap = ChatSession.Capture();

            // 2) Prepare the view-more UI
            s.SetModInfo(description, displayName, internalName);
            s.SetReturnSnapshot(snap);

            // 3) Optionally hide chat while the modal is open
            Main.drawingPlayerChat = false;

            // 4) Open it
            IngameFancyUI.OpenUIState(s);
        }
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
    }
}
