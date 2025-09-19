using System;
using ChatPlus.Core.Chat.ChatButtons.Shared;
using ChatPlus.Core.Features.ModIcons;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.ChatButtons;

internal class ModIconButton : BaseChatButton
{
    public ModIconButton() : base(ChatButtonType.ModIcons) { }
    protected override UserInterface UI => ModContent.GetInstance<ModIconSystem>().ui;
    protected override UIState State => ModContent.GetInstance<ModIconSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => ModIconState.WasOpenedByButton = flag;
}
