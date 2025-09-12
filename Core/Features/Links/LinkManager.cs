using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Links;

internal class LinkManager : ModSystem
{
    public override void Load()
    {
        ChatManager.Register<LinkTagHandler>(["l", "link"]);
    }
}
