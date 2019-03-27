using ForgeModGenerator.Models;
using ForgeModGenerator.Persistence;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

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
        IEnumerable<Formatting> FormattingTypes { get; }
    }

    public sealed class SessionContextService : ISessionContextService
    {
        public static SessionContextService Instance { get; }
        static SessionContextService() => Instance = new SessionContextService();

        private SessionContextService()
        {
            Log.Info("Session loading..");
            StartPage = new Uri("../ApplicationModule/DashboardPage.xaml", UriKind.Relative);
            Refresh();
            Log.Info("Session loaded");
        }

        public void Refresh()
        {
            Log.Info("Session refreshing..");
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
            Log.Info("Session refreshed");
        }

        public Uri StartPage { get; }

        public bool IsModSelected => SelectedMod != null;

        private Dictionary<Type, PreferenceData> Preferences { get; set; }

        public IEnumerable<Formatting> FormattingTypes => Enum.GetValues(typeof(Formatting)).Cast<Formatting>();

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

        public T GetPreferences<T>() where T : PreferenceData
        {
            Type type = typeof(T);
            if (Preferences.TryGetValue(type, out PreferenceData data))
            {
                return (T)data;
            }
            Log.Info($"Tried to get {type} preferences, but it doesn't exist");
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

        private ObservableCollection<Mod> FindMods()
        {
            string[] paths = Directory.GetDirectories(AppPaths.Mods);
            List<Mod> found = new List<Mod>(paths.Length);
            foreach (string path in paths)
            {
                Mod imported = ModHelper.ImportMod(path);
                if (imported != null)
                {
                    found.Add(imported);
                    Log.Info($"Mod {imported.ModInfo.Name} loaded");
                }
            }
            return new ObservableCollection<Mod>(found);
        }

        private ObservableCollection<ForgeVersion> FindForgeVersions()
        {
            string[] paths = Directory.GetFiles(AppPaths.ForgeVersions);
            List<ForgeVersion> found = new List<ForgeVersion>(paths.Length);
            IEnumerable<string> filePaths = paths.Where(x => Path.GetExtension(x) == ".zip");
            foreach (string path in filePaths)
            {
                ForgeVersion version = new ForgeVersion(path);
                found.Add(version);
                Log.Info($"Forge version {version.Name} detected");
            }
            return new ObservableCollection<ForgeVersion>(found);
        }

        private Dictionary<Type, PreferenceData> FindPreferences()
        {
            Dictionary<Type, PreferenceData> dictionary = new Dictionary<Type, PreferenceData>();
            foreach (string filePath in Directory.EnumerateFiles(AppPaths.Preferences))
            {
                string jsonText = File.ReadAllText(filePath);
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings() {
                        TypeNameHandling = TypeNameHandling.All
                    };
                    object preferences = JsonConvert.DeserializeObject(jsonText, settings);
                    dictionary.Add(preferences.GetType(), (PreferenceData)preferences);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to load preferences {filePath}");
                    throw;
                }
            }
            return dictionary;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
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
