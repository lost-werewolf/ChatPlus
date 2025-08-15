using System;
using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.UI.Commands
{
    internal class CommandInitializerSystem : ModSystem
    {
        public static List<Command> Commands { get; private set; } = [];

        public override void PostSetupContent()
        {
            Commands.Clear();

            // Load commands from ModCommand
            foreach (var cmdList in CommandLoader.Commands)
            {
                foreach (ModCommand cmd in cmdList.Value)
                {
                    // Skip "help" from tmodloader and add it in vanilla instead
                    if (cmd.Command == "help") continue;

                    string name = "/" + cmd.Command;
                    string usage = cmd.Description;
                    Mod mod = cmd.Mod;

                    Commands.Add(new Command(name, usage, mod));
                    Log.Info("(1) Command loaded: " + name + ", usage: " + cmd.Usage + ", from mod: " + cmd.Mod.Name);
                }
            }

            // Load vanilla commands from ChatManager
            foreach (var cmdId in ChatManager.Commands._localizedCommands)
            {
                string name = cmdId.Key.Value; // the command, e.g "/p"
                string localizationKey = cmdId.Value._name; // the key, e.g "Party"
                string usage = Language.GetTextValue("ChatCommandDescription." + localizationKey); // description, e.g "Send a message to your party members"
                Texture2D terrariaIcon = Ass.TerrariaIcon.Value;

                Commands.Add(new Command(name, usage));
                Log.Info("(2) Command loaded: " + name + ", usage: " + usage + ", from ChatManager");
            }

            // Sort first by mod name, then command name
            Commands.Sort((a, b) =>
            {
                string am = a.Mod != null
                    ? (a.Mod.DisplayNameClean ?? a.Mod.Name)
                    : "Terraria";
                string bm = b.Mod != null
                    ? (b.Mod.DisplayNameClean ?? b.Mod.Name)
                    : "Terraria";

                int modCmp = string.Compare(am, bm, StringComparison.OrdinalIgnoreCase);
                if (modCmp != 0) return modCmp;

                // Command name key
                string an = a.Name.StartsWith("/") ? a.Name.Substring(1) : a.Name;
                string bn = b.Name.StartsWith("/") ? b.Name.Substring(1) : b.Name;

                return string.Compare(an, bn, StringComparison.OrdinalIgnoreCase);
            });

            Log.Info("Total commands loaded: " + Commands.Count);
        }
    }
}
