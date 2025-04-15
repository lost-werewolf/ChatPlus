using log4net;
using Terraria.ModLoader;

namespace LinksInChat.Utilities
{
    public static class Log
    {
        public static ILog Logger => ModContent.GetInstance<LinksInChat>().Logger;
        public static void Info(string message) => Logger.Info(message);
        public static void Warn(string message) => Logger.Warn(message);
        public static void Error(string message) => Logger.Error(message);
        public static void Fatal(string message) => Logger.Fatal(message);
        public static void Debug(string message) => Logger.Debug(message);
    }
}

