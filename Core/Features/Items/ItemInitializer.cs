using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.Items
{
    internal class ItemInitializer : ModSystem
    {
        public static List<Item> Items { get; private set; } = [];

        public override void PostSetupContent()
        {
            InitializeItems();
        }

        private void InitializeItems()
        {
            Items.Clear();

            var payload = new List<Item>(ItemLoader.ItemCount);

            for (int i = 1; i < ItemLoader.ItemCount; i++)
            {
                try
                {
                    Terraria.Item t = new Terraria.Item();
                    t.SetDefaults(i);

                    // Skip unobtainable/placeholder
                    if (t == null || string.IsNullOrWhiteSpace(t.Name))
                        continue;

                    string tag = $"[i:{i}]"; // chat icon tag
                    payload.Add(new Item(
                        ID: i,
                        Tag: tag,
                        DisplayName: t.Name
                    ));
                }
                catch
                {
                    // ignore bad item types
                }
            }

            payload.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));

            Items.AddRange(payload);
        }
    }
}
