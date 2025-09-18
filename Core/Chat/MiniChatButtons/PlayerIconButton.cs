using System;
using ChatPlus.Core.Chat.MiniChatButtons.Shared;
using ChatPlus.Core.Features.PlayerIcons;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Chat.MiniChatButtons;

internal class PlayerIconButton : BaseChatButton
{
    protected override ChatButtonType Type => ChatButtonType.PlayerIcons;

    protected override UserInterface UI => ModContent.GetInstance<PlayerIconSystem>().ui;
    protected override UIState State => ModContent.GetInstance<PlayerIconSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => PlayerIconState.WasOpenedByButton = flag;

    protected override void DrawCustom(SpriteBatch sb, Vector2 pos)
    {
        var dims = GetDimensions();
        bool forceNormal = IsMouseHovering || UI.CurrentState == State;
        ChatButtonRenderer.Draw(sb, Type, dims.Position(), 24, grayscale: !forceNormal);
    }
}
