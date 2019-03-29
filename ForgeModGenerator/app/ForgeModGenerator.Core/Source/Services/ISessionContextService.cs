using ForgeModGenerator.Models;
using ForgeModGenerator.Persistence;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ForgeModGenerator.Services
{
    public interface ISessionContextService : INotifyPropertyChanged
    {
        void Refresh();

        ObservableCollection<ForgeVersion> ForgeVersions { get; }

        ObservableCollection<Mod> Mods { get; }
        ObservableCollection<Mod> SelectedMods { get; }
        Mod SelectedMod { get; set; }

        Uri StartPage { get; }
        bool IsModSelected { get; }

        bool AskBeforeClose { get; set; }

        T GetPreferences<T>() where T : PreferenceData;
        T GetOrCreatePreferences<T>() where T : PreferenceData;
    }
}
