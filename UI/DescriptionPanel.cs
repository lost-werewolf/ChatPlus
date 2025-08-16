using System;
using System.IO;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Emojis;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UploadWindow;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AdvancedChatFeatures.UI
{
    public class DescriptionPanel<TData> : DraggablePanel
    {
        private readonly UIText text;
        public UIText GetText() => text;

        public DescriptionPanel(string initialText)
        {
            // Size
            Width.Set(320, 0);
            Height.Set(60, 0);

            // Style
            BackgroundColor = ColorHelper.DarkBlue * 0.8f;

            // Position
            VAlign = 1f;
            Left.Set(80, 0);

            text = new(initialText, 0.9f, false);
            Append(text);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (ConnectedPanel.GetType() == typeof(UploadPanel))
            {
                string fullFilePath = FileUploadHelper.OpenFileDialog();
                string fileName = Path.GetFileName(fullFilePath);
                string tag = UploadTagHandler.Tag(fileName);
                Texture2D tex = FileUploadHelper.ReadAndCreateTextureFromPath(fullFilePath);
                UploadInitializer.Uploads.Add(new Upload(
                    Tag: tag,
                    FileName: fileName,
                    FullFilePath: fullFilePath,
                    Image: tex
                ));

                // Copy image to our folder
                string folder = Path.Combine(Main.SavePath, "AdvancedChatFeatures", "Uploads");
                Directory.CreateDirectory(folder);
                string dest = Path.Combine(folder, fileName);
                File.Copy(fullFilePath, dest, overwrite: true);
            }

            base.LeftClick(evt);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            Conf.C.Open();

            // Expand autocomplete section in config after UI is built
            Main.QueueMainThreadAction(() =>
            {
                var state = Main.InGameUI.CurrentState;
                if (state is not UIElement root) return;

                Conf.C.ExpandSection(root, "Style");
            });
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

            Top.Set(-Conf.C.featureStyleConfig.ItemsPerWindow * 30 - 38 - pad, 0);

            base.Draw(spriteBatch);
        }
    }
}
