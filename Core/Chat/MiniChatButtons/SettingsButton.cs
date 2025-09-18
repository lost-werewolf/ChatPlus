using System;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Chat.MiniChatButtons.Shared;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.MiniChatButtons;

internal class SettingsButton : BaseChatButton
{
    protected override ChatButtonType Type => ChatButtonType.Settings;

    // This button opens the mod config instead of toggling a UI state
    protected override UserInterface UI => ModContent.GetInstance<ChatButtonsSystem>().ui; // unused
    protected override UIState State => ModContent.GetInstance<ChatButtonsSystem>().state; // unused
    protected override Action<bool> SetOpenedByButton => _ => { };
    public override void LeftClick(UIMouseEvent evt)
    {
        if (!Main.drawingPlayerChat)
            return;

        // Open this mod's config UI
        try
        {
            Conf.C?.Open(null, null, true, true);
        }
        catch
        {
            // ignore; Open may throw if UI not ready
        }
    }

    protected override void DrawCustom(SpriteBatch sb, Vector2 pos)
    {
        var dims = GetDimensions();
        bool forceNormal = IsMouseHovering || UI.CurrentState == State; // maintain hover-to-color behavior
        ChatButtonRenderer.Draw(sb, Type, dims.Position(), 24, grayscale: !forceNormal);
    }
}
