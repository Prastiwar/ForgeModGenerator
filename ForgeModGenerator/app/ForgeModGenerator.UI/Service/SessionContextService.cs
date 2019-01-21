using ForgeModGenerator.Core;
using ForgeModGenerator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ForgeModGenerator.Service
{
    public interface ISessionContextService : INotifyPropertyChanged
    {
        ObservableCollection<PreferenceData> Preferences { get; }

        ObservableCollection<ForgeVersion> ForgeVersions { get; }

        ObservableCollection<Mod> Mods { get; }
        ObservableCollection<Mod> SelectedMods { get; }
        Mod SelectedMod { get; set; }

        Uri StartPage { get; }
        bool IsModSelected { get; }
    }

    public class SessionContextService : ISessionContextService
    {
        public SessionContextService()
        {
            Mods = FindMods();
            SelectedMod = Mods.Count > 0 ? Mods[0] : null;
            SelectedMods = new ObservableCollection<Mod>();
            if (SelectedMod != null)
            {
                SelectedMods.Add(SelectedMod);
            }
            ForgeVersions = FindForgeVersions();
            StartPage = new Uri("../Dashboard/DashboardPage.xaml", UriKind.Relative);
        }

        public Uri StartPage { get; }

        public bool IsModSelected => SelectedMod != null;

        private ObservableCollection<PreferenceData> preferences;
        public ObservableCollection<PreferenceData> Preferences {
            get => preferences;
            set => Set(ref preferences, value);
        }

        private ObservableCollection<Mod> mods;
        public ObservableCollection<Mod> Mods {
            get => mods;
            set => Set(ref mods, value);
        }

        private ObservableCollection<Mod> selectedMods;
        public ObservableCollection<Mod> SelectedMods {
            get => selectedMods;
            set => Set(ref selectedMods, value);
        }

        private Mod selectedMod;
        public Mod SelectedMod {
            get => selectedMod;
            set {
                Set(ref selectedMod, value);
                OnPropertyChanged(nameof(IsModSelected));
            }
        }

        private ObservableCollection<ForgeVersion> forgeVersions;
        public ObservableCollection<ForgeVersion> ForgeVersions {
            get => forgeVersions;
            set => Set(ref forgeVersions, value);
        }

        protected ObservableCollection<Mod> FindMods()
        {
            string[] paths = null;
            try
            {
                paths = Directory.GetDirectories(AppPaths.Mods);
            }
            catch (Exception)
            {
                Log.InfoBox($"Path: {AppPaths.Mods} was not found");
                throw;
            }
            List<Mod> found = new List<Mod>(paths.Length);
            foreach (string path in paths)
            {
                Mod imported = Mod.Import(path);
                if (imported != null)
                {
                    found.Add(imported);
                }
            }
            return new ObservableCollection<Mod>(found);
        }

        protected ObservableCollection<ForgeVersion> FindForgeVersions()
        {
            string[] paths = null;
            try
            {
                paths = Directory.GetFiles(AppPaths.ForgeVersions);
            }
            catch (Exception)
            {
                Log.InfoBox($"Path: {AppPaths.ForgeVersions} was not found");
                throw;
            }
            List<ForgeVersion> found = new List<ForgeVersion>(paths.Length);
            IEnumerable<string> filePaths = paths.Where(x => Path.GetExtension(x) == ".zip");
            foreach (string path in filePaths)
            {
                found.Add(new ForgeVersion(path));
            }
            return new ObservableCollection<ForgeVersion>(found);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if ((field != null && field.Equals(newValue))
                || (field == null && newValue == null))
            {
                return false;
            }
            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
