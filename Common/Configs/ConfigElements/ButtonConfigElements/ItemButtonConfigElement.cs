using ChatPlus.Common.Configs.ConfigElements.Base;
using ChatPlus.Core.Chat.MiniChatButtons.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Common.Configs.ConfigElements.ButtonConfigElements;

internal class ItemButtonConfigElement : BaseBoolConfigElement
{
    protected override void OnToggled(bool newValue)
    {
        if (MemberInfo != null && Item != null)
        {
            MemberInfo.SetValue(Item, newValue);
        }
    }

    protected override void DrawPreview(SpriteBatch sb)
    {
        var dims = GetDimensions();
        Vector2 pos = new(dims.X + 175 - 8, dims.Y + 4);
        ChatButtonRenderer.Draw(sb, ChatButtonType.Items, pos, 24, grayscale: true);
    }
}
