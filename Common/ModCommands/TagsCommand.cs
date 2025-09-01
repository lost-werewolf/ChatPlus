using System.Reflection;
using ChatPlus.Core.Features.Scrollbar;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;

namespace ChatPlus.Common.ModCommands
{
    public class TagsCommand : ModCommand
    {
        public override string Command => "tags";
        public override string Description => "Shows all tags available in Chat+.";
        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            SendTagsToMainNewText();
        }

        private void SendTagsToMainNewText()
        {
            Main.NewText("[c/fff014:Tags:]");

            Main.NewText("Commands   /");
            Main.NewText("Colors        [c");
            Main.NewText("Emojis        [e");
            Main.NewText("Glyphs        [g");
            Main.NewText("Items          [i");
            Main.NewText("Links          [l");
            Main.NewText("Mods          [m");
            Main.NewText("Players        [p");
            Main.NewText("Uploads       [u");
        }
    }
}
