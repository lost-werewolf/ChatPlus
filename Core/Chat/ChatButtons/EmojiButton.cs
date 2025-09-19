using System;
using ChatPlus.Core.Chat.ChatButtons.Shared;
using ChatPlus.Core.Features.Emojis;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.ChatButtons;

internal class EmojiButton : BaseChatButton
{
    public EmojiButton() : base(ChatButtonType.Emojis) { }
    protected override UserInterface UI => ModContent.GetInstance<EmojiSystem>().ui;
    protected override UIState State => ModContent.GetInstance<EmojiSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => EmojiState.WasOpenedByButton = flag;
}
