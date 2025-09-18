using System;
using ChatPlus.Core.Chat.MiniChatButtons.Shared;
using ChatPlus.Core.Features.Mentions;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Chat.MiniChatButtons;

internal class MentionButton : BaseChatButton
{
    protected override ChatButtonType Type => ChatButtonType.Mentions;

    protected override UserInterface UI => ModContent.GetInstance<MentionSystem>().ui;
    protected override UIState State => ModContent.GetInstance<MentionSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => MentionState.WasOpenedByButton = flag;


    protected override void DrawCustom(SpriteBatch sb, Vector2 pos)
    {
        if (!Main.drawingPlayerChat)
            return;

        var position = ChatButtonLayout.ComputeTopLeft(Type);
        Left.Set(position.X, 0f);
        Top.Set(position.Y, 0f);

        if (!ChatButtonLayout.IsEnabled(Type))
            return;

        if (IsMouseHovering)
        {
            Main.LocalPlayer.mouseInterface = true;

            var r = GetDimensions().ToRectangle();
            r.X -= 2;
            DrawHelper.DrawPixelatedBorder(sb, r, new Color(15, 15, 30), 2, 2);
        }

        var dims = GetDimensions();
        bool forceNormal = IsMouseHovering || UI.CurrentState == State;
        ChatButtonRenderer.Draw(sb, Type, dims.Position(), 24, grayscale: !forceNormal);
    }
}
