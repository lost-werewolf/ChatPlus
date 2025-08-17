using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Uploads
{
    [Autoload(Side = ModSide.Client)]
    internal class UploadInitializer : ModSystem
    {
        public static List<Upload> Uploads { get; private set; } = [];

        public override void PostSetupContent()
        {
            ChatManager.Register<UploadTagHandler>("u");
            InitializeUploadedTextures();
        }

        public override void Unload()
        {
            Uploads = null;
            UploadTagHandler.Clear();
        }

        public void AddNewUpload(Upload upload)
        {
            if (!Uploads.Contains(upload))
            {
                Uploads.Add(upload);
            }
            else
            {
                Log.Info("Duplicate detected; file not added");
            }
        }

        private void InitializeUploadedTextures()
        {
            Uploads.Clear();

            string folder = Path.Combine(Main.SavePath, "AdvancedChatFeatures", "Uploads");
            Directory.CreateDirectory(folder);

            var exts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg" };
            var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                                 .Where(f => exts.Contains(Path.GetExtension(f)))
                                 .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                                 .ToList();

            Main.QueueMainThreadAction(() =>
            {
                foreach (var file in files)
                {
                    try
                    {
                        string key = Path.GetFileNameWithoutExtension(file);
                        using var fs = File.OpenRead(file);
                        Texture2D texture = Texture2D.FromStream(Main.instance.GraphicsDevice, fs);

                        if (UploadTagHandler.Register(key, texture))
                        {
                            AddNewUpload(new Upload(
                                Tag: UploadTagHandler.GenerateTag(key),
                                FileName: Path.GetFileName(file),
                                FullFilePath: file,
                                Texture: texture
                            ));
                            Log.Info($"({Uploads.Count}) Initialized file {Path.GetFileName(file)}");
                        }
                    }
                    catch
                    {
                        Log.Info($"({Uploads.Count}) Failed to read file {Path.GetFileName(file)}");
                    }
                }

                int fileCountInFolder = Directory.GetFiles(folder).Length;
                Log.Info($"[end] Found ({Uploads.Count}/{fileCountInFolder}) uploads");
            });
        }

    }
}
