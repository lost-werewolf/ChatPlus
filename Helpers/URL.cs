using System;
using System.Diagnostics;
using Terraria;

namespace AdvancedChatFeatures.Helpers
{
    public static class URL
    {
        public static void OpenURL(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            try
            {
                // Use the default browser to open the URL by assigning url to FileName.
                Process.Start(new ProcessStartInfo
                {
                    FileName = url, // Specify the URL to open
                    UseShellExecute = true
                });
            }
            catch (Exception)
            {
                Main.NewText($"Failed to open URL: {url}");
                Log.Error($"Failed to open URL: {url}");
            }
        }
    }
}
