using System;
using System.Collections.Generic;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Features.PlayerIcons
.PlayerInfo;
using ChatPlus.Core.UI;
using Terraria;
using Terraria.UI;

namespace ChatPlus.Core.Features.PlayerIcons
;

public class PlayerIconPanel : BasePanel<PlayerIcon>
{
    protected override IEnumerable<PlayerIcon> GetSource() => PlayerIconManager.PlayerIcons;
    protected override BaseElement<PlayerIcon> BuildElement(PlayerIcon data) => new PlayerIconElement(data);
    protected override string GetDescription(PlayerIcon data) => data.PlayerName + "\nClick to view more";
    protected override string GetTag(PlayerIcon data) => data.Tag;

    public override void Update(Microsoft.Xna.Framework.GameTime gt)
    {
        // Refresh population each frame in case players join/leave
        if (items.Count != PlayerIconManager.PlayerIcons.Count)
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
