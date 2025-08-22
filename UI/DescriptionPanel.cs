using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ChatPlus.Common.Configs;
using ChatPlus.Helpers;
using ChatPlus.UploadHandler;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Utilities.FileBrowser;
using static nativefiledialog;

namespace ChatPlus.UI
{
    public class DescriptionPanel<TData> : DraggablePanel
    {
        private readonly UIText text;
        public UIText GetText() => text;

        public DescriptionPanel(string initialText = null, bool centerText = false)
        {
            // Size
            Width.Set(320, 0);
            Height.Set(60, 0);

            // Style
            BackgroundColor = ColorHelper.DarkBlue * 1.0f;

            // Position
            VAlign = 1f;
            Left.Set(80, 0);

            // Text
            text = new(initialText ?? string.Empty, 0.9f, false)
            {
                HAlign = 0.5f
            };

            if (centerText)
            {
                text.HAlign = 0.5f;
                text.VAlign = 0.5f;
            }

            Append(text);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            UploadImage();
        }
        private void UploadImage()
        {
            if (typeof(TData) != typeof(Upload))
                return;

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

                UploadInitializer.AddNewUpload(
                    new Upload(
                        Tag: tag,
                        FileName: fileName,
                        FullFilePath: dest,
                        Texture: texture
                    )
                );

                // 4) Refresh UI now (no re-init; no async)
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

        public override void RightClick(UIMouseEvent evt)
        {
            OpenUploadsFolder();
        }

        private void OpenUploadsFolder()
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

        public void SetTextWithLinebreak(string rawText)
        {
            float scale = 0.9f;
            string tooltip = rawText;

            // Measure the text size
            if (FontAssets.MouseText == null) return;
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(tooltip);
            float scaledWidth = textSize.X * scale / Main.UIScale;

            // If the text is too wide, insert a line break at the last space before it overflows
            if (scaledWidth > Width.Pixels)
            {
                // Estimate the max number of characters that fit
                int maxChars = (int)(Width.Pixels / scale / textSize.X * tooltip.Length) - 2;
                int breakIndex = tooltip.LastIndexOf(' ', Math.Min(tooltip.Length - 1, maxChars));
                if (breakIndex > 0)
                {
                    tooltip = tooltip[..breakIndex] + "\n" + tooltip[(breakIndex + 1)..];
                }
            }

            text.SetText(tooltip);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int pad = 2;

            Top.Set(-Conf.C.autocompleteWindowConfig.ItemsPerWindow * 30 - 38 - pad, 0);

            if (ConnectedPanel.GetType() == typeof(UploadPanel))
            {
                Height.Set(90, 0);
                text.SetText("Left click to upload an image\nRight click to open image folder\nModify image size with (u:img|size=100)");
            }

            base.Draw(spriteBatch);
        }
    }
}
