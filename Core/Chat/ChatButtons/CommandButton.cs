using System;
using ChatPlus.Core.Chat.ChatButtons.Shared;
using ChatPlus.Core.Features.Commands;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.ChatButtons;

internal class CommandButton : BaseChatButton
{
    public CommandButton() : base(ChatButtonType.Commands) { }
    protected override UserInterface UI => ModContent.GetInstance<CommandSystem>().ui;
    protected override UIState State => ModContent.GetInstance<CommandSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => CommandState.WasOpenedByButton = flag;
}
