using System.Collections.Generic;
using ChatPlus.Common.Configs;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.MiniChatButtons.Shared;

/// <summary>
/// Central system that manages all chat buttons (emoji, uploads, glyphs, etc).
/// </summary>
[Autoload(Side = ModSide.Client)]
public class ChatButtonsSystem : ModSystem
{
    public UserInterface ui;
    public UIState state;

    public override void OnWorldLoad()
    {
        ui = new UserInterface();
        state = new UIState();

        var cfg = Conf.C;

        if (cfg?.ShowEmojiButton ?? true) state.Append(new EmojiButton());
        if (cfg?.ShowUploadButton ?? true) state.Append(new UploadButton());
        if (cfg?.ShowColorButton ?? true) state.Append(new ColorButton());
        if (cfg?.ShowCommandButton ?? true) state.Append(new CommandButton());
        if (cfg?.ShowGlyphButton ?? true) state.Append(new GlyphButton());
        if (cfg?.ShowItemButton ?? true) state.Append(new ItemButton());
        if (cfg?.ShowMentionButton ?? true) state.Append(new MentionButton());
        if (cfg?.ShowSettingsButton ?? true) state.Append(new SettingsButton());
        if (cfg?.ShowModIconButton ?? true) state.Append(new ModIconButton());
        if (cfg?.ShowPlayerIconButton ?? true) state.Append(new PlayerIconButton());

        ui.SetState(state);
    }

    public override void Unload()
    {
        ui = null;
        state = null;
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            "ChatPlus: Chat Buttons",
            () =>
            {
                if (Main.drawingPlayerChat) // only when chat is open
                {
                    ui?.Update(Main._drawInterfaceGameTime);
                    ui?.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
                }
                return true;
            },
            InterfaceScaleType.UI
        ));
    }
}
