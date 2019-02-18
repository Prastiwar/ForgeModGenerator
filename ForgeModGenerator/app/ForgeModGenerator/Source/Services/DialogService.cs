using GalaSoft.MvvmLight.Views;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ForgeModGenerator.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowError(string message, string title, string buttonText = null, Action afterHideCallback = null) => Task.Run(() => {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            afterHideCallback?.Invoke();
        });

        public Task ShowError(Exception error, string title, string buttonText = null, Action afterHideCallback = null) => Task.Run(() => {
            MessageBox.Show(error.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            afterHideCallback?.Invoke();
        });

        public Task ShowMessage(string message, string title) => Task.Run(() => {
            MessageBox.Show(message, title);
        });

        public Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback = null) => Task.Run(() => {
            MessageBox.Show(message, title, MessageBoxButton.OK);
            afterHideCallback?.Invoke();
        });

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback = null) => Task.Run(() => {
            MessageBoxButton buttons = MessageBoxButton.OKCancel;
            if (!string.IsNullOrEmpty(buttonConfirmText) && buttonConfirmText.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                buttons = MessageBoxButton.YesNo;
            }
            else if (!string.IsNullOrEmpty(buttonCancelText) && buttonCancelText.Equals("no", StringComparison.OrdinalIgnoreCase))
            {
                buttons = MessageBoxButton.YesNo;
            }
            MessageBoxResult result = MessageBox.Show(message, title, buttons);
            bool boolResult = result == MessageBoxResult.OK || result == MessageBoxResult.Yes;
            afterHideCallback?.Invoke(boolResult);
            return boolResult;
        });

        public Task ShowMessageBox(string message, string title) => Task.Run(() => {
            MessageBox.Show(message, title);
        });
    }
}
