using System;
using ChatPlus.Core.Chat.ChatButtons.Shared;
using ChatPlus.Core.Features.Uploads;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.ChatButtons;

internal class UploadButton : BaseChatButton
{
    // Overrides
    public UploadButton() : base(ChatButtonType.Uploads) { }
    protected override UserInterface UI => ModContent.GetInstance<UploadSystem>().ui;
    protected override UIState State => ModContent.GetInstance<UploadSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => UploadState.WasOpenedByButton = flag;
}
