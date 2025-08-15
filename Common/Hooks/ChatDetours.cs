using System;
using System.Linq;
using System.Text;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Common.Hooks
{
    /// <summary>
    /// Multiple chat detours which implement new functionality:
    /// 1. Ctrl+A to select all text in the chat input and backspace/delete to clear it.
    /// 2. Tab to move caret to the end of the input.
    /// 3. Arrow keys (Left/Right) to move caret position and cancel selection.
    /// </summary>
    public class ChatDetours : ModSystem
    {
        private static int caretPos;
        private static bool selectAll; // Ctrl+A state

        public override void Load()
        {
            On_Main.GetInputText += GetInputText;
            On_Main.DrawPlayerChat += DrawPlayerChat;
            On_Main.DoUpdate_HandleChat += DoUpdate_HandleChat;
        }

        public override void Unload()
        {
            On_Main.GetInputText -= GetInputText;
            On_Main.DrawPlayerChat -= DrawPlayerChat;
            On_Main.DoUpdate_HandleChat -= DoUpdate_HandleChat;
        }

        private void DoUpdate_HandleChat(On_Main.orig_DoUpdate_HandleChat orig)
        {
            orig();
            if (!Main.drawingPlayerChat) { caretPos = 0; selectAll = false; }
            else if (caretPos > Main.chatText.Length) caretPos = Main.chatText.Length;
        }

        private string GetInputText(On_Main.orig_GetInputText orig, string oldString, bool allowMultiLine = false)
        {
            if (!Main.drawingPlayerChat) return orig(oldString, allowMultiLine);

            Main.inputTextEnter = false;
            Main.inputTextEscape = false;

            string text = oldString ?? "";
            var old = Main.oldInputText;
            var cur = Main.inputText;

            bool ctrl = cur.IsKeyDown(Keys.LeftControl) || cur.IsKeyDown(Keys.RightControl);

            // Ctrl+A -> select all (highlight everything, caret at end)
            if (ctrl && cur.IsKeyDown(Keys.A) && !old.IsKeyDown(Keys.A))
            {
                selectAll = text.Length > 0;
                caretPos = text.Length;
            }

            // Tab -> move caret to end (optional but handy)
            if (cur.IsKeyDown(Keys.Tab) && !old.IsKeyDown(Keys.Tab))
            {
                caretPos = text.Length;
                selectAll = false;
            }

            caretPos = Math.Clamp(caretPos, 0, text.Length);

            // Arrow keys move caret and cancel selection
            if (cur.IsKeyDown(Keys.Left) && !old.IsKeyDown(Keys.Left) && caretPos > 0) { caretPos--; selectAll = false; }
            if (cur.IsKeyDown(Keys.Right) && !old.IsKeyDown(Keys.Right) && caretPos < text.Length) { caretPos++; selectAll = false; }

            // Gather typed characters
            string typed = "";
            for (int i = 0; i < Main.keyCount; i++)
            {
                int n = Main.keyInt[i];
                string s = Main.keyString[i];
                if (n == 13) Main.inputTextEnter = true;
                else if (n == 27) Main.inputTextEscape = true;
                else if (n >= 32 && n != 127) typed += s;
            }
            Main.keyCount = 0;

            // Typing while selectAll: replace entire text
            if (typed.Length > 0)
            {
                if (selectAll)
                {
                    text = typed;
                    caretPos = typed.Length;
                    selectAll = false;
                }
                else
                {
                    text = text.Insert(caretPos, typed);
                    caretPos += typed.Length;
                }
            }

            // Backspace: if selectAll => clear all; else delete char before caret
            if (cur.IsKeyDown(Keys.Back) && !old.IsKeyDown(Keys.Back))
            {
                if (selectAll)
                {
                    text = "";
                    caretPos = 0;
                    selectAll = false;
                }
                else if (caretPos > 0)
                {
                    text = text.Remove(caretPos - 1, 1);
                    caretPos--;
                }
            }

            // Delete (optional): cancel selection and delete at caret
            if (cur.IsKeyDown(Keys.Delete) && !old.IsKeyDown(Keys.Delete))
            {
                if (selectAll)
                {
                    text = "";
                    caretPos = 0;
                    selectAll = false;
                }
                else if (caretPos < text.Length)
                {
                    text = text.Remove(caretPos, 1);
                }
            }

            Main.oldInputText = Main.inputText;
            Main.inputText = Keyboard.GetState();
            caretPos = Math.Clamp(caretPos, 0, text.Length);
            return text;
        }

        private void DrawPlayerChat(On_Main.orig_DrawPlayerChat orig, Main self)
        {
            if (Main.drawingPlayerChat) PlayerInput.WritingText = true;
            Main.instance.HandleIME();

            TextSnippet[] arr = null;
            if (Main.drawingPlayerChat)
            {
                Main.instance.textBlinkerCount++;
                if (Main.instance.textBlinkerCount >= 20)
                {
                    Main.instance.textBlinkerState = Main.instance.textBlinkerState == 0 ? 1 : 0;
                    Main.instance.textBlinkerCount = 0;
                }

                int lineCount = Math.Max(1, (Main.chatText?.Count(c => c == '\n') ?? 0) + 1);
                int width = Main.screenWidth - 300;
                int height = lineCount * 28;
                int startX = 78;
                int startY = Main.screenHeight - 6 - height;

                DrawHelper.DrawNineSlice(startX, startY, width, height, TextureAssets.TextBack.Value, new Color(100, 100, 100, 100));

                // If Ctrl+A is active, draw a blue selection behind the whole text
                if (selectAll && !string.IsNullOrEmpty(Main.chatText))
                {
                    Vector2 typedSize = FontAssets.MouseText.Value.MeasureString(Main.chatText);

                    Rectangle selRect = new Rectangle(
                        x: 88,
                        y: Main.screenHeight - height,
                        width: (int)Math.Ceiling(typedSize.X) + 4,
                        height: 20 // matches chat line height visually
                    );

                    Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, selRect, ColorHelper.Blue*0.5f);
                }

                var sb = new StringBuilder(Main.chatText ?? "");
                int pos = Math.Clamp(caretPos, 0, sb.Length);
                sb.Insert(pos, Main.instance.textBlinkerState == 1 ? "|" : " ");
                var list = ChatManager.ParseMessage(sb.ToString(), Color.White);
                arr = list.ToArray();

                ChatManager.DrawColorCodedStringWithShadow(
                    Main.spriteBatch,
                    FontAssets.MouseText.Value,
                    arr,
                    new Vector2(88f, Main.screenHeight - height),
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    out _
                );
            }

            Main.chatMonitor.DrawChat(Main.drawingPlayerChat);

            if (Main.drawingPlayerChat && arr != null)
            {
                Vector2 sz = ChatManager.GetStringSize(FontAssets.MouseText.Value, arr, Vector2.Zero);
                Main.instance.DrawWindowsIMEPanel(new Vector2(88f, Main.screenHeight - 30) + new Vector2(sz.X + 10f, -6f));
            }
        }
    }
}
