// using System;
// using AdvancedChatFeatures.Helpers;                               // hook helpers
// using Microsoft.Xna.Framework.Input;   // Keys
// using Terraria;
// using Terraria.ModLoader;

// namespace AdvancedChatFeatures.Common.Hooks;

// /// <summary>
// /// Minimal caret‑and‑insert patch for the chat input box.
// /// </summary>
// public sealed class InsertCaretHook : ModSystem
// {
//     /// Current insertion point (0 … chatText.Length)
//     private static int _caret;

//     // ─────────────────────────────────────────────────────────────── //
//     //  Install / remove the hook
//     // ─────────────────────────────────────────────────────────────── //
//     public override void Load()
//     {
//         On_Main.GetInputText += InterceptInput;
//         Log.Info("[InsertCaretHook] attached");
//     }

//     public override void Unload()
//     {
//         On_Main.GetInputText -= InterceptInput;
//     }

//     // ─────────────────────────────────────────────────────────────── //
//     //  Replacement for Main.GetInputText
//     // ─────────────────────────────────────────────────────────────── //
//     private string InterceptInput(On_Main.orig_GetInputText orig,
//                                   string oldString, bool allowMultiLine = false)
//     {
//         // If chat is closed, let vanilla run and reset caret
//         if (!Main.drawingPlayerChat)
//         {
//             _caret = 0;
//             return orig(oldString, allowMultiLine);
//         }

//         string text = oldString ?? string.Empty;

//         // Make sure the caret is always inside the current text
//         _caret = Math.Clamp(_caret, 0, text.Length);

//         // ── 1) freshly typed printable characters ───────────────── //
//         string inserted = "";
//         for (int i = 0; i < Main.keyCount; i++)
//         {
//             int scanCode = Main.keyInt[i];
//             string glyph = Main.keyString[i];

//             // printable ASCII except DEL(127) and CR(13)
//             if (scanCode >= 32 && scanCode != 127 && scanCode != 13)
//                 inserted += glyph;
//         }
//         Main.keyCount = 0;                       // we consumed them

//         // ── 2) caret movement with single *presses* of ← / → ────── //
//         bool leftPressed = Main.inputText.IsKeyDown(Keys.Left)
//                           && !Main.oldInputText.IsKeyDown(Keys.Left);

//         bool rightPressed = Main.inputText.IsKeyDown(Keys.Right)
//                           && !Main.oldInputText.IsKeyDown(Keys.Right);

//         if (leftPressed && _caret > 0)
//         {
//             _caret--;
//             Log.Info($"[InsertCaretHook] caret ← {_caret}");
//         }
//         else if (rightPressed && _caret < text.Length)
//         {
//             _caret++;
//             Log.Info($"[InsertCaretHook] caret → {_caret}");
//         }

//         // ── 3) apply new characters at the caret ────────────────── //
//         if (inserted.Length > 0)
//         {
//             text = text.Insert(_caret, inserted);
//             Log.Info($"[InsertCaretHook] inserted \"{inserted}\" at {_caret}");
//             _caret += inserted.Length;
//         }

//         // ── 4) let vanilla handle Backspace, Enter, Esc, IME, … ── //
//         //     (we pass the text we've already modified)
//         string result = orig(text, allowMultiLine);

//         // Keep the caret valid even if vanilla changed the string
//         _caret = Math.Clamp(_caret, 0, result.Length);

//         // Update key state for the next frame, just like vanilla does
//         Main.oldInputText = Main.inputText;
//         Main.inputText = Keyboard.GetState();

//         // If the message was sent (Enter) the caret should reset
//         if (Main.inputTextEnter)
//             _caret = 0;

//         return result;
//     }
// }
