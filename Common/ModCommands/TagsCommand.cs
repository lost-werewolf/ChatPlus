using System.Reflection;
using ChatPlus.Core.Features.Scrollbar;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.Localization;
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
            Main.NewText(Language.GetTextValue("Mods.ChatPlus.TagsHeader", "[c/fff014:Tags:]"));

            Main.NewText(Language.GetTextValue("Mods.ChatPlus.TagCommands", "/   Commands"));
            Main.NewText(Language.GetTextValue("Mods.ChatPlus.TagColors", "[c   Colors"));
            Main.NewText(Language.GetTextValue("Mods.ChatPlus.TagEmojis", "[e   Emojis"));
            Main.NewText(Language.GetTextValue("Mods.ChatPlus.TagGlyphs", "[g   Glyphs"));
            Main.NewText(Language.GetTextValue("Mods.ChatPlus.TagItems", "[i   Items"));
            Main.NewText(Language.GetTextValue("Mods.ChatPlus.TagLinks", "[l   Links"));
            Main.NewText(Language.GetTextValue("Mods.ChatPlus.TagMods", "[m   Mods"));
            Main.NewText(Language.GetTextValue("Mods.ChatPlus.TagPlayers", "[p   Players"));
            Main.NewText(Language.GetTextValue("Mods.ChatPlus.TagUploads", "[u   Uploads"));
        }
    }
}
