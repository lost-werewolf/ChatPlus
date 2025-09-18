using System;
using ChatPlus.Core.Chat.MiniChatButtons.Shared;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;
using static Terraria.Localization.NetworkText;

namespace ChatPlus.Core.Chat.MiniChatButtons;

internal class ModIconButton : BaseChatButton
{
    protected override ChatButtonType Type => ChatButtonType.ModIcons;

    protected override UserInterface UI => ModContent.GetInstance<ModIconSystem>().ui;
    protected override UIState State => ModContent.GetInstance<ModIconSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => ModIconState.WasOpenedByButton = flag;
    // Effect


    protected override void DrawCustom(SpriteBatch sb, Vector2 pos)
    {
        var dims = GetDimensions();
        bool forceNormal = IsMouseHovering || UI.CurrentState == State;
        ChatButtonRenderer.Draw(sb, Type, dims.Position(), 24, grayscale: !forceNormal);
    }
}
