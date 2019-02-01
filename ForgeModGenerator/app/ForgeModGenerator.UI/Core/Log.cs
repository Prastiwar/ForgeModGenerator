using NLog;
using System;
using System.Windows;

namespace ForgeModGenerator
{
    public static class Log
    {
        public static readonly ILogger ErrorLogger = LogManager.GetLogger("ErrorLog");
        public static readonly ILogger InfoLogger = LogManager.GetLogger("InfoLog");

        public static void Error(Exception ex, string message = "", bool messageClient = false)
        {
            if (messageClient)
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ErrorLogger.Error(ex, message);
        }

        public static void Info(string message, bool messageClient = false)
        {
            if (messageClient)
            {
                MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            InfoLogger.Info(message);
        }

        public static void Warning(string message, bool messageClient = false)
        {
            if (messageClient)
            {
                MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            InfoLogger.Warn(message);
        }

        public static void Warning(Exception ex, string message = "", bool messageClient = false)
        {
            if (messageClient)
            {
                MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            ErrorLogger.Warn(ex, message);
        }
    }
}
