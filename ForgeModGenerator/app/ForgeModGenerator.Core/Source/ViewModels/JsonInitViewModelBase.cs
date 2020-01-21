//using ForgeModGenerator.Models;
//using ForgeModGenerator.Utility;
//using ForgeModGenerator.Validation;
//using Prism.Commands;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Collections.Specialized;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Windows.Input;

//namespace ForgeModGenerator.ViewModels
//{
//    public abstract class JsonInitViewModelBase<TModel> : SimpleInitViewModelBase<TModel>
//        where TModel : ObservableDirtyObject, IValidable
//    {
//        public JsonInitViewModelBase(GeneratorContext<TModel> context) : base(context) { }

//        private bool isFileUpdateAvailable;
//        public bool IsFileUpdateAvailable {
//            get => isFileUpdateAvailable;
//            set => SetProperty(ref isFileUpdateAvailable, value);
//        }

//        protected override IEnumerable<TModel> FindModelsFromFile(string filePath)
//        {
//            if (!File.Exists(filePath))
//            {
//                return Enumerable.Empty<TModel>();
//            }
//        }

//        /// <summary> Deserialized folders from FoldersJsonFilePath and checks if any file doesn't exists, if so, prompt if should fix this </summary>
//        protected async void CheckJsonFileMismatch()
//        {
//            IEnumerable<TFolder> deserializedFolders = Explorer.FileSynchronizer.Finder.FindFoldersFromFile(FoldersJsonFilePath, false);
//            bool hasNotExistingFile = deserializedFolders != null ? deserializedFolders.Any(folder => folder.Files.Any(file => !File.Exists(file.Info.FullName))) : false;
//            if (hasNotExistingFile)
//            {
//                string fileName = Path.GetFileName(FoldersJsonFilePath);
//                string questionMessage = $"{fileName} file has occurencies that doesn't exist in root folder. Do you want to fix it and overwrite {fileName}? ";
//                bool shouldFix = await DialogService.ShowMessage(questionMessage, "Conflict found", "Yes", "No", null).ConfigureAwait(true);
//                if (shouldFix)
//                {
//                    RegenerateCode();
//                }
//            }
//            foreach (TFolder folder in deserializedFolders)
//            {
//                folder.Clear(); // dereference file paths
//            }
//        }

//        /// <summary> Adds found files that are not referenced in json file </summary>
//        protected void RegenerateJsonWithExplorer()
//        {
//            Explorer.FileSynchronizer.AddNotReferencedFiles();
//            CheckForUpdate();
//            RegenerateCode();
//        }
//    }
//}
