using System;
using System.Diagnostics;

namespace LinksInChat.Utilities
{
    public static class URL
    {
        public static bool IsLink(string text)
        {
            return text.StartsWith("http://") || text.StartsWith("https://") || text.StartsWith("www.") || text.EndsWith(".com");
        }

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
                Log.Warn($"Failed to open URL: {url}");
            }
        }

        public static string ExtractURLFromMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return null;

            // If we have a .com link but no www, we add www. to the start of it.
            if (message.Contains(".com") && !message.Contains("www."))
            {
                message = message.Replace(".com", "www." + ".com");
                return message;
            }

            // Find any instances of https:// or http://, followed by any characters until a space or end of string, its guaranteed to be a URL, return it. 
            // Dont use regex, just string methods.
            if (message.Contains("https://") || message.Contains("http://"))
            {
                int startIndex = message.IndexOf("https://", StringComparison.OrdinalIgnoreCase);
                if (startIndex == -1)
                    startIndex = message.IndexOf("http://", StringComparison.OrdinalIgnoreCase);

                int endIndex = message.IndexOf(' ', startIndex);
                if (endIndex == -1)
                    endIndex = message.Length;

                string result = message.Substring(startIndex, endIndex - startIndex);
                return result;
            }
            return null;
        }
    }
}