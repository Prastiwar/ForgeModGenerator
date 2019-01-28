using ForgeModGenerator.Core;
using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> SoundGenerator Business ViewModel </summary>
    public class SoundGeneratorViewModel : FileListViewModelBase<SoundEvent>
    {
        public override string CollectionRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public SoundsGeneratorPreferences Preferences { get; }

        public SoundGeneratorViewModel(ISessionContextService sessionContext) : base(sessionContext)
        {
            OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
            AllowedExtensions = new string[] { ".ogg" };
            FileEditForm = new UserControls.SoundEditForm();
            Preferences = sessionContext.GetPreferences<SoundsGeneratorPreferences>();
            if (Preferences == null)
            {
                Preferences = SoundsGeneratorPreferences.Default;
            }
            Refresh();
            OnFileAdded += AddSoundToJson;
            OnFileRemoved += RemoveSoundFromJson;
        }

        // Get formatted sound from full path, "shorten.path.toFile"
        public static string FormatDottedSoundName(string path)
        {
            int startIndex = path.IndexOf("sounds") + 7;
            if (startIndex == -1)
            {
                return null;
            }
            return Path.ChangeExtension(path.Substring(startIndex, path.Length - startIndex), null);
        }

        // Get formatted sound from full path, "modid:shorten/path/toFile"
        public static string FormatSoundName(string modid, string path)
        {
            int startIndex = path.IndexOf("sounds") + 7;
            if (startIndex == -1)
            {
                return null;
            }
            string shortPath = Path.ChangeExtension(path.Substring(startIndex, path.Length - startIndex), null);
            return $"{modid}:{shortPath}";
        }

        private void AddSoundToJson(object item)
        {
            SoundEvent sound = (SoundEvent)item;
            if (!HasSoundWritten(sound.FilePath))
            {
                // TODO: Add sound
            }
        }

        private void RemoveSoundFromJson(object item)
        {
            SoundEvent sound = (SoundEvent)item;
            if (HasSoundWritten(sound.FilePath))
            {
                // TODO: Remove sound
            }
        }

        protected override void FileCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.FileCollection_CollectionChanged(sender, e);
            ShouldUpdate = CanRefresh() ? IsUpdateAvailable() : false;
        }

        protected override bool Refresh()
        {
            bool canRefresh = base.Refresh();
            ShouldUpdate = canRefresh ? IsUpdateAvailable() : false;
            return canRefresh;
        }

        private bool shouldUpdate;
        public bool ShouldUpdate {
            get => shouldUpdate;
            set => Set(ref shouldUpdate, value);
        }

        private ICommand updateSoundsJson;
        public ICommand UpdateSoundsJson => updateSoundsJson ?? (updateSoundsJson = new RelayCommand(ForceJsonUpdate));

        protected override void OnEdited(bool result, IFileItem file)
        {
            if (result)
            {
                // TODO: Save changes
            }
        }

        private void ForceJsonUpdate()
        {
        }

        private bool HasSoundWritten(string sound)
        {
            return false;
        }

        private bool IsUpdateAvailable()
        {
            return false;
        }

        protected override ObservableCollection<FileList<SoundEvent>> FindCollection(string path, bool createRootIfEmpty)
        {
            ObservableCollection<FileList<SoundEvent>> foundCollection = new ObservableCollection<FileList<SoundEvent>>();
            if (!File.Exists(path))
            {
                File.AppendAllText(path, "{}");
                return foundCollection;
            }
            Converter.SoundCollectionConverter converter = new Converter.SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            try
            {
                FileList<SoundEvent> allSounds = JsonConvert.DeserializeObject<FileList<SoundEvent>>(File.ReadAllText(path), converter);
                foundCollection.Add(allSounds);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Couldn't load sounds.json");
                throw;
            }
            return foundCollection;
        }
    }
}
