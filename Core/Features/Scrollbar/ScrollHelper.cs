using System;
using System.Collections;
using System.Reflection;
using ChatPlus.Core.Features.Scrollbar.UI;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace ChatPlus.Core.Features.Scrollbar
{
    public static class ScrollHelper
    {
        public static int GetTotalLineCount()
        {
            // Get RemadeChatMonitor
            var monitorType = typeof(Main).Assembly.GetType("Terraria.GameContent.UI.Chat.RemadeChatMonitor");
            var messagesField = monitorType.GetField("_messages", BindingFlags.Instance | BindingFlags.NonPublic);
            var list = messagesField.GetValue(Main.chatMonitor) as IEnumerable;

            // Get ChatMessageContainer
            var containerType = typeof(Main).Assembly.GetType("Terraria.UI.Chat.ChatMessageContainer");
            var lineCountProp = containerType?.GetProperty("LineCount", BindingFlags.Instance | BindingFlags.Public);

            // Count total
            int total = 0;
            foreach (var msg in list)
            {
                if (msg != null)
                {
                    try
                    {
                        int lines = (int)lineCountProp.GetValue(msg);
                        total += Math.Max(1, lines);
                    }
                    catch
                    {
                        total++;
                    }
                }
            }
            return total;
        }

        public static void DebugDrawChatScrollList(SpriteBatch sb, ChatScrollList list)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            foreach (var child in list)
            {
                var dims = child.GetOuterDimensions(); // child bounds
                Rectangle r = dims.ToRectangle();

                // fill
                sb.Draw(pixel, r, Color.Red * 0.5f);

                // border lines
                sb.Draw(pixel, new Rectangle(r.Left, r.Top, r.Width, 1), Color.White);              // top
                sb.Draw(pixel, new Rectangle(r.Left, r.Bottom - 1, r.Width, 1), Color.White);       // bottom
                sb.Draw(pixel, new Rectangle(r.Left, r.Top, 1, r.Height), Color.White);             // left
                sb.Draw(pixel, new Rectangle(r.Right - 1, r.Top, 1, r.Height), Color.White);        // right

                //Log.Info($"Child {i++} rect: {r}");
            }
        }

        public static void DrawChatMonitorBackground(SpriteBatch sb)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Rectangle r = new(82, Main.screenHeight - 247, Main.screenWidth - 300-7, height: 210);
            sb.Draw(pixel, r, Color.White*0.02f);
        }
    }
}
