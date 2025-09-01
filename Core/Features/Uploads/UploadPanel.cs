using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ChatPlus.Core.Helpers;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Utilities.FileBrowser;
using static nativefiledialog;

namespace ChatPlus.Core.Features.Uploads
{
    public class UploadPanel : BasePanel<Upload>
    {
        protected override BaseElement<Upload> BuildElement(Upload data) => new UploadElement(data);
        protected override IEnumerable<Upload> GetSource() => UploadManager.Uploads;
        protected override string GetDescription(Upload data) => data.FileName;
        protected override string GetTag(Upload data) => data.Tag;

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }

        public void UploadImage()
        {
            string fullFilePath = OpenFileDialog();
            if (string.IsNullOrEmpty(fullFilePath))
                return;

            try
            {
                string fileName = Path.GetFileName(fullFilePath);
                string key = Path.GetFileNameWithoutExtension(fileName);

                // 1) Persist a copy into the mod's uploads folder FIRST
                string folder = Path.Combine(Main.SavePath, "ChatPlus", "Uploads");
                Directory.CreateDirectory(folder);
                string dest = Path.Combine(folder, fileName);

                // If user picked a file inside the same folder with same name, this is harmless
                File.Copy(fullFilePath, dest, true);

                // 2) Load texture from the DESTINATION (ensures file exists before we read & register)
                Texture2D texture;
                using (var fs = File.OpenRead(dest))
                    texture = Texture2D.FromStream(Main.instance.GraphicsDevice, fs);

                if (texture == null)
                {
                    Main.NewText("Failed to load image.", Color.Red);
                    return;
                }

                // 3) Register tag & add/replace in memory
                UploadTagHandler.Register(key, texture);
                string tag = UploadTagHandler.GenerateTag(key);

                UploadManager.AddNewUpload(
                    new Upload(
                        Tag: tag,
                        FileName: fileName,
                        FullFilePath: dest,
                        Texture: texture
                    )
                );

                // 4) Refresh UI
                if (ConnectedPanel is UploadPanel up)
                    up.PopulatePanel();

                Log.Info($"Added {fileName} as {tag}");
                //Main.NewText($"Added {fileName} as {tag}", Color.LightGreen);
            }
            catch (Exception ex)
            {
                Main.NewText("Upload failed: " + ex.Message, Color.Red);
                Log.Error("Upload failed: " + ex);
            }
        }

        private static string OpenFileDialog()
        {
            var extensions = new ExtensionFilter
            {
                Name = "Images",
                Extensions = ["png", "jpg", "jpeg"]
            };

            // Concatenate extensions for NFD: "png,jpg,jpeg"
            string extensionStr = string.Join(',', extensions.Extensions);

            // Initial directory – use the user's Pictures folder or leave null for default
            //string startDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            nfdresult_t result = NFD_OpenDialog(
                extensionStr,              // filter list
                null,                  // initial directory
                out string outPath);

            if (result == nfdresult_t.NFD_CANCEL)
            {
                // User cancelled the dialog
                Log.Info("File dialog was cancelled by the user.");
                return null;
            }
            if (string.IsNullOrEmpty(outPath))
            {
                // No file was selected
                Log.Error("No file selected.");
                return null;
            }
            if (!File.Exists(outPath))
            {
                // The selected file does not exist
                Log.Error($"File does not exist: {outPath}");
                return null;
            }
            if (result != nfdresult_t.NFD_OKAY)
            {
                // An error occurred
                Log.Error($"Failed to open file dialog: {NFD_GetError()}");
                return null;
            }
            // Log.Info($"Selected file: {outPath}");
            return outPath;
        }

        public void OpenUploadsFolder()
        {
            try
            {
                string folder = Path.Combine(Main.SavePath, "ChatPlus", "Uploads");
                Process.Start(new ProcessStartInfo($@"{folder}")
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Main.NewText("Error opening folder: " + ex.Message, Color.Red);
                Log.Error("Error opening client log: " + ex.Message);
            }
        }
    }
}
