using System;
using ChatPlus.Core.Chat.MiniChatButtons.Shared;
using ChatPlus.Core.Features.Emojis;
using ChatPlus.Core.Features.Uploads;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Chat.MiniChatButtons;

internal class EmojiButton : BaseChatButton
{
    protected override ChatButtonType Type => ChatButtonType.Emojis;

    protected override UserInterface UI => ModContent.GetInstance<EmojiSystem>().ui;
    protected override UIState State => ModContent.GetInstance<EmojiSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => EmojiState.WasOpenedByButton = flag;

    private string currentEmoji = "[e:smiling_face_with_sunglasses]";
    private Random rng = new Random();
    public override void MouseOut(UIMouseEvent evt)
    {
        base.MouseOut(evt);

        //PickRandomEmoji();
    }

    private void PickRandomEmoji()
    {
        var faceEmojis = EmojiManager.FindEmojis("face");

        if (EmojiManager.Emojis == null || EmojiManager.Emojis.Count == 0)
        {
            currentEmoji = "[e:smiling_face_with_sunglasses]";
            return;
        }

        var pick = EmojiManager.Emojis[rng.Next(EmojiManager.Emojis.Count)];
        currentEmoji = pick.Tag;
    }

    protected override void DrawCustom(SpriteBatch sb, Vector2 pos)
    {
        // If not hovering and state not open, show grayscale variant

        bool forceNormal = IsMouseHovering || UI.CurrentState == State;
        var dims = GetDimensions();
        ChatButtonRenderer.Draw(sb, Type, dims.Position(), 24, grayscale: !forceNormal);
    }
}
