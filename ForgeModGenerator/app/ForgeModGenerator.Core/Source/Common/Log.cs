using System;
using System.Windows;

namespace ForgeModGenerator
{
    //public static class Log
    //{
    //    public static string UnexpectedErrorMessage = "Something went wrong, this should never happened. Please, report a bug at https://github.com/Prastiwar/ForgeModGenerator/issues/new?template=bug_report.md";

    //    public static readonly ILogger ErrorLogger = LogManager.GetLogger("ErrorLog");
    //    public static readonly ILogger InfoLogger = LogManager.GetLogger("InfoLog");

    //    private static string FormatMoreInformation(string message, string moreInformation) => $"{message}{Environment.NewLine}More information: {moreInformation}";

    //    public static void Error(Exception ex, string message = "", bool messageClient = false, string moreInformation = null)
    //    {
    //        if (messageClient)
    //        {
    //            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    //        }
    //        string msg = string.IsNullOrEmpty(moreInformation) ? message : FormatMoreInformation(message, moreInformation);
    //        ErrorLogger.Error(ex, msg);
    //    }

    //    public static void Info(string message, bool messageClient = false, string moreInformation = null)
    //    {
    //        if (messageClient)
    //        {
    //            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    //        }
    //        string msg = string.IsNullOrEmpty(moreInformation) ? message : FormatMoreInformation(message, moreInformation);
    //        InfoLogger.Info(msg);
    //    }

    //    public static void Warning(string message, bool messageClient = false, string moreInformation = null)
    //    {
    //        if (messageClient)
    //        {
    //            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
    //        }
    //        string msg = string.IsNullOrEmpty(moreInformation) ? message : FormatMoreInformation(message, moreInformation);
    //        InfoLogger.Warn(msg);
    //    }

    //    public static void Warning(Exception ex, string message = "", bool messageClient = false, string moreInformation = null)
    //    {
    //        if (messageClient)
    //        {
    //            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
    //        }
    //        string msg = string.IsNullOrEmpty(moreInformation) ? message : FormatMoreInformation(message, moreInformation);
    //        ErrorLogger.Warn(ex, msg);
    //    }
    //}
}
