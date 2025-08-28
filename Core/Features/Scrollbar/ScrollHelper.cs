using System;
using System.Collections;
using System.IO.Pipelines;
using System.Reflection;
using ChatPlus.Core.Features.Scrollbar.UI;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace ChatPlus.Core.Features.Scrollbar;

public static class ScrollHelper
{
    public static int GetTotalLineCount()
    {
        // Reflect into Terraria's RemadeChatMonitor to get all message containers
        var monitorType = typeof(Main).Assembly.GetType("Terraria.GameContent.UI.Chat.RemadeChatMonitor");
        var messagesField = monitorType?.GetField("_messages", BindingFlags.Instance | BindingFlags.NonPublic);
        if (messagesField == null) return 0;
        var messagesList = messagesField.GetValue(Main.chatMonitor) as IEnumerable;

        // Reflect ChatMessageContainer to get line counts
        var containerType = typeof(Main).Assembly.GetType("Terraria.UI.Chat.ChatMessageContainer");
        var lineCountProp = containerType?.GetProperty("LineCount", BindingFlags.Instance | BindingFlags.Public);

        int totalLines = 0;
        if (messagesList != null)
        {
            foreach (var msg in messagesList)
            {
                if (msg == null) continue;
                if (lineCountProp != null)
                {
                    try
                    {
                        int lines = (int)lineCountProp.GetValue(msg);
                        totalLines += Math.Max(1, lines);
                    }
                    catch
                    {
                        totalLines++;
                    }
                }
                else
                {
                    // If LineCount property isn't found, count each message as 1 line
                    totalLines++;
                }
            }
        }
        return totalLines;
    }

    public static void DrawChatMonitorBackground(SpriteBatch spriteBatch)
    {
        // Draw a faint background rectangle behind the chat area for readability
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Rectangle chatBounds = new Rectangle(82, Main.screenHeight - 247, Main.screenWidth - 300 - 7, 210);
        spriteBatch.Draw(pixel, chatBounds, Color.White * 0.02f);
    }
}
