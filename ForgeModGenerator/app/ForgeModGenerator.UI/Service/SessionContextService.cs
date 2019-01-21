using ForgeModGenerator.Core;
using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.Model;
using Newtonsoft.Json;
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
        ObservableCollection<ForgeVersion> ForgeVersions { get; }

        ObservableCollection<Mod> Mods { get; }
        ObservableCollection<Mod> SelectedMods { get; }
        Mod SelectedMod { get; set; }

        Uri StartPage { get; }
        bool IsModSelected { get; }

        T GetPreferences<T>() where T : PreferenceData;
    }

    public class SessionContextService : ISessionContextService
    {
        public SessionContextService()
        {
            Log.Info("Session loading..");
            Mods = FindMods();
            SelectedMod = Mods.Count > 0 ? Mods[0] : null;
            SelectedMods = new ObservableCollection<Mod>();
            if (SelectedMod != null)
            {
                SelectedMods.Add(SelectedMod);
            }
            ForgeVersions = FindForgeVersions();
            StartPage = new Uri("../Dashboard/DashboardPage.xaml", UriKind.Relative);

            Preferences = FindPreferences();
            Log.Info("Session loaded");
        }

        public Uri StartPage { get; }

        public bool IsModSelected => SelectedMod != null;

        protected Dictionary<Type, PreferenceData> Preferences { get; set; }

        public T GetPreferences<T>() where T : PreferenceData
        {
            Type type = typeof(T);
            if (Preferences.TryGetValue(type, out PreferenceData data))
            {
                return (T)data;
            }
            return null;
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
            string[] paths = Directory.GetDirectories(AppPaths.Mods);
            List<Mod> found = new List<Mod>(paths.Length);
            foreach (string path in paths)
            {
                Mod imported = Mod.Import(path);
                if (imported != null)
                {
                    found.Add(imported);
                    Log.Info($"Mod {imported.ModInfo.Name} loaded");
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
                Log.Info($"Forge version {version.Name} detected");
            }
            return new ObservableCollection<ForgeVersion>(found);
        }

        private Dictionary<Type, PreferenceData> FindPreferences()
        {
            Dictionary<Type, PreferenceData> dictionary = new Dictionary<Type, PreferenceData>();
            string[] filePaths = Directory.GetFiles(AppPaths.Preferences);
            foreach (string filePath in filePaths)
            {
                string typeName = Path.GetFileNameWithoutExtension(filePath);
                string jsonText = File.ReadAllText(filePath);
                Type type = Type.GetType(typeName);
                try
                {
                    dictionary.Add(type, (PreferenceData)JsonConvert.DeserializeObject(jsonText, type));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to load preferences for {type}");
                    throw;
                }
            }
            return dictionary;
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
