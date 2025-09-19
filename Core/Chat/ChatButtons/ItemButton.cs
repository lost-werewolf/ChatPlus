using System;
using ChatPlus.Core.Chat.ChatButtons.Shared;
using ChatPlus.Core.Features.Items;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.ChatButtons;

internal class ItemButton : BaseChatButton
{
    public ItemButton() : base(ChatButtonType.Items) { }

    protected override UserInterface UI => ModContent.GetInstance<ItemSystem>().ui;
    protected override UIState State => ModContent.GetInstance<ItemSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => ItemState.WasOpenedByButton = flag;
}
