﻿using System;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using static Terraria.Localization.NetworkText;

namespace ChatPlus.Core.Chat
{
    /// <summary>
    /// Multiple chat detours which implement new functionality:
    /// 1. Ctrl+A to select all text in the chat input and backspace/delete to clear it.
    /// 2. Tab to move caret to the end of the input.
    /// 3. Arrow keys (Left/Right) to move caret position and cancel selection.
    /// </summary>
    public class HandleChatSystem : ModSystem
    {
        // Variables
        private static int caretPos;
        private static bool selectAll;
        private static int selectionAnchor = -1; // -1 = no selection
        public static (int start, int end)? GetSelection()
        {
            if (selectionAnchor == -1 || caretPos == selectionAnchor) return null;
            int start = Math.Min(caretPos, selectionAnchor);
            int end = Math.Max(caretPos, selectionAnchor);
            return (start, end);
        }

        // Add holding
        private int _leftArrowHoldFrames = 0;
        private int _rightArrowHoldFrames = 0;
        private int _backspaceHoldFrames = 0;

        // Public
        public static void SetCaretPos(int to) => caretPos = to;
        public static int GetCaretPos() => caretPos;
        public static bool IsSelectAll() => selectAll;

        public override void Load()
        {
            if (ModLoader.TryGetMod("ChatImprover", out Mod _))
            {
                return;
            }

            On_Main.GetInputText += GetInputText;
            On_Main.DoUpdate_HandleChat += DoUpdate_HandleChat;
        }

        public override void Unload()
        {
            if (ModLoader.TryGetMod("ChatImprover", out Mod _))
            {
                return;
            }

            On_Main.GetInputText -= GetInputText;
            On_Main.DoUpdate_HandleChat -= DoUpdate_HandleChat;
        }

        private void DoUpdate_HandleChat(On_Main.orig_DoUpdate_HandleChat orig)
        {
            orig();

            if (!Conf.C.BetterTextEditor) return;

            if (!Main.drawingPlayerChat)
            {
                caretPos = 0;
                selectAll = false;
            }
            else if (caretPos > Main.chatText.Length)
            {
                caretPos = Main.chatText.Length;
            }

            int curLen = Main.chatText?.Length ?? 0;

            if (_armCaretAfterAltInsert && curLen > _lenBeforeAltInsert)
            {
                caretPos = curLen;
                selectAll = false;
                selectionAnchor = -1;

                PlayerInput.WritingText = true;
                Main.instance.textBlinkerCount = 0;
                Main.instance.textBlinkerState = 1;

                _armCaretAfterAltInsert = false;
                _lenBeforeAltInsert = -1;
            }
        }

        private static bool _armCaretAfterAltInsert;
        private static int _lenBeforeAltInsert = -1;

        private string GetInputText(On_Main.orig_GetInputText orig, string oldString, bool allowMultiLine = false)
        {
            //return orig(oldString, allowMultiLine);

            if (!Main.drawingPlayerChat)
            {
                return orig(oldString, allowMultiLine);
            }

            if (!Conf.C.BetterTextEditor)
                return orig(oldString, allowMultiLine);

            Main.inputTextEnter = false;
            Main.instance.HandleIME();

            HandleCharacterKeyPressed(); // Input text is entered here
            HandleLeftRightArrowKeysPressed(); // Caret navigation with left right keys

            // More functionality
            HandleClipboardKeys();
            HandleCtrlAPressed();
            HandleTabKeyPressed();
            HandleBackKeyPressed();

            // Clamp caret
            caretPos = Math.Clamp(caretPos, 0, Main.chatText.Length);

            return Main.chatText;
        }

        private void HandleClipboardKeys()
        {
            bool ctrl = Main.keyState.IsKeyDown(Keys.LeftControl) || Main.keyState.IsKeyDown(Keys.RightControl);

            if (ctrl && Main.keyState.IsKeyDown(Keys.C) && !Main.oldKeyState.IsKeyDown(Keys.C))
            {
                var sel = GetSelection();
                if (sel != null) Platform.Get<IClipboard>().Value = Main.chatText.Substring(sel.Value.start, sel.Value.end - sel.Value.start);
                else Platform.Get<IClipboard>().Value = Main.chatText;
            }

            if (ctrl && Main.keyState.IsKeyDown(Keys.X) && !Main.oldKeyState.IsKeyDown(Keys.X))
            {
                var sel = GetSelection();
                if (sel != null)
                {
                    Platform.Get<IClipboard>().Value = Main.chatText.Substring(sel.Value.start, sel.Value.end - sel.Value.start);
                    Main.chatText = Main.chatText.Remove(sel.Value.start, sel.Value.end - sel.Value.start);
                    caretPos = sel.Value.start;
                }
                else
                {
                    Platform.Get<IClipboard>().Value = Main.chatText;
                    Main.chatText = "";
                    caretPos = 0;
                }
                selectionAnchor = -1;
            }

            if (ctrl && Main.keyState.IsKeyDown(Keys.V) && !Main.oldKeyState.IsKeyDown(Keys.V))
            {
                string clip = Platform.Get<IClipboard>().Value;
                if (!string.IsNullOrEmpty(clip))
                {
                    var sel = GetSelection();
                    if (sel != null)
                    {
                        Main.chatText = Main.chatText.Remove(sel.Value.start, sel.Value.end - sel.Value.start)
                                                   .Insert(sel.Value.start, clip);
                        caretPos = sel.Value.start + clip.Length;
                        selectionAnchor = -1;
                    }
                    else
                    {
                        Main.chatText = Main.chatText.Insert(caretPos, clip);
                        caretPos += clip.Length;
                    }
                }
            }
        }

