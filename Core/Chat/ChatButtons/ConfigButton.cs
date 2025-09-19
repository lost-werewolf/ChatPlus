using System;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Chat.ChatButtons.Shared;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.ChatButtons;

internal class ConfigButton : BaseChatButton
{
    public ConfigButton() : base(ChatButtonType.Config) { }

    // This button opens the mod config instead of toggling a UI state
    protected override UserInterface UI => ModContent.GetInstance<ChatButtonsSystem>().ui; // unused
    protected override UIState State => ModContent.GetInstance<ChatButtonsSystem>().state; // unused
    protected override Action<bool> SetOpenedByButton => _ => { };
    public override void LeftClick(UIMouseEvent evt)
    {
        if (!Main.drawingPlayerChat)
            return;

        if (Main.mapFullscreen)
        {
            Main.mapFullscreen = false;
        }

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
}
