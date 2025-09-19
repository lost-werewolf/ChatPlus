using System;
using ChatPlus.Core.Chat.ChatButtons.Shared;
using ChatPlus.Core.Features.Glyphs;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.ChatButtons;

internal class GlyphButton : BaseChatButton
{
    public GlyphButton() : base(ChatButtonType.Glyphs) { }
    protected override UserInterface UI => ModContent.GetInstance<GlyphSystem>().ui;
    protected override UIState State => ModContent.GetInstance<GlyphSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => GlyphState.WasOpenedByButton = flag;
}
