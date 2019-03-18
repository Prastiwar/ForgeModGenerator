using MaterialDesignThemes.Wpf;
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

    public class SnackbarService : SnackbarMessageQueue, ISnackbarService
    {
        public void Notify(string text) => Enqueue(text);
        public void Notify(string text, string buttonText, Action onButtonClick) => Enqueue(text, buttonText, onButtonClick);
        public void Notify<TArgument>(string text, object buttonText, Action<TArgument> onButtonClick, TArgument actionArgument) => Enqueue(text, buttonText, onButtonClick, actionArgument);
        public void Notify(string text, bool neverConsiderToBeDuplicate) => Enqueue(text, neverConsiderToBeDuplicate);
        public void Notify(string text, object buttonText, Action onButtonClick, bool pushToFront) => Enqueue(text, buttonText, onButtonClick, pushToFront);

        public void Notify<TArgument>(string text, object buttonText, Action<TArgument> onButtonClick, TArgument actionArgument, bool pushToFront) =>
            Enqueue(text, buttonText, onButtonClick, actionArgument, pushToFront);

        public void Notify<TArgument>(string text, object buttonText, Action<TArgument> onButtonClick, TArgument actionArgument, bool pushToFront, bool neverConsiderToBeDuplicate) =>
            Enqueue(text, buttonText, onButtonClick, actionArgument, pushToFront, neverConsiderToBeDuplicate);

        public void Notify(string text, object buttonText, Action<object> onButtonClick, object actionArgument, bool pushToFront, bool neverConsiderToBeDuplicate) =>
            Enqueue(text, buttonText, onButtonClick, actionArgument, pushToFront, neverConsiderToBeDuplicate);
    }
}
