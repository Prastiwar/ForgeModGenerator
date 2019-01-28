using ForgeModGenerator.Core;
using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
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

        private void AddSoundToJson(object item)
        {
            SoundEvent sound = (SoundEvent)item;
            if (!HasSoundWritten(sound.EventName))
            {
                // TODO: Add sound
            }
            ForceJsonUpdate();
        }

        private void RemoveSoundFromJson(object item)
        {
            SoundEvent sound = (SoundEvent)item;
            if (HasSoundWritten(sound.EventName))
            {
                // TODO: Remove sound
            }
            ForceJsonUpdate();
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
                ForceJsonUpdate();
            }
        }

        private void ForceJsonUpdate()
        {
            Converter.SoundCollectionConverter converter = new Converter.SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            string json = JsonConvert.SerializeObject(Files, Formatting.Indented, converter);
            File.WriteAllText(CollectionRootPath, json);
        }

        private bool HasSoundWritten(string soundEventName)
        {
            return File.ReadLines(CollectionRootPath).Any(x => x.Contains(soundEventName));
        }

        private bool IsUpdateAvailable()
        {
            Converter.SoundCollectionConverter converter = new Converter.SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            FileList<SoundEvent> allSounds = JsonConvert.DeserializeObject<FileList<SoundEvent>>(File.ReadAllText(CollectionRootPath), converter);
            return false;
        }

        protected override bool CanRefresh() => SessionContext.SelectedMod != null;

        protected override ObservableCollection<FileList<SoundEvent>> FindCollection(string path, bool createRootIfEmpty)
        {
            string soundsFolder = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            ObservableCollection<FileList<SoundEvent>> rootCollection = null;
            if (!File.Exists(path))
            {
                File.AppendAllText(path, "{}");
                return createRootIfEmpty ? CreateEmptyRoot(soundsFolder) : rootCollection;
            }
            Converter.SoundCollectionConverter converter = new Converter.SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            try
            {
                rootCollection = JsonConvert.DeserializeObject<ObservableCollection<FileList<SoundEvent>>>(File.ReadAllText(path), converter);
                if (createRootIfEmpty && (rootCollection == null || rootCollection.Count <= 0))
                {
                    rootCollection = CreateEmptyRoot(soundsFolder);
                }
                return rootCollection;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Couldn't load sounds.json");
                throw;
            }
        }
    }
}
