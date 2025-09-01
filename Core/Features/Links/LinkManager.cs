using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Links;

internal class LinkManager : ModSystem
{
    public static readonly List<LinkEntry> Links = new();

    public override void Load()
    {
        ChatManager.Register<LinkTagHandler>(["l", "link"]);

        // Example links
        Links.Clear();
        Add("Terraria Wiki", "https://terraria.wiki.gg");
        Add("tModLoader GitHub", "https://github.com/tModLoader/tModLoader");
        Add("tML Discord", "https://discord.gg/tmodloader");
        Add("Steam Workshop", "https://steamcommunity.com/app/1281930/workshop/");
        Add("ChatPlus Source", "https://github.com/emyhrberg"); // placeholder
    }

    private static void Add(string display, string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;
        Links.Add(new LinkEntry(display, url));
    }
}
