using ForgeModGenerator.Services;
using Microsoft.Extensions.Logging;
using System;

namespace ForgeModGenerator
{
    public static class Log
    {
        public static string UnexpectedErrorMessage = "Something went wrong, this should never happened. Please, report a bug at https://github.com/Prastiwar/ForgeModGenerator/issues/new?template=bug_report.md";
        
        private static IDialogService dialogService;
        private static ILogger errorLogger;
        private static ILogger infoLogger;
        private static bool isInitialized;

        private static string FormatMoreInformation(string message, string moreInformation) => $"{message}{Environment.NewLine}More information: {moreInformation}";

        public static void Initialize(IDialogService dialogService, ILogger errorLogger, ILogger infoLogger)
        {
            Log.dialogService = dialogService;
            Log.errorLogger = errorLogger;
            Log.infoLogger = infoLogger;
            isInitialized = true;
        }

        private static void InitCheck()
        {
            if (!isInitialized)
            {
                throw new ClassNotInitializedException(typeof(Log));
            }
        }

        public static void Error(Exception ex, string message = "", bool messageClient = false, string moreInformation = null)
        {
            InitCheck();
            if (messageClient)
            {
                dialogService.ShowError(message, "Error", "OK", null);
            }
            string msg = string.IsNullOrEmpty(moreInformation) ? message : FormatMoreInformation(message, moreInformation);
            errorLogger.LogError(ex, msg);
        }

        public static void Info(string message, bool messageClient = false, string moreInformation = null)
        {
            InitCheck();
            if (messageClient)
            {
                dialogService.ShowMessage(message, "Info", "OK", null);
            }
            string msg = string.IsNullOrEmpty(moreInformation) ? message : FormatMoreInformation(message, moreInformation);
            infoLogger.LogInformation(msg);
        }

        public static void Warning(string message, bool messageClient = false, string moreInformation = null)
        {
            InitCheck();
            if (messageClient)
            {
                dialogService.ShowMessage(message, "Warning", "OK", null);
            }
            string msg = string.IsNullOrEmpty(moreInformation) ? message : FormatMoreInformation(message, moreInformation);
            infoLogger.LogWarning(msg);
        }

        public static void Warning(Exception ex, string message = "", bool messageClient = false, string moreInformation = null)
        {
            InitCheck();
            if (messageClient)
            {
                dialogService.ShowMessage(message, "Warning", "OK", null);
            }
            string msg = string.IsNullOrEmpty(moreInformation) ? message : FormatMoreInformation(message, moreInformation);
            errorLogger.LogWarning(ex, msg);
        }
    }
}
