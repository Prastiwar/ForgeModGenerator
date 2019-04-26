using ForgeModGenerator.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ForgeModGenerator.Services
{
    public interface ISessionContextService : INotifyPropertyChanged
    {
        ObservableCollection<ForgeVersion> ForgeVersions { get; }

        ObservableCollection<McMod> Mods { get; }
        ObservableCollection<McMod> SelectedMods { get; }
        McMod SelectedMod { get; set; }

        Uri StartPage { get; }
        bool IsModSelected { get; }

        bool AskBeforeClose { get; set; }

        void Refresh();
        void DownloadNewForgeVersion();
    }
}
