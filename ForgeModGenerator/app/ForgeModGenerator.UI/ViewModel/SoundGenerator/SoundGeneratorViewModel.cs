using ForgeModGenerator.Core;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> SoundGenerator Business ViewModel </summary>
    public class SoundGeneratorViewModel : FileListViewModelBase
    {
        public SoundGeneratorViewModel(ISessionContextService sessionContext) : base(sessionContext)
        {
            OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
            Refresh();
        }

        public override string CollectionRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        private ICommand soundClick;
        public ICommand SoundClick => soundClick ?? (soundClick = new RelayCommand<string>(OnSoundClick));

        private void OnSoundClick(string soundPath)
        {

        }
    }
}
