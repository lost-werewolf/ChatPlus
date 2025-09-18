using System;
using System.Collections.Generic;
using System.Reflection;
using ChatPlus.Core.Chat.MiniChatButtons.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.GameContent.UI.Chat;
using ChatPlus.Core.Helpers;

namespace ChatPlus.Common.Configs.ConfigElements;

public class ChatBoxPreviewElement : ConfigElement
{
    private readonly ChatButtonType[] orderRightToLeft =
    [
        ChatButtonType.Emojis,
        ChatButtonType.Uploads,
        ChatButtonType.Mentions,
        ChatButtonType.Items,
        ChatButtonType.Glyphs,
        ChatButtonType.Colors,
        ChatButtonType.Commands,
        ChatButtonType.ModIcons,
        ChatButtonType.PlayerIcons,
        ChatButtonType.Settings,
    ];
    private static readonly Lazy<Asset<Texture2D>> settingsIcon =
        new(() => ModContent.Request<Texture2D>("ChatPlus/Assets/LastUpdatedIcon", AssetRequestMode.ImmediateLoad));

    private static readonly Lazy<Asset<Texture2D>> colorIcon =
        new(() => ModContent.Request<Texture2D>("ChatPlus/Assets/ButtonColor", AssetRequestMode.ImmediateLoad));

    private static readonly Lazy<Asset<Texture2D>> uploadIcon =
        new(() => ModContent.Request<Texture2D>("ChatPlus/Assets/ButtonUpload", AssetRequestMode.ImmediateLoad));

    public override void OnBind()
    {
        base.OnBind();
        Width = StyleDimension.Fill;
        HAlign = 0.5f;
        VAlign = 0.5f;
        MinHeight = new StyleDimension(30, 0);
        Label = string.Empty;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        var types = GetEnabledOrderLive();
        if (types.Count == 0)
        {
            return;
        }

        var dims = GetDimensions();
        float size = 24f;
        float gap = 6f;

        float baseX = dims.X + dims.Width - size - 4;
        float y = dims.Y + dims.Height - size - 2f;

        for (int i = 0; i < types.Count; i++)
        {
            float x = baseX - i * (size + gap);
            ChatButtonRenderer.Draw(spriteBatch, types[i], new Vector2(x, y), (int)size, grayscale: false);
        }
    }

    private List<ChatButtonType> GetEnabledOrderLive()
    {
        var list = new List<ChatButtonType>(orderRightToLeft.Length);
        foreach (var type in orderRightToLeft)
        {
            if (IsEnabledLive(type))
            {
                list.Add(type);
            }
        }
        return list;
    }

    private bool IsEnabledLive(ChatButtonType kind)
    {
        if (Item == null)
        {
            return true;
        }

        string fieldName = kind switch
        {
            ChatButtonType.Commands => "ShowCommandButton",
            ChatButtonType.Colors => "ShowColorButton",
            ChatButtonType.Emojis => "ShowEmojiButton",
            ChatButtonType.Glyphs => "ShowGlyphButton",
            ChatButtonType.Items => "ShowItemButton",
            ChatButtonType.ModIcons => "ShowModIconButton",
            ChatButtonType.Mentions => "ShowMentionButton",
            ChatButtonType.PlayerIcons => "ShowPlayerIconButton",
            ChatButtonType.Uploads => "ShowUploadButton",
            ChatButtonType.Settings => "ShowSettingsButton",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(fieldName))
        {
            return true;
        }

        var type = Item.GetType();

        var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
        {
            var value = field.GetValue(Item);
            if (value is bool b1)
            {
                if (value is null) Log.Info("null1");
                return b1;
            }
        }

        var prop = type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop != null)
        {
            var value = prop.GetValue(Item, null);
            if (value is bool b2)
            {
                if (value is null) Log.Info("null2");
                return b2;
            }
        }

        return true;
    }
}
