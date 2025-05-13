using System;
using System.IO;
using System.Runtime.CompilerServices;
using AdvancedChatFeatures.Common.Configs;
using log4net;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Helpers
{
    public static class Log
    {
        private static ILog LoggerInstance => ModContent.GetInstance<AdvancedChatFeatures>().Logger;

        private static DateTime lastLogTime;

        public static void Error(string message)
        {
            if (Conf.C != null && !Conf.C.ShowDebugMessages)
            {
                return;
            }

            LoggerInstance.Info(message);
        }

        public static void SlowInfo(string message, int seconds = 1, [CallerFilePath] string callerFilePath = "")
        {
            if (Conf.C != null && !Conf.C.ShowDebugMessages)
                return; // Skip logging if the config is set to false

            // Extract the class name from the caller's file path.
            string className = Path.GetFileNameWithoutExtension(callerFilePath);
            var instance = ModContent.GetInstance<AdvancedChatFeatures>();
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            // Use TimeSpanFactory to create a 3-second interval.
            TimeSpan interval = TimeSpan.FromSeconds(seconds);
            if (DateTime.UtcNow - lastLogTime >= interval)
            {
                // Prepend the class name to the log message.
                instance.Logger.Info($"[{className}] {message}");
                lastLogTime = DateTime.UtcNow;
            }
        }

        public static void Info(string message, [CallerFilePath] string callerFilePath = "")
        {
            if (Conf.C != null && !Conf.C.ShowDebugMessages)
                return; // Skip logging if the config is set to false

            // Extract the class name from the caller's file path.
            string className = Path.GetFileNameWithoutExtension(callerFilePath);
            var instance = ModContent.GetInstance<AdvancedChatFeatures>();
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            // Prepend the class name to the log message.
            instance.Logger.Info($"[{className}] {message}");
        }
    }
}

