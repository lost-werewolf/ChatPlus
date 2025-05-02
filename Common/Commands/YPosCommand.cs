using LinksInChat.Common.Hooks;
using LinksInChat.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace LinksInChat.Common.Commands;

/// <summary>Move chat horizontally:  <c>/x &lt;pixels&gt;</c></summary>
public class YPosCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat;
    public override string Command => "y";         // primary name

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length > 0 && int.TryParse(args[0], out int delta))
        {
            ChatPosHelper.OffsetY += delta;
            string msg = $"Chat offset → X={ChatPosHelper.OffsetX}, " +
                         $"Y={ChatPosHelper.OffsetY}";
            Main.NewText(msg, Color.LightGreen);
            Mod.Logger.Info(msg);
        }
        else
        {
            Main.NewText("Usage: /y <pixels>", Color.Red);
        }
    }
}

