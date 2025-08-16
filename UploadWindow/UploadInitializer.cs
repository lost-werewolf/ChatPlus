using System.Collections.Generic;
using System.IO;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.UploadWindow
{
    [Autoload(Side = ModSide.Client)]
    internal class UploadInitializer : ModSystem
    {
        public static List<Upload> Uploads { get; private set; } = new();
        private static bool _handlerRegistered;

        public override void Load()
        {
            if (!_handlerRegistered)
            {
                ChatManager.Register<UploadTagHandler>("u");
                _handlerRegistered = true;
            }
        }

        public override void Unload()
        {
            Uploads = null;
            UploadTagHandler.Clear();
        }

        public override void PostSetupContent()
        {
            Uploads = new List<Upload>();

            // Scan a user folder for PNGs (customize as needed)
            string folder = Path.Combine(Main.SavePath, "AdvancedChatFeatures", "Uploads");
            Directory.CreateDirectory(folder);

            foreach (var file in Directory.EnumerateFiles(folder, "*.png", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    string key = Path.GetFileNameWithoutExtension(file);
                    using var fs = File.OpenRead(file);
                    Texture2D tex = Texture2D.FromStream(Main.instance.GraphicsDevice, fs);
                    if (UploadTagHandler.Register(key, tex))
                    {
                        Uploads.Add(new Upload(UploadTagHandler.Tag(key), key, file, $"{key}.png"));
                        Log.Info($"Found: {key}");
                    }
                }
                catch { /* ignore bad files */ }
            }
            Log.Info($"[end] Found {Uploads.Count} uploads");
        }
    }
}
