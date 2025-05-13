using AdvancedChatFeatures.Common.Hooks;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Common.Commands;

/// <summary>Move chat horizontally:  <c>/x &lt;pixels&gt;</c></summary>
public class XPosCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat;
    public override string Command => "x";         // primary name

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length > 0 && int.TryParse(args[0], out int delta))
        {
            ChatPosHook.OffsetX += delta;
            string msg = $"Chat offset → X={ChatPosHook.OffsetX}, " +
                         $"Y={ChatPosHook.OffsetY}";
            Main.NewText(msg, Color.LightGreen);
            Mod.Logger.Info(msg);
        }
        else
        {
            Main.NewText("Usage: /x <pixels>", Color.Red);
        }
    }
}

