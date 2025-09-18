using ChatPlus.Core.Features.Emojis;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.UI;
internal class EmojiButton : UIElement
{
    public EmojiButton()
    {

    }

    public override void LeftClick(UIMouseEvent evt)
    {
        EmojiState.WasOpenedByButton = true;
        var ui = ModContent.GetInstance<EmojiSystem>().ui;
        var state = ModContent.GetInstance<EmojiSystem>().state;
        if (ui.CurrentState == null) ui.SetState(state);
        else ui.SetState(null);

    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}
