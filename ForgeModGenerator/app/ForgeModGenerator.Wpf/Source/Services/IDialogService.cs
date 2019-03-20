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
    }
}
