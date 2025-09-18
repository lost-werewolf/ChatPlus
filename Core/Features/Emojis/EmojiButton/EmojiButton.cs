using ChatPlus.Core.Features.Emojis;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Emojis.EmojiButton;
internal class EmojiButton : UIElement
{
    public EmojiButton()
    {
        Width.Set(24, 0f);
        Height.Set(24, 0f);
        Left.Set(190, 0f); 
        Top.Set(-38, 0f); 
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        if (!Main.drawingPlayerChat)
            return;

        var emojiSystem = ModContent.GetInstance<EmojiSystem>();
        var ui = emojiSystem.ui;
        var state = emojiSystem.state;

        if (ui.CurrentState == null)
        {
            EmojiState.WasOpenedByButton = true;
            ui.SetState(state);
        }
        else
        {
            EmojiState.WasOpenedByButton = false;
            ui.SetState(null);
        }
    }

    public override void Draw(SpriteBatch sb)
    {
        if (!Main.drawingPlayerChat)
            return;

        // Position
        Top.Set(Main.screenHeight-59, 0);
        Left.Set(Main.screenWidth - 300+24+21,0);

        // Hover draw
        var sunglassesEmoji = "[e:smiling_face_with_sunglasses/gray]";
        if (IsMouseHovering)
        {
            sunglassesEmoji = "[e:smiling_face_with_sunglasses]";
            Rectangle r = GetDimensions().ToRectangle();
            r.X -= 3;
            r.Y -= 2;
            r.Width += 6;
            r.Height += 4;
            DrawHelper.DrawPixelatedBorder(sb, r, new Color(15,15,30) * 1.0f, 2, 2);
        }

        // Draw emoji
        var pos = GetDimensions().Position() + new Vector2(2, 2);
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value,
        sunglassesEmoji, pos, Color.White, 0f, Vector2.Zero, new Vector2(1));

        // debug
        //var rect = GetDimensions().ToRectangle();
        //sb.Draw(TextureAssets.MagicPixel.Value, rect, Color.Red * 0.5f);
    }
}
