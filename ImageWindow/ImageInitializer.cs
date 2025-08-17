using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.ImageWindow;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.ImageWindow
{
    [Autoload(Side = ModSide.Client)]
    internal class ImageInitializer : ModSystem
    {
        public static List<Image> Uploads { get; private set; } = [];
        private static bool _handlerRegistered;

        public override void Load()
        {
            if (!_handlerRegistered)
            {
                ChatManager.Register<ImageTagHandler>("u");
                _handlerRegistered = true;
            }
        }

        public override void Unload()
        {
            Uploads = null;
            ImageTagHandler.Clear();
        }

        public override void PostSetupContent()
        {
            Uploads = new List<Image>();

            string folder = Path.Combine(Main.SavePath, "AdvancedChatFeatures", "Uploads");
            Directory.CreateDirectory(folder);

            // Collect all supported image files
            var exts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg" };
            var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                                 .Where(f => exts.Contains(Path.GetExtension(f)));

            Main.QueueMainThreadAction(() =>
            {
                foreach (var file in files)
                {
                    try
                    {
                        string key = Path.GetFileNameWithoutExtension(file);
                        using var fs = File.OpenRead(file);
                        Texture2D tex = Texture2D.FromStream(Main.instance.GraphicsDevice, fs);

                        if (ImageTagHandler.Register(key, tex))
                        {
                            Uploads.Add(new Image(
                                Tag: ImageTagHandler.Tag(key),
                                FileName: Path.GetFileName(file),
                                FullFilePath: file,
                                Texture: tex
                            ));
                            Log.Info($"Found: {file}");
                        }
                    }
                    catch
                    {
                        // ignore broken files
                    }
                }

                Log.Info($"[end] Found {Uploads.Count} uploads");
            });

            int fileCountInFolder = Directory.GetFiles(folder).Length;

            Log.Info($"[end] Found ({Uploads.Count}/{fileCountInFolder}) uploads");
        }
    }
}
