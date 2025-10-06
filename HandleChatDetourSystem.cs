using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Chat
{
    /// <summary>
    /// This system handles detours of the chat to implement and modify some features related to the chat:
    /// 1. Handles TextSnippet inserting (such as ctrl-clicking an item into chat)
    /// </summary>
    public class HandleChatDetourSystem : ModSystem
    {
        public override void Load()
        {
            On_ChatManager.AddChatText += OverrideAddChatText;
        }

        public override void Unload()
        {
            On_ChatManager.AddChatText -= OverrideAddChatText;
        }

        private bool OverrideAddChatText(On_ChatManager.orig_AddChatText orig, ReLogic.Graphics.DynamicSpriteFont font, string text, Vector2 baseScale)
        {
            if (!orig.Invoke(font, text, baseScale)) return false;

            // Move the caret after we've gone ahead and added the text
            HandleChatSystem.caretPos += text.Length;
            return true;
        }
    }
}
