using System;
using ChatPlus.Core.Chat.ChatButtons.Shared;
using ChatPlus.Core.Features.PlayerIcons;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.ChatButtons;

internal class PlayerIconButton : BaseChatButton
{
    public PlayerIconButton() : base(ChatButtonType.PlayerIcons) { }
    protected override UserInterface UI => ModContent.GetInstance<PlayerIconSystem>().ui;
    protected override UIState State => ModContent.GetInstance<PlayerIconSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => PlayerIconState.WasOpenedByButton = flag;
}