        private void HandleCtrlAPressed()
        {
            bool ctrl = Main.keyState.IsKeyDown(Keys.LeftControl) || Main.keyState.IsKeyDown(Keys.RightControl);
            if (ctrl && Main.keyState.IsKeyDown(Keys.A) && !Main.oldKeyState.IsKeyDown(Keys.A))
            {
                if (Main.chatText.Length > 0)
                {
                    selectionAnchor = 0;
                    caretPos = Main.chatText.Length;
                }
            }
        }

        private void HandleTabKeyPressed()
        {
            // Tab -> move caret to end
            if (Main.keyState.IsKeyDown(Keys.Tab) && !Main.oldKeyState.IsKeyDown(Keys.Tab))
            {
                caretPos = Main.chatText.Length;
                selectAll = false;
            }
        }

        private void HandleCharacterKeyPressed()
        {
            caretPos = Math.Clamp(caretPos, 0, Main.chatText.Length);

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

            if (typed.Length > 0)
            {
                var sel = GetSelection();
                if (sel != null)
                {
                    int start = Math.Clamp(sel.Value.start, 0, Main.chatText.Length);
                    int end = Math.Clamp(sel.Value.end, 0, Main.chatText.Length);

                    if (end > start)
                    {
                        Main.chatText = Main.chatText.Remove(start, end - start)
                                                     .Insert(start, typed);
                        caretPos = start + typed.Length;
                    }
                    else
                    {
                        // nothing valid selected → just insert
                        Main.chatText = Main.chatText.Insert(caretPos, typed);
                        caretPos += typed.Length;
                    }

                    selectionAnchor = -1;
                }
                else
                {
                    // No selection → insert at caret
                    Main.chatText = Main.chatText.Insert(caretPos, typed);
                    caretPos += typed.Length;
                    selectionAnchor = -1;
                }
            }
        }

        private void HandleLeftRightArrowKeysPressed()
        {
            bool shift = Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift);
            bool ctrl = Main.keyState.IsKeyDown(Keys.LeftControl) || Main.keyState.IsKeyDown(Keys.RightControl);

            if (Main.keyState.IsKeyDown(Keys.Left))
            {
                _leftArrowHoldFrames++;
                if (_leftArrowHoldFrames == 1 || _leftArrowHoldFrames > 35 && _leftArrowHoldFrames % 2 == 0)
                {
                    var sel = GetSelection();
                    if (sel != null && !shift)
                    {
                        // Collapse selection → move caret to start
                        caretPos = sel.Value.start;
                        selectionAnchor = -1;
                    }
                    else if (caretPos > 0)
                    {
                        int oldCaret = caretPos;
                        caretPos = ctrl ? MoveCaretWordLeft(caretPos, Main.chatText) : caretPos - 1;

                        if (shift)
                        {
                            if (selectionAnchor == -1) selectionAnchor = oldCaret;
                        }
                        else selectionAnchor = -1;
                    }
                }
            }
            else _leftArrowHoldFrames = 0;

            if (Main.keyState.IsKeyDown(Keys.Right))
            {
                _rightArrowHoldFrames++;
                if (_rightArrowHoldFrames == 1 || _rightArrowHoldFrames > 35 && _rightArrowHoldFrames % 2 == 0)
                {
                    var sel = GetSelection();
                    if (sel != null && !shift)
                    {
                        // Collapse selection → move caret to end
                        caretPos = sel.Value.end;
                        selectionAnchor = -1;
                    }
                    else if (caretPos < Main.chatText.Length)
                    {
                        int oldCaret = caretPos;
                        caretPos = ctrl ? MoveCaretWordRight(caretPos, Main.chatText) : caretPos + 1;

                        if (shift)
                        {
                            if (selectionAnchor == -1) selectionAnchor = oldCaret;
                        }
                        else selectionAnchor = -1;
                    }
                }
            }
            else _rightArrowHoldFrames = 0;
        }

        private void HandleBackKeyPressed()
        {
            if (Main.keyState.IsKeyDown(Keys.Back))
            {
                _backspaceHoldFrames++;
                if (_backspaceHoldFrames == 1 || _backspaceHoldFrames > 35 && _backspaceHoldFrames % 2 == 0)
                {
                    var sel = GetSelection();
                    if (sel != null)
                    {
                        Main.chatText = Main.chatText.Remove(sel.Value.start, sel.Value.end - sel.Value.start);
                        caretPos = sel.Value.start;
                        selectionAnchor = -1;
                    }
                    else if (caretPos > 0)
                    {
                        Main.chatText = Main.chatText.Remove(caretPos - 1, 1);
                        caretPos--;
                    }
                }
            }
            else _backspaceHoldFrames = 0;
        }

        #region helpers
        private int MoveCaretWordLeft(int pos, string text)
        {
            if (pos <= 0) return 0;
            int i = pos - 1;
            while (i > 0 && char.IsWhiteSpace(text[i])) i--;
            while (i > 0 && !char.IsWhiteSpace(text[i - 1])) i--;
            return i;
        }

        private int MoveCaretWordRight(int pos, string text)
        {
            if (pos >= text.Length) return text.Length;
            int i = pos;
            while (i < text.Length && !char.IsWhiteSpace(text[i])) i++;
            while (i < text.Length && char.IsWhiteSpace(text[i])) i++;
            return i;
        }

        #endregion
    }
}
