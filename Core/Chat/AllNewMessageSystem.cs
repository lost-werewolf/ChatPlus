using System;
using System.Collections;
using System.Reflection;
using ChatPlus.Core.Features.Scrollbar;
using ChatPlus.Core.Helpers;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace ChatPlus.Core.Chat;

internal class AllNewMessageSystem : ModSystem
{
    private Hook hookNewText;

    private delegate void NewTextStringOrig(string newText, byte R, byte G, byte B);

    public override void Load()
    {
        var method = typeof(Main).GetMethod("NewText",BindingFlags.Public | BindingFlags.Static,binder: null,
            types: [typeof(string), typeof(byte), typeof(byte), typeof(byte)],
            modifiers: null);

        hookNewText = new Hook(method,
            new Action<NewTextStringOrig, string, byte, byte, byte>((orig, text, r, g, b) =>
            {
                //AddLineCounters(text);
                orig(text, r, g, b);
            }));
    }

    private void AddLineCounters(string text)
    {
        Log.Info($"[Hook] {text}");

        var scrollSystem = ModContent.GetInstance<ChatScrollSystem>();
        var list = scrollSystem?.state?.chatScrollList;
        if (list == null) return;

        if (Main.chatMonitor is not RemadeChatMonitor monitor) return;
        if (monitor._messages.Count == 0) return;

        var container = monitor._messages[0];

        int linesInMessage = 1;
        try
        {
            var lineCountProp = container.GetType().GetProperty("LineCount");
            if (lineCountProp != null)
                linesInMessage = Math.Max(1, (int)lineCountProp.GetValue(container));
            else if (container._parsedText != null)
                linesInMessage = Math.Max(1, container._parsedText.Count);
        }
        catch { }

        int startNumber = Math.Max(1, ScrollHelper.GetTotalLineCount() - linesInMessage + 1);
        bool wasAtBottom = list.ViewPosition >= list.GetTotalHeight() - list.GetInnerDimensions().Height - 1f;

        for (int i = 0; i < linesInMessage; i++)
        {
            var counter = new UIText((startNumber + i).ToString())
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = { Pixels = Main.screenWidth - 300 },
                Height = { Pixels = 21f }
            };
            list.Add(counter);
        }

        list.Recalculate();
        if (wasAtBottom)
            list.ViewPosition = list.GetTotalHeight();
    }

    public override void Unload()
    {
        hookNewText?.Dispose();
        hookNewText = null;
    }
}
