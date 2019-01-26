using ForgeModGenerator.Core;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> SoundGenerator Business ViewModel </summary>
    public class SoundGeneratorViewModel : FileListViewModelBase<SoundEvent>
    {
        public override string CollectionRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

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
        public static string FormatDottedSoundName(string modid, string path) => FormatSoundName(modid, path).Replace('/', '.').Remove(0, modid.Length + 1);

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
                //WriteFilsToJson(soundPath, Preferences.ShouldGeneratePrettyJson);
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
            //string jsonPath = ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            //string tempJsonPath = jsonPath.Replace("sounds.json", "temp.json");
            //bool prettyPrint = Preferences.ShouldGeneratePrettyJson;

            //if (!File.Exists(jsonPath))
            //{
            //    File.AppendAllText(jsonPath, prettyPrint ? "{\n}" : "{}");
            //}

            //using (StreamWriter writer = new StreamWriter(tempJsonPath))
            //{
            //    using (StreamReader reader = new StreamReader(jsonPath))
            //    {
            //        IEnumerable<IEnumerable<string>> missingFiles = Files.Select(file => file.Where(path => !HasSoundWritten(path)));
            //        WriteFilesToJson(missingFiles, prettyPrint, writer, reader);
            //    }
            //}
            //File.Delete(jsonPath);
            //File.Move(tempJsonPath, jsonPath);
            //ShouldUpdate = false;
            //Log.Info("sounds.json Updated successfully", true);
        }

        // Removes close line and adds new comma
        private static void PrepareJsonForAddElements(StreamWriter writer, StreamReader reader, bool prettyPrint)
        {
            //if (prettyPrint)
            //{
            //    long index = 0L;
            //    long length = reader.BaseStream.GetLineCount();
            //    string line = null;

            //    while ((line = reader.ReadLine()) != null)
            //    {
            //        if (index < length - 2)
            //        {
            //            writer.WriteLine(line);
            //        }
            //        else
            //        {
            //            writer.Write(line + ",");
            //            break;
            //        }
            //        index++;
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < reader.BaseStream.Length - 1; i++)
            //    {
            //        char currentChar = (char)reader.Read();
            //        writer.Write(currentChar);
            //    }
            //    writer.Write(',');
            //}
        }

        private bool HasSoundWritten(string sound)
        {
            //string jsonPath = ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            //string formattedName = FormatSoundName(SessionContext.SelectedMod.ModInfo.Modid, sound);
            //return File.ReadLines(jsonPath).Any(x => x.Contains(formattedName));
            return false;
        }

        private void WriteFilesToJson(IEnumerable<IEnumerable<string>> missingFiles, bool prettyPrint, StreamWriter writer, StreamReader reader)
        {
            //PrepareJsonForAddElements(writer, reader, prettyPrint);

            //int filesCount = missingFiles.Count();
            //int filesIndex = 0;
            //foreach (IEnumerable<string> files in missingFiles)
            //{
            //    bool isLastFiles = filesIndex == filesCount - 1;

            //    int pathIndex = 0;
            //    int pathCount = files.Count();

            //    foreach (string path in files)
            //    {
            //        bool isLastPath = pathIndex == pathCount - 1;
            //        string formatName = FormatSoundName(SessionContext.SelectedMod.ModInfo.Modid, path);
            //        string dotformatName = formatName.Replace('/', '.').Remove(0, SessionContext.SelectedMod.ModInfo.Modid.Length + 1);
            //        string template = Preferences.SoundTemplate;
            //        if (!prettyPrint)
            //        {
            //            template.Replace(" ", "").Replace("\n", "").Replace("\r", "");
            //        }
            //        // TODO: Write template from preferences
            //        //template.Replace();
            //        //writer.Write(template);
            //        writer.WriteLine("");
            //        writer.Write('"');
            //        writer.Write(dotformatName);
            //        writer.Write("\": {\n");
            //        writer.Write("\"sounds\": [\n");
            //        writer.WriteLine("{{ \"name\": \"{0}\" }}", formatName);
            //        writer.WriteLine("]");
            //        writer.WriteLine(isLastFiles && isLastPath ? "}" : "},");
            //        pathIndex++;
            //    }
            //    filesIndex++;
            //}
            //writer.Write(prettyPrint ? "\n}" : "}"); // close json
        }

        private bool IsUpdateAvailable()
        {
            //string jsonPath = ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            //if (File.Exists(jsonPath) && Files.Count > 0)
            //{
            //    foreach (FileCollection file in Files)
            //    {
            //        foreach (string path in file)
            //        {
            //            string formatName = FormatSoundName(SessionContext.SelectedMod.ModInfo.Modid, path);
            //            if (!File.ReadLines(jsonPath).Any(line => line.Contains(formatName)))
            //            {
            //                Log.Info($"Detected update available to sounds.json for {SessionContext.SelectedMod.ModInfo.Name}");
            //                return true;
            //            }
            //        }
            //    }
            //}
            return false;
        }
    }
}
