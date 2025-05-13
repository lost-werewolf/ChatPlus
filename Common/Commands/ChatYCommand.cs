using AdvancedChatFeatures.Common.Hooks;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Common.Commands;

/// <summary>Move chat horizontally:  <c>/x &lt;pixels&gt;</c></summary>
public class ChatYCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat;
    public override string Command => "chaty";         // primary name
    public override string Description => "Move chat vertically (up/down)";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length > 0 && int.TryParse(args[0], out int delta))
        {
            ChatPosHook.OffsetY = delta;
            string msg = $"Chat offset → X={ChatPosHook.OffsetX}, " +
                         $"Y={ChatPosHook.OffsetY}";
            Main.NewText(msg, Color.LightGreen);
            Mod.Logger.Info(msg);
        }
        else
        {
            Main.NewText("Usage: /y <pixels>", Color.Red);
        }
    }
}

