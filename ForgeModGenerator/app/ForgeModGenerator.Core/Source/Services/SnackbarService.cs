using System;

namespace ForgeModGenerator.Services
{
    public interface ISnackbarService
    {
        void Notify(string text);
        void Notify(string text, string buttonText, Action onButtonClick);
        void Notify<TArgument>(string text, object buttonText, Action<TArgument> onButtonClick, TArgument actionArgument);
        void Notify(string text, bool neverConsiderToBeDuplicate);
        void Notify(string text, object buttonText, Action actionHandler, bool pushToFront);
        void Notify<TArgument>(string text, object buttonText, Action<TArgument> onButtonClick, TArgument actionArgument, bool pushToFront);
        void Notify<TArgument>(string text, object buttonText, Action<TArgument> onButtonClick, TArgument actionArgument, bool pushToFront, bool neverConsiderToBeDuplicate);
        void Notify(string text, object buttonText, Action<object> onButtonClick, object actionArgument, bool pushToFront, bool neverConsiderToBeDuplicate);
    }
}
