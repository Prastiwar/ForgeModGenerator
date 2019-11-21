using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace ForgeModGenerator.Services
{
    public class DialogService : IDialogService
    {
        private readonly FieldInfo dialogInstancesField = typeof(DialogHost).GetField("LoadedInstances", BindingFlags.NonPublic | BindingFlags.Static);

        public Task ShowError(string message, string title, string buttonText = null, Action afterHideCallback = null)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            afterHideCallback?.Invoke();
            return Task.Delay(0);
        }

        public Task ShowError(Exception error, string title, string buttonText = null, Action afterHideCallback = null)
        {
            MessageBox.Show(error.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            afterHideCallback?.Invoke();
            return Task.Delay(0);
        }

        public Task ShowMessage(string message, string title)
        {
            MessageBox.Show(message, title);
            return Task.Delay(0);
        }

        public Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback = null)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK);
            afterHideCallback?.Invoke();
            return Task.Delay(0);
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback = null)
        {
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
            return Task.FromResult(boolResult);
        }

        public Task<object> Show(object content) => ShowDialog(content, null, null);

        public Task<object> Show(object content, EventHandler<DialogOpenedEventArgs> openedArgs, EventHandler<DialogClosingEventArgs> closingArgs)
        {
            DialogOpenedEventHandler openedEventHandler = new DialogOpenedEventHandler((s, args) => {
                DialogOpenedEventArgs extArgs = new DialogOpenedEventArgs();
                openedArgs(s, extArgs);
                if (extArgs.ShouldClose)
                {
                    args.Session.Close();
                }
            });

            DialogClosingEventHandler dialogClosingEventHandler = new DialogClosingEventHandler((s, args) => {
                DialogClosingEventArgs extArgs = new DialogClosingEventArgs(args.Parameter);
                closingArgs(s, extArgs);
                if (extArgs.IsCancelled)
                {
                    args.Cancel();
                }
            });
            return ShowDialog(content, openedEventHandler, dialogClosingEventHandler);
        }

        private Task<object> ShowDialog(object content, DialogOpenedEventHandler openedEventHandler, DialogClosingEventHandler dialogClosingEventHandler)
        {
            HashSet<DialogHost> instances = (HashSet<DialogHost>)dialogInstancesField.GetValue(null);
            if (instances != null)
            {
                if (instances.Count == 0)
                {
                    throw new InvalidOperationException("No loaded DialogHost instances.");
                }
                DialogHost host = instances.First();
                if (host.IsOpen)
                {
                    host.IsOpen = false;
                }
                return host.ShowDialog(content, openedEventHandler, dialogClosingEventHandler);
            }
            return DialogHost.Show(content, openedEventHandler, dialogClosingEventHandler);
        }
    }
}
