using System.Collections.Generic;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.ItemWindow
{
    internal class ItemInitializer : ModSystem
    {
        public static List<Item> Items { get; private set; } = [];

        public override void PostSetupContent()
        {
            Items.Clear();

            // Iterate vanilla items; modded items can be added if desired
            for (int type = 1; type < ItemLoader.ItemCount; type++)
            {
                try
                {
                    var item = new Terraria.Item();
                    item.SetDefaults(type);

                    // Skip unobtainable or placeholder
                    if (item == null || string.IsNullOrWhiteSpace(item.Name))
                        continue;

                    string name = item.Name;
                    string tag = $"[i:{type}]"; // chat tag shows icon
                    string tooltip = item.ToolTip?.ToString() ?? string.Empty;

                    Items.Add(new Item(tag, name, tooltip));
                }
                catch { /* ignore bad item types */ }
            }

            Items.Sort((a, b) => string.Compare(a.Name, b.Name, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
