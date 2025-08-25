using System;
using System.Collections.Generic;
using ChatPlus.ColorHandler;
using Terraria;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.Colors
{
    internal class ColorInitializer : ModSystem
    {
        public static List<ColorItem> Colors { get; private set; } = [];

        public override void PostSetupContent()
        {
            Colors.Clear();

            void AddHex(string hex, string name, string description)
            {
                // e.g. [c/32FF82:Your text]
                string tagPrefix = $"[c/{hex.TrimStart('#')}:";
                Colors.Add(new ColorItem(tagPrefix, hex, name, description));
            }

            // Status message colors
            AddHex("#FFFFFF", "Player chat", "Player chat messages.");
            AddHex("#32FF82", "Event begin", "When most events begin.");
            AddHex("#AF4BFF", "Invasion/Boss", "When an invasion begins, or when a boss is defeated or summoned.");
            AddHex("#E11919", "Player death", "When a player dies.");
            AddHex("#FF1919", "NPC/Pet death", "When a town NPC or pet dies.");
            AddHex("#327DFF", "NPC/Pet arrived", "When a town NPC or pet arrives.");
            AddHex("#FFF014", "Status message", "General status messages.");
            AddHex("#FF00A0", "Party", "When an NPC throws a Party.");
        }
    }
}
