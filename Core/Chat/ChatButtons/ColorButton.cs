using System;
using ChatPlus.Core.Chat.ChatButtons.Shared;
using ChatPlus.Core.Features.Colors;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.ChatButtons;

internal class ColorButton : BaseChatButton
{
    public ColorButton() : base(ChatButtonType.Colors) { }
    protected override UserInterface UI => ModContent.GetInstance<ColorSystem>().ui;
    protected override UIState State => ModContent.GetInstance<ColorSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => ColorState.WasOpenedByButton = flag;
}
