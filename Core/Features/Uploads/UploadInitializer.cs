using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChatPlus.Helpers;
using ChatPlus.UploadHandler;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Uploads
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
            // Dispose all textures on unload
            if (Uploads != null)
            {
                foreach (var u in Uploads)
                {
                    try { u.Texture?.Dispose(); } catch { }
                }
            }

            Uploads = null;
            UploadTagHandler.Clear();
        }

        public static void AddNewUpload(Upload upload)
        {
            int idx = Uploads.FindIndex(u =>
                !string.IsNullOrEmpty(u.Tag) &&
                string.Equals(u.Tag, upload.Tag, StringComparison.OrdinalIgnoreCase));

            if (idx >= 0)
                Uploads[idx] = upload;
            else
                Uploads.Add(upload);
        }

        public static void InitializeUploadedTextures()
        {
            string folder = Path.Combine(Main.SavePath, "ChatPlus", "Uploads");
            Directory.CreateDirectory(folder);

            var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                                 .Where(f =>
                                 {
                                     string e = Path.GetExtension(f);
                                     return e.Equals(".png", StringComparison.OrdinalIgnoreCase)
                                         || e.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
                                         || e.Equals(".jpeg", StringComparison.OrdinalIgnoreCase);
                                 })
                                 .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                                 .ToList();

            Main.QueueMainThreadAction(() =>
            {
                try
                {
                    var fresh = new List<Upload>(files.Count);
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
                            // ignore bad file
                        }
                    }

                    // swap atomically
                    foreach (var old in Uploads)
                    {
                        try { old.Texture?.Dispose(); } catch { }
                    }
                    Uploads.Clear();
                    Uploads.AddRange(fresh);

                    Log.Info($"Uploads refreshed: {Uploads.Count} files");
                }
                catch (Exception ex)
                {
                    Log.Error("InitializeUploadedTextures failed: " + ex);
                }
            });
        }
    }
}
