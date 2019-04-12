using System;
using System.Threading.Tasks;

namespace ForgeModGenerator.Services
{
    public interface IDialogService
    {
        Task ShowError(string message, string title, string buttonText, Action afterHideCallback);
        Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback);

        Task ShowMessage(string message, string title);
        Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback);
        Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback);

        Task<object> Show(object content);
        Task<object> Show(object content, EventHandler<DialogOpenedArgs> openedArgs, EventHandler<DialogClosingArgs> closingArgs);
    }

    public class DialogOpenedArgs : EventArgs
    {
        public bool ShouldClose { get; private set; }

        public void Close() => ShouldClose = true;
    }

    public class DialogClosingArgs : EventArgs
    {
        public DialogClosingArgs(object result) => Result = result;

        public object Result { get; }

        public bool IsCancelled { get; private set; }

        public void Cancel() => IsCancelled = true;
    }
}
