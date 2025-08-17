using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Utilities.FileBrowser;
using static nativefiledialog;

namespace AdvancedChatFeatures.Helpers;

/// <summary>
/// Lets the user choose an image file and hands the uploadedTexture off to 
/// <see cref="BackgroundHook"/>. or <see cref="LogoHook"/>.
/// </summary>
public static class FileUploadHelper
{
    /// <summary>Run when the “Choose File” button is clicked.</summary>
    public static Texture2D CreateTextureFromPath(string path)
    {
        try
        {
            using var fs = File.OpenRead(path);
            Texture2D texture = Texture2D.FromStream(Main.instance.GraphicsDevice, fs);
            return texture;
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to read and create asset from path {path} – {ex.Message}");
            return null;
        }
    }

    /// <returns>Full path of the chosen file, or <c>null</c> if cancelled / unsupported.</returns>
    public static string OpenFileDialog()
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
        Log.Info($"Selected file: {outPath}");
        return outPath;
    }
}