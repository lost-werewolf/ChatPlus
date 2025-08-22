using System.Collections.Generic;
using System.Linq;
using AdvancedChatFeatures.Helpers;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.ItemHandler
{
    internal class ItemInitializer : ModSystem
    {
        public static List<Item> Items { get; private set; } = [];

        public override void PostSetupContent()
        {
            Items.Clear();

            for (int i = 1; i < ItemLoader.ItemCount; i++)
            {
                try
                {
                    Terraria.Item item = new Terraria.Item();
                    item.SetDefaults(i);

                    // Skip unobtainable or placeholder
                    if (item == null || string.IsNullOrWhiteSpace(item.Name))
                        continue;

                    string name = item.Name;
                    string prefix = "[i";
                    string tag = $"{prefix}:{i}]"; // chat tag shows icon

                    Items.Add(new Item(
                        ID: i,
                        Tag: tag,
                        DisplayName: name
                    ));
                }
                catch { /* ignore bad item types */ }
            }

            Items.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
