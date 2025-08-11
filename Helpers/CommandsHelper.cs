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

        public static Texture2D GetModIcon(TmodFile tmodFile)
        {
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
                    return iconTexture.Value;
                }
            }
            catch (Exception ex)
            {
                Log.Info("Error while retrieving icon from TmodFile: " + ex);
            }
            return null;
        }
    }
}
