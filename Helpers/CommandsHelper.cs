using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Chat;
using Terraria.Chat.Commands;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Helpers
{
    public static class CommandsHelper
    {
        public static List<ModCommand> GetAllCommands()
        {
            List<ModCommand> res = [];

            foreach (var cmdList in CommandLoader.Commands.Values)
            {
                foreach (ModCommand cmd in cmdList)
                {
                    res.Add(cmd);
                }
            }

            return res;
        }

        public static List<Mod> GetModsWithCommands()
        {
            List<Mod> res = [];

            // count commands per mod
            var countByMod = new Dictionary<Mod, int>();
            foreach (var cmd in CommandsHelper.GetAllCommands())
            {
                if (cmd?.Mod == null) continue;
                countByMod.TryGetValue(cmd.Mod, out int c);
                countByMod[cmd.Mod] = c + 1;
            }

            // keep only mods that have commands
            foreach (var kv in countByMod)
                if (kv.Value > 0)
                    res.Add(kv.Key);

            // sort alphabetically by display name
            res.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, System.StringComparison.OrdinalIgnoreCase));

            return res;
        }

        public static Asset<Texture2D> GetModIcon(TmodFile tmodFile)
        {
            if (tmodFile == null)
            {
                Log.Info("null file, returning null.");
                return null;
            }

            try
            {
                // Check if the file contains "icon.png"
                if (!tmodFile.HasFile("icon.png"))
                {
                    Log.Error("The TmodFile does not have an icon.");
                    return null;
                }

                // Open the file and retrieve the stream for "icon.png"
                using (tmodFile.Open())
                using (Stream stream = tmodFile.GetStream("icon.png", true))
                {
                    Asset<Texture2D> iconTexture = Main.Assets.CreateUntracked<Texture2D>(stream, ".png", AssetRequestMode.ImmediateLoad);

                    // Log.Info("Successfully loaded icon from TmodFile.");
                    return iconTexture;
                }
            }
            catch (Exception ex)
            {
                Log.Info("Error while retrieving icon from TmodFile: " + ex);
            }
            return null;
        }

        public static void ExecuteCommand(string commandText)
        {
            // Run through command processor
            var message = new ChatMessage(commandText);
            var caller = new ChatCommandCaller();

            if (CommandLoader.HandleCommand("/" + message.Text, caller))
            {
                Main.NewText("executed " + commandText);
                // Command executed.
            }
            else
            {
                Main.NewText("failed " + commandText);
            }
        }
    }
}
