﻿using ChatPlus.Common.Configs.ConfigElements.Base;
using ChatPlus.Core.Chat.ChatButtons.Shared;
using Microsoft.Xna.Framework.Graphics;

namespace ChatPlus.Common.Configs.ConfigElements.ButtonConfigElements;
internal class ViewmodeButtonConfigElement : BaseBoolConfigElement
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
        Vector2 pos = new(dims.X + 175 - 7, dims.Y + 3);

        ChatButtonRenderer.Draw(sb, ChatButtonType.Viewmode, pos, 24, grayscale: true, preview: true);
    }
}

