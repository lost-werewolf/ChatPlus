using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ChatPlus.Common.ModCommands
{
    public class TagsCommand : ModCommand
    {
        public static LocalizedText UsageText { get; private set; }
        public static LocalizedText DescriptionText { get; private set; }
        public static LocalizedText HeaderText { get; private set; }
        public static LocalizedText[] TagTexts { get; private set; }

        public override void SetStaticDefaults()
        {
            string key = $"Commands.{nameof(TagsCommand)}.";

            UsageText = Mod.GetLocalization($"{key}Usage");
            DescriptionText = Mod.GetLocalization($"{key}Description");
            HeaderText = Mod.GetLocalization($"{key}TagsHeader");

            TagTexts = Enumerable.Range(0, 9).Select(i => Mod.GetLocalization($"{key}Tag_{i}")).ToArray();
        }

        public override CommandType Type => CommandType.Chat;
        public override string Command => "tags";
        public override string Usage => UsageText.Value;

        public override string Description => DescriptionText.Value;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Header
            Main.NewText(HeaderText.Format("[c/fff014:", "]"));

            // Each tag line
            foreach (var tagText in TagTexts)
            {
                Main.NewText(tagText.Value);
            }
        }
    }
}
