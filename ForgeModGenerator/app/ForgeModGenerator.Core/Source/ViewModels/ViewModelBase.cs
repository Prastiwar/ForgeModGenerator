using ForgeModGenerator.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        public ViewModelBase(ISessionContextService sessionContext)
        {
            SessionContext = sessionContext;
            SessionContext.PropertyChanged += OnSessionContextPropertyChanged;
        }

        public ISessionContextService SessionContext { get; }

        private bool isLoading;
        public bool IsLoading {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        private ICommand onLoadedCommand;
        public ICommand OnLoadedCommand => onLoadedCommand ?? (onLoadedCommand = new DelegateCommand(OnLoaded));

        protected virtual void OnLoaded() => Refresh();

        protected virtual bool CanRefresh() => SessionContext.SelectedMod != null;

        public abstract Task<bool> Refresh();

        protected virtual async void OnSessionContextPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SessionContext.SelectedMod))
            {
                await Refresh().ConfigureAwait(false);
            }
        }
    }
}
