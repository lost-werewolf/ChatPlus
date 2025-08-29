using System;
using System.Collections.Generic;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Features.PlayerHeads.PlayerInfo;
using ChatPlus.Core.UI;
using Terraria;
using Terraria.UI;

namespace ChatPlus.Core.Features.PlayerHeads;

public class PlayerHeadPanel : BasePanel<PlayerHead>
{
    protected override IEnumerable<PlayerHead> GetSource() => PlayerHeadInitializer.PlayerIcons;
    protected override BaseElement<PlayerHead> BuildElement(PlayerHead data) => new PlayerHeadElement(data);
    protected override string GetDescription(PlayerHead data) => data.PlayerName + "\nClick to view more";
    protected override string GetTag(PlayerHead data) => data.Tag;

    public override void Update(Microsoft.Xna.Framework.GameTime gt)
    {
        // Refresh population each frame in case players join/leave
        if (items.Count != PlayerHeadInitializer.PlayerIcons.Count)
            PopulatePanel();

        base.Update(gt);
    }

    public void OpenPlayerInfoForSelected()
    {
        if (!TryGetSelected(out var entry)) return;

        int who = entry.PlayerIndex;
        string name = null;
        if (who < 0 || who >= Main.maxPlayers || !Main.player[who].active)
        {
            // try by name
            name = entry.PlayerName ?? "";
            who = -1;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i]?.active == true &&
                    string.Equals(Main.player[i].name, name, StringComparison.OrdinalIgnoreCase))
                { who = i; break; }
            }
            if (who < 0) { Main.NewText("Player not found.", Microsoft.Xna.Framework.Color.Orange); return; }
        }

        var s = PlayerInfoState.instance;
        if (s == null)
        {
            Main.NewText("Player info UI not available.", Microsoft.Xna.Framework.Color.Orange);
            return;
        }

        // 1) Snapshot chat
        var snap = ChatSession.Capture();

        // 2) Configure and open
        s.SetPlayer(who, Main.player[who]?.name);
        s.SetReturnSnapshot(snap);

        // Optional: hide chat while modal is open
        Main.drawingPlayerChat = false;

        IngameFancyUI.OpenUIState(s);
    }
}
