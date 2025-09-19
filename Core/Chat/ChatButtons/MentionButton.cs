using System;
using ChatPlus.Core.Chat.ChatButtons.Shared;
using ChatPlus.Core.Features.Mentions;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.ChatButtons;

internal class MentionButton : BaseChatButton
{
    public MentionButton() : base(ChatButtonType.Mentions) { }
    protected override UserInterface UI => ModContent.GetInstance<MentionSystem>().ui;
    protected override UIState State => ModContent.GetInstance<MentionSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => MentionState.WasOpenedByButton = flag;
}
