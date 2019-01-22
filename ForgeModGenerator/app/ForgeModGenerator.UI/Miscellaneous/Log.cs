using NLog;
using System;
using System.Windows;

namespace ForgeModGenerator.Miscellaneous
{
    public static class Log
    {
        public static readonly ILogger ErrorLogger = LogManager.GetLogger("ErrorLog");
        public static readonly ILogger InfoLogger = LogManager.GetLogger("InfoLog");

        public static void Error(Exception ex, string message, bool messageClient = false)
        {
            MessageClientIfNeeded(message, messageClient);
            ErrorLogger.Error(ex, message);
        }

        public static void Info(string message, bool messageClient = false)
        {
            MessageClientIfNeeded(message, messageClient);
            InfoLogger.Info(message);
        }

        public static void Warning(string message, bool messageClient = false)
        {
            MessageClientIfNeeded(message, messageClient);
            InfoLogger.Warn(message);
        }

        public static void Warning(Exception ex, string message, bool messageClient = false)
        {
            MessageClientIfNeeded(message, messageClient);
            InfoLogger.Warn(ex, message);
        }

        private static void MessageClientIfNeeded(string message, bool show)
        {
            if (show)
            {
                MessageBox.Show(message);
            }
        }
    }
}
