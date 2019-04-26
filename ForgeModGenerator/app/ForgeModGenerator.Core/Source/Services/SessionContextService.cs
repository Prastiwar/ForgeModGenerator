using ForgeModGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        private bool askBeforeClose;
        public bool AskBeforeClose {
            get => askBeforeClose;
            set => SetProperty(ref askBeforeClose, value);
        }

        private ObservableCollection<McMod> mods;
        public ObservableCollection<McMod> Mods {
            get => mods;
            set => SetProperty(ref mods, value);
        }

        private ObservableCollection<McMod> selectedMods;
        public ObservableCollection<McMod> SelectedMods {
            get => selectedMods;
            set => SetProperty(ref selectedMods, value);
        }

        private McMod selectedMod;
        public McMod SelectedMod {
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

            if (SelectedMod == null)
            {
                SelectedMod = Mods.Count > 0 ? Mods[0] : null;
            }
            if (SelectedMods == null)
            {
                SelectedMods = new ObservableCollection<McMod>();
                if (SelectedMod != null)
                {
                    SelectedMods.Add(SelectedMod);
                }
            }
        }

        public void DownloadNewForgeVersion()
        {
            Process.Start("https://files.minecraftforge.net/"); // to install version
            Process.Start(AppPaths.ForgeVersions); // paste zip there
        }

        protected ObservableCollection<McMod> FindMods()
        {
            string[] paths = Directory.GetDirectories(AppPaths.Mods);
            List<McMod> found = new List<McMod>(paths.Length);
            foreach (string path in paths)
            {
                if (TryGetModFromPath(path, out McMod imported))
                {
                    found.Add(imported);
                    Log.Info($"Mod {imported.ModInfo.Name} detected");
                }
            }
            return new ObservableCollection<McMod>(found);
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

        protected abstract bool TryGetModFromPath(string path, out McMod mcMod);

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
