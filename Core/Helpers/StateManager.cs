using System;
using ChatPlus.Common.Compat.CustomTags;
using ChatPlus.Core.Chat;
using ChatPlus.Core.Features.Colors;
using ChatPlus.Core.Features.Commands;
using ChatPlus.Core.Features.Emojis;
using ChatPlus.Core.Features.Glyphs;
using ChatPlus.Core.Features.Items;
using ChatPlus.Core.Features.Mentions;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.PlayerIcons;
using ChatPlus.Core.Features.Uploads;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace ChatPlus.Core.Helpers;
public class StateManager
{
    public CommandSystem CommandSystem { get; }
    public ColorSystem ColorSystem { get; }
    public EmojiSystem EmojiSystem { get; }
    public GlyphSystem GlyphSystem { get; }
    public ItemSystem ItemSystem { get; }
    public ModIconSystem ModIconSystem { get; }
    public MentionSystem MentionSystem { get; }
    public PlayerIconSystem PlayerIconSystem { get; }
    public UploadSystem UploadSystem { get; }
    public CustomTagSystem CustomTagSystem { get; }

    public StateManager(
        CommandSystem commandSystem,
        ColorSystem colorSystem,
        EmojiSystem emojiSystem,
        GlyphSystem glyphSystem,
        ItemSystem itemSystem,
        ModIconSystem modIconSystem,
        MentionSystem mentionSystem,
        PlayerIconSystem playerIconSystem,
        UploadSystem uploadSystem,
        CustomTagSystem customTagSystem)
    {
        CommandSystem = commandSystem;
        ColorSystem = colorSystem;
        EmojiSystem = emojiSystem;
        GlyphSystem = glyphSystem;
        ItemSystem = itemSystem;
        ModIconSystem = modIconSystem;
        MentionSystem = mentionSystem;
        PlayerIconSystem = playerIconSystem;
        UploadSystem = uploadSystem;
        CustomTagSystem = customTagSystem;
    }

    public void OpenStateByTriggers(GameTime gameTime,UserInterface ui,UIState state,params ITrigger[] triggers)
    {
        if (!Main.drawingPlayerChat)
        {
            if (ui.CurrentState != null)
                ui.SetState(null);
            return;
        }

        if ((state is EmojiState && EmojiState.WasOpenedByButton) ||
    (state is UploadState && UploadState.WasOpenedByButton) ||
    (state is ColorState && ColorState.WasOpenedByButton) ||
    (state is ItemState && ItemState.WasOpenedByButton) ||
    (state is CommandState && CommandState.WasOpenedByButton) ||
    (state is GlyphState && GlyphState.WasOpenedByButton) ||
    (state is MentionState && MentionState.WasOpenedByButton) ||
    (state is ModIconState && ModIconState.WasOpenedByButton) ||
    (state is PlayerIconState && PlayerIconState.WasOpenedByButton) ||
    (state is CustomTagState && CustomTagState.WasOpenedByButton))
        {
            if (ui.CurrentState != state)
            {
                CloseOthers(ui);
                ui.SetState(state);
                ui.CurrentState.Recalculate();
            }

            ui.Update(gameTime);
            return;
        }

        string text = Main.chatText ?? string.Empty;
        int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);

        bool shouldOpen = false;
        if (triggers != null && triggers.Length > 0)
        {
            foreach (var trigger in triggers)
            {
                if (trigger.ShouldOpen(text, caret))
                {
                    shouldOpen = true;
                    break;
                }
            }
        }
        // debug test for fullscreen map
        //if (state.GetType() == typeof(ItemState))
            //Log.Info(shouldOpen);

        if (shouldOpen)
        {
            if (ui.CurrentState != state)
            {
                CloseOthers(ui);
                ui.SetState(state);
                ui.CurrentState.Recalculate();
            }

            ui.Update(gameTime);
        }
        else
        {
            if (ui.CurrentState == state)
                ui.SetState(null);
        }
    }

    public bool IsAnyStateActive()
    {
        return CommandSystem?.ui?.CurrentState != null ||
               ColorSystem?.ui?.CurrentState != null ||
               EmojiSystem?.ui?.CurrentState != null ||
               GlyphSystem?.ui?.CurrentState != null ||
               ItemSystem?.ui?.CurrentState != null ||
               ModIconSystem?.ui?.CurrentState != null ||
               PlayerIconSystem?.ui?.CurrentState != null ||
               UploadSystem?.ui?.CurrentState != null ||
               MentionSystem?.ui?.CurrentState != null ||
               CustomTagSystem?.ui?.CurrentState != null;
    }

    public void CloseOthers(UserInterface keep)
    {
        void Close(UserInterface u)
        {
            if (u != null && u != keep && u.CurrentState != null)
                u.SetState(null);
        }

        Close(CommandSystem?.ui);
        Close(ColorSystem?.ui);
        Close(EmojiSystem?.ui);
        Close(GlyphSystem?.ui);
        Close(ItemSystem?.ui);
        Close(ModIconSystem?.ui);
        Close(PlayerIconSystem?.ui);
        Close(UploadSystem?.ui);
        Close(MentionSystem?.ui);
        Close(CustomTagSystem?.ui);
    }
    public static bool IsAnyGridPanelActive()
    {
        var sm = ChatPlus.StateManager;
        if (sm == null) return false;

        return
            sm.CommandSystem?.state?.Panel?.IsGridModeEnabled == true ||
            sm.ColorSystem?.state?.Panel?.IsGridModeEnabled == true ||
            sm.EmojiSystem?.state?.Panel?.IsGridModeEnabled == true ||
            sm.GlyphSystem?.state?.Panel?.IsGridModeEnabled == true ||
            sm.ItemSystem?.state?.Panel?.IsGridModeEnabled == true ||
            sm.ModIconSystem?.state?.Panel?.IsGridModeEnabled == true ||
            sm.PlayerIconSystem?.state?.Panel?.IsGridModeEnabled == true ||
            sm.UploadSystem?.state?.Panel?.IsGridModeEnabled == true ||
            sm.MentionSystem?.state?.Panel?.IsGridModeEnabled == true;
    }

    public void ResetAllWasOpenedByButton()
    {
        CommandState.WasOpenedByButton = false;
        ColorState.WasOpenedByButton = false;
        EmojiState.WasOpenedByButton = false;
        GlyphState.WasOpenedByButton = false;
        ItemState.WasOpenedByButton = false;
        ModIconState.WasOpenedByButton = false;
        MentionState.WasOpenedByButton = false;
        PlayerIconState.WasOpenedByButton = false;
        UploadState.WasOpenedByButton = false;
        CustomTagState.WasOpenedByButton = false;
    }
}
