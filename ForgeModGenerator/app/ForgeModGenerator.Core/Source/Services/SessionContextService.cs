using ForgeModGenerator.Models;
using ForgeModGenerator.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ForgeModGenerator.Services
{
    /// <summary> Holds shared data context over whole application </summary>
    public abstract class SessionContextService : ISessionContextService
    {
        public abstract Uri StartPage { get; }

        public bool IsModSelected => SelectedMod != null;

        private Dictionary<Type, PreferenceData> Preferences { get; set; }

        private bool askBeforeClose;
        public bool AskBeforeClose {
            get => askBeforeClose;
            set => SetProperty(ref askBeforeClose, value);
        }

        private ObservableCollection<Mod> mods;
        public ObservableCollection<Mod> Mods {
            get => mods;
            set => SetProperty(ref mods, value);
        }

        private ObservableCollection<Mod> selectedMods;
        public ObservableCollection<Mod> SelectedMods {
            get => selectedMods;
            set => SetProperty(ref selectedMods, value);
        }

        private Mod selectedMod;
        public Mod SelectedMod {
            get => selectedMod;
            set {
                SetProperty(ref selectedMod, value);
                OnPropertyChanged(nameof(IsModSelected));
            }
        }

        private ObservableCollection<ForgeVersion> forgeVersions;
        public ObservableCollection<ForgeVersion> ForgeVersions {
            get => forgeVersions;
            set => SetProperty(ref forgeVersions, value);
        }

        public virtual void Refresh()
        {
            Mods = FindMods();
            ForgeVersions = FindForgeVersions();
            Preferences = FindPreferences();

            if (SelectedMod == null)
            {
                SelectedMod = Mods.Count > 0 ? Mods[0] : null;
            }
            if (SelectedMods == null)
            {
                SelectedMods = new ObservableCollection<Mod>();
                if (SelectedMod != null)
                {
                    SelectedMods.Add(SelectedMod);
                }
            }
        }

        public T GetPreferences<T>() where T : PreferenceData
        {
            Type type = typeof(T);
            if (Preferences.TryGetValue(type, out PreferenceData data))
            {
                return (T)data;
            }
            return null;
        }

        public T GetOrCreatePreferences<T>() where T : PreferenceData
        {
            T preferences = GetPreferences<T>();
            if (preferences == null)
            {
                preferences = Activator.CreateInstance<T>();
                preferences.SavePreferences();
            }
            return preferences;
        }

        protected ObservableCollection<Mod> FindMods()
        {
            string[] paths = Directory.GetDirectories(AppPaths.Mods);
            List<Mod> found = new List<Mod>(paths.Length);
            foreach (string path in paths)
            {
                if (TryGetModFromPath(path, out Mod imported))
                {
                    found.Add(imported);
                    Log.Info($"Mod {imported.ModInfo.Name} detected");
                }
            }
            return new ObservableCollection<Mod>(found);
        }

        protected ObservableCollection<ForgeVersion> FindForgeVersions()
        {
            string[] paths = Directory.GetFiles(AppPaths.ForgeVersions);
            List<ForgeVersion> found = new List<ForgeVersion>(paths.Length);
            IEnumerable<string> filePaths = paths.Where(x => Path.GetExtension(x) == ".zip");
            foreach (string path in filePaths)
            {
                ForgeVersion version = new ForgeVersion(path);
                found.Add(version);
                Log.Info($"Forge Version {version.Name} detected");
            }
            return new ObservableCollection<ForgeVersion>(found);
        }

        protected Dictionary<Type, PreferenceData> FindPreferences()
        {
            Dictionary<Type, PreferenceData> dictionary = new Dictionary<Type, PreferenceData>();
            foreach (string filePath in Directory.EnumerateFiles(AppPaths.Preferences))
            {
                if (TryGetPreferencesFromFilePath(filePath, out PreferenceData preferences))
                {
                    dictionary.Add(preferences.GetType(), preferences);
                }
            }
            return dictionary;
        }

        protected abstract bool TryGetModFromPath(string path, out Mod mod);

        protected abstract bool TryGetPreferencesFromFilePath(string json, out PreferenceData preferences);

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
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
    }
}
