using System;
using System.IO;
using System.Runtime.CompilerServices;
using log4net;
using Terraria.ModLoader;

namespace ChatPlus.Helpers
{
    public static class Log
    {
        private static ILog LoggerInstance => ModContent.GetInstance<ChatPlus>().Logger;

        private static DateTime lastLogTime;

        public static void Error(string message)
        {
            //if (Conf.C != null && !Conf.C.ShowDebugMessages) return;

            LoggerInstance.Error(message);
        }

        public static void SlowInfo(string message, int seconds = 1, [CallerFilePath] string callerFilePath = "")
        {
            //if (Conf.C != null && !Conf.C.ShowDebugMessages) return;

            // Extract the class name from the caller's file path.
            string className = Path.GetFileNameWithoutExtension(callerFilePath);
            var instance = ModContent.GetInstance<ChatPlus>();
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

        public static void Info(string message, bool printCallerInMessage = true, [CallerFilePath] string callerFilePath = "")
        {
            //if (Conf.C != null && !Conf.C.ShowDebugMessages) return; 

            // Extract the class name from the caller's file path.
            string className = Path.GetFileNameWithoutExtension(callerFilePath);
            var instance = ModContent.GetInstance<ChatPlus>();
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            // Prepend the class name to the log message.
            if (printCallerInMessage)
                instance.Logger.Info($"[{className}] {message}");
            else
                instance.Logger.Info(message);
        }

        public static void Warn(string message, [CallerFilePath] string callerFilePath = "")
        {
            //if (Conf.C != null && !Conf.C.ShowDebugMessages) return; 

            // Extract the class name from the caller's file path.
            string className = Path.GetFileNameWithoutExtension(callerFilePath);
            var instance = ModContent.GetInstance<ChatPlus>();
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            // Prepend the class name to the log message.
            instance.Logger.Warn($"[{className}] {message}");
        }
    }
}

