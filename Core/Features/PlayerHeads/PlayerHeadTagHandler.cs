using System;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.PlayerHeads;

/// <summary>
/// Parses [p:PlayerName] into a PlayerHeadSnippet showing the player's head.
/// </summary>
public sealed class PlayerHeadTagHandler : ITagHandler
{
    public static string GenerateTag(string name) => PlayerHeadInitializer.GenerateTag(name);

    TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
    {
        // text contains name portion after prefix
        string name = (text ?? string.Empty).Trim();

        if (!PlayerHeadInitializer.TryGetIndex(name, out int idx))
        {
            // try case-insensitive match over current players
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var plr = Main.player[i];
                if (plr?.active == true && string.Equals(plr.name, name, StringComparison.OrdinalIgnoreCase))
                {
                    idx = i;
                    break;
                }
            }
        }

        if (idx < 0 || idx >= Main.maxPlayers)
            return new TextSnippet(name);

        return new PlayerHeadSnippet(idx)
        {
            Text = GenerateTag(name)
        };
    }
}
