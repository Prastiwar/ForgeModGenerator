using ForgeModGenerator.Core;
using ForgeModGenerator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace ForgeModGenerator.Service
{
    public interface ISessionContextService : INotifyPropertyChanged
    {
        ObservableCollection<Mod> Mods { get; }
        Mod SelectedMod { get; set; }
        Uri StartPage { get; }
    }

    public class SessionContextService : ISessionContextService
    {
        public SessionContextService()
        {
            Mods = FindMods();
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

        protected ObservableCollection<Mod> FindMods()
        {
            string[] modPaths = null;
            try
            {
                modPaths = Directory.GetDirectories(AppPaths.Mods);
            }
            catch (Exception)
            {
                Log.InfoBox($"Path: {AppPaths.Mods} was not found");
                throw;
            }
            List<Mod> foundMods = new List<Mod>(modPaths.Length);
            foreach (string modPath in modPaths)
            {
                Mod importedMod = Mod.Import(modPath);
                if (importedMod != null)
                {
                    foundMods.Add(importedMod);
                }
            }
            return new ObservableCollection<Mod>(foundMods);
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
