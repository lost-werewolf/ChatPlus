using System;
using ChatPlus.Core.Chat.MiniChatButtons.Shared;
using ChatPlus.Core.Features.Colors;
using ChatPlus.Core.Features.Commands;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.MiniChatButtons;

internal class ColorButton : BaseChatButton
{
    protected override ChatButtonType Type => ChatButtonType.Colors;
    protected override UserInterface UI => ModContent.GetInstance<ColorSystem>().ui;
    protected override UIState State => ModContent.GetInstance<ColorSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => ColorState.WasOpenedByButton = flag;

    protected override void DrawCustom(SpriteBatch sb, Vector2 pos)
    {
        var dims = GetDimensions();
        bool forceNormal = IsMouseHovering || UI.CurrentState == State;
        ChatButtonRenderer.Draw(sb, Type, dims.Position(), 24, grayscale: !forceNormal);
    }

}
