using ForgeModGenerator.Core;
using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> SoundGenerator Business ViewModel </summary>
    public class SoundGeneratorViewModel : FileListViewModelBase
    {
        public override string CollectionRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public SoundsGeneratorPreferences Preferences { get; }

        public SoundGeneratorViewModel(ISessionContextService sessionContext) : base(sessionContext)
        {
            OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
            AllowedExtensions = new string[] { ".ogg" };
            Preferences = sessionContext.GetPreferences<SoundsGeneratorPreferences>();
            Refresh();
        }

        protected override void FileCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ShouldUpdate = CanRefresh() ? IsUpdateAvailable() : false;
        }

        protected override bool Refresh()
        {
            bool canRefresh = base.Refresh();
            ShouldUpdate = canRefresh ? IsUpdateAvailable() : false;
            return canRefresh;
        }

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
            string jsonPath = ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            string tempJsonPath = jsonPath.Replace("sounds.json", "temp.json");

            if (!File.Exists(jsonPath))
            {
                using (StreamWriter json = File.CreateText(jsonPath))
                {
                    json.WriteLine("{");
                    json.WriteLine("}");
                }
            }

            using (StreamWriter writer = new StreamWriter(tempJsonPath))
            {
                using (StreamReader reader = new StreamReader(jsonPath))
                {
                    long length = reader.BaseStream.GetLineCount();
                    bool prettyPrint = length > 1;

                    PrepareJsonForAddElements(writer, reader, prettyPrint);

                    IEnumerable<IEnumerable<string>> missingFiles = Files.Select(file => file.Where(path => !File.ReadLines(jsonPath).Any(x => x.Contains(FormatSoundName(SessionContext.SelectedMod.ModInfo.Modid, path)))));
                    WriteFilesToJson(prettyPrint, writer, missingFiles);
                    writer.Write(prettyPrint ? "}" : "\n}");
                };
            }
            File.Delete(jsonPath);
            File.Move(tempJsonPath, jsonPath);
            ShouldUpdate = false;
            Log.Info("sounds.json Updated successfully", true);
        }

        private static void PrepareJsonForAddElements(StreamWriter writer, StreamReader reader, bool prettyPrint)
        {
            if (prettyPrint)
            {
                long index = 0L;
                long length = reader.BaseStream.GetLineCount();
                string line = null;

                while ((line = reader.ReadLine()) != null)
                {
                    if (index < length - 2)
                    {
                        writer.WriteLine(line);
                    }
                    else
                    {
                        writer.Write(line + ",");
                        break;
                    }
                    index++;
                }
            }
            else
            {
                for (int i = 0; i < reader.BaseStream.Length - 1; i++)
                {
                    char currentChar = (char)reader.Read();
                    writer.Write(currentChar);
                }
                writer.Write(',');
            }
        }

        private void WriteFilesToJson(bool prettyPrint, StreamWriter writer, IEnumerable<IEnumerable<string>> missingFiles)
        {
            if (prettyPrint)
            {

            }
            else
            {
                foreach (IEnumerable<string> files in missingFiles)
                {
                    int filesCount = missingFiles.Count();
                    int filesIndex = 0;
                    bool isLastFiles = filesIndex == filesCount - 1;

                    int pathIndex = 0;
                    int pathCount = files.Count();

                    foreach (string path in files)
                    {
                        bool isLastPath = pathIndex == pathCount - 1;
                        string formatName = FormatSoundName(SessionContext.SelectedMod.ModInfo.Modid, path);
                        string dotformatName = formatName.Replace('/', '.');
                        writer.WriteLine("");
                        writer.Write('"');
                        writer.Write(dotformatName);
                        writer.Write("\": {");
                        writer.WriteLine("\"sounds\": [");
                        writer.WriteLine("{{ \"name\": \"{0}:{1}\" }}", SessionContext.SelectedMod.ModInfo.Modid, formatName);
                        writer.WriteLine("]");
                        writer.WriteLine(isLastFiles && isLastPath ? "}" : "},");
                        pathIndex++;
                    }
                }
            }
        }

        private bool IsUpdateAvailable()
        {
            string jsonPath = ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            if (File.Exists(jsonPath) && Files.Count > 0)
            {
                foreach (FileCollection file in Files)
                {
                    foreach (string path in file)
                    {
                        string formatName = FormatSoundName(SessionContext.SelectedMod.ModInfo.Modid, path);
                        if (!File.ReadLines(jsonPath).Any(line => line.Contains(formatName)))
                        {
                            Log.Info($"Detected update available to sounds.json for {SessionContext.SelectedMod.ModInfo.Name}");
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
