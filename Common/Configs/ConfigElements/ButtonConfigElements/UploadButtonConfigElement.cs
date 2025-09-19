using ChatPlus.Common.Configs.ConfigElements.Base;
using ChatPlus.Core.Chat.ChatButtons.Shared;
using Microsoft.Xna.Framework.Graphics;

namespace ChatPlus.Common.Configs.ConfigElements.ButtonConfigElements;

internal class UploadButtonConfigElement : BaseBoolConfigElement
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
        Vector2 pos = new(dims.X + 175 - 6, dims.Y + 2);
        ChatButtonRenderer.Draw(sb, ChatButtonType.Uploads, pos, 24, grayscale: true, preview: true);
    }
}
