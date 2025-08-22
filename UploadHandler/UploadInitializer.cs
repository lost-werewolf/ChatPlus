using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChatPlus.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.UploadHandler
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

        public static void AddNewUpload(Upload upload)
        {
            int idx = Uploads.FindIndex(u =>
                !string.IsNullOrEmpty(u.Tag) &&
                string.Equals(u.Tag, upload.Tag, StringComparison.OrdinalIgnoreCase));

            if (idx >= 0)
                Uploads[idx] = upload;  // replace in place (refresh)
            else
                Uploads.Add(upload);
        }


        public static void InitializeUploadedTextures()
        {
            string folder = Path.Combine(Main.SavePath, "ChatPlus", "Uploads");
            Directory.CreateDirectory(folder);

            var exts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg" };
            var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                                 .Where(f => exts.Contains(Path.GetExtension(f)))
                                 .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                                 .ToList();

            // Load on the main thread (for GraphicsDevice)
            Main.QueueMainThreadAction(() =>
            {
                var fresh = new List<Upload>(files.Count);

                // Reset the tag registry so removed files are truly gone
                UploadTagHandler.Clear();

                foreach (var file in files)
                {
                    try
                    {
                        string key = Path.GetFileNameWithoutExtension(file);
                        using var fs = File.OpenRead(file);
                        Texture2D texture = Texture2D.FromStream(Main.instance.GraphicsDevice, fs);

                        if (UploadTagHandler.Register(key, texture))
                        {
                            fresh.Add(new Upload(
                                Tag: UploadTagHandler.GenerateTag(key),
                                FileName: Path.GetFileName(file),
                                FullFilePath: file,
                                Texture: texture
                            ));
                        }
                    }
                    catch
                    {
                        // ignore broken files
                    }
                }

                // 🔹 Swap atomically so UI reflects current disk state (adds + deletions)
                Uploads.Clear();
                Uploads.AddRange(fresh);

                int fileCountInFolder = Directory.GetFiles(folder).Length;
                Log.Info($"[end] Found ({Uploads.Count}/{fileCountInFolder}) uploads");
            });
        }
    }
}
