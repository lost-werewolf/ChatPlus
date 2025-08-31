using ChatPlus.Core.Features.Emojis;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Links;
internal class LinkInitializer : ModSystem
{
    public override void Load()
    {
        ChatManager.Register<LinkTagHandler>(["l", "link"]);
    }
}
