using ForgeModGenerator.Core;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight.Command;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> SoundGenerator Business ViewModel </summary>
    public class SoundGeneratorViewModel : FileListViewModelBase
    {
        public static string FormatDottedSoundName(string modid, string path) => FormatSoundName(modid, path).Replace('/', '.');
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
        public SoundGeneratorViewModel(ISessionContextService sessionContext) : base(sessionContext)
        {
            OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
            Refresh();
            ShouldUpdate = IsUpdateAvailable();
        }

        public override string CollectionRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        private bool shouldUpdate;
        public bool ShouldUpdate {
            get => shouldUpdate;
            set => Set(ref shouldUpdate, value);
        }

        private ICommand soundClick;
        public ICommand SoundClick => soundClick ?? (soundClick = new RelayCommand<string>(OnSoundClick));

        private void OnSoundClick(string soundPath)
        {
            System.Diagnostics.Process.Start(soundPath);
        }

        private ICommand updateSoundsJson;
        public ICommand UpdateSoundsJson => updateSoundsJson ?? (updateSoundsJson = new RelayCommand(ForceJsonUpdate));

        private void ForceJsonUpdate()
        {
            foreach (FileCollection file in Files)
            {
                foreach (string path in file.Paths)
                {
                    string formatName = FormatSoundName(SessionContext.SelectedMod.ModInfo.Modid, path);
                    string dotformatName = formatName.Replace('/', '.');
                }
            }
            //ShouldUpdate = false;
            MessageBox.Show("Updated successfully");
        }

        private bool IsUpdateAvailable()
        {
            string jsonPath = ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            if (File.Exists(jsonPath) && Files.Count > 0)
            {
                foreach (FileCollection file in Files)
                {
                    foreach (string path in file.Paths)
                    {
                        string formatName = FormatSoundName(SessionContext.SelectedMod.ModInfo.Modid, path);
                        if (!File.ReadLines(jsonPath).Any(line => line.Contains(formatName)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
