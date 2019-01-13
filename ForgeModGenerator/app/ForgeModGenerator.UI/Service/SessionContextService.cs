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
        ObservableCollection<ForgeVersion> ForgeVersions { get; }
        ObservableCollection<Mod> Mods { get; }
        Mod SelectedMod { get; set; }
        Uri StartPage { get; }
    }

    public class SessionContextService : ISessionContextService
    {
        public SessionContextService()
        {
            Mods = FindMods();
            ForgeVersions = FindForgeVersions();
            StartPage = new Uri("DashboardPage.xaml", UriKind.Relative);
        }

        public Uri StartPage { get; }

        private ObservableCollection<Mod> mods;
        public ObservableCollection<Mod> Mods {
            get => mods;
            set => Set(ref mods, value);
        }

        private Mod selectedMod;
        public Mod SelectedMod {
            get => selectedMod;
            set => Set(ref selectedMod, value);
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
