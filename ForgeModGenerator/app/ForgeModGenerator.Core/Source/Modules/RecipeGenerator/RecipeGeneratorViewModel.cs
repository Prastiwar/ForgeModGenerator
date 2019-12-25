using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.ViewModels;
using Prism.Commands;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgeModGenerator.RecipeGenerator.ViewModels
{
    /// <summary> RecipeGenerator Business ViewModel </summary>
    public class RecipeGeneratorViewModel : FoldersWatcherViewModelBase<ObservableFolder<Recipe>, Recipe>
    {
        public RecipeGeneratorViewModel(GeneratorContext<Recipe> context,
                                        IFoldersExplorerFactory<ObservableFolder<Recipe>, Recipe> factory,
                                        ISerializer<Recipe> recipeSerializer)
            : base(context, factory)
        {
            RecipeSerializer = recipeSerializer;

            Explorer.OpenFileDialog.Filter = "Json file (*.json) | *.json";
            Explorer.OpenFileDialog.Multiselect = true;
            Explorer.OpenFileDialog.CheckFileExists = true;
            Explorer.OpenFileDialog.ValidateNames = true;
            Explorer.OpenFolderDialog.ShowNewFolderButton = true;
            Explorer.AllowFileExtensions(".json");

            Explorer.FileSynchronizer.SyncFilter = NotifyFilter.File;
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null
            ? ModPaths.RecipesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid)
            : null;

        protected ISerializer<Recipe> RecipeSerializer { get; }

        private ICommand openRecipeEditorCommand;
        public ICommand OpenRecipeEditorCommand => openRecipeEditorCommand ?? (openRecipeEditorCommand = new DelegateCommand<ObservableFolder<Recipe>>(EditAndGenerateNewRecipe));

        private string tempFilePath;

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                IsLoading = true;
                Explorer.Folders.Clear();
                await InitializeFoldersAsync(await Explorer.FileSynchronizer.Finder.FindFoldersAsync(FoldersRootPath, true).ConfigureAwait(true)).ConfigureAwait(false);
                Explorer.FileSynchronizer.RootPath = FoldersRootPath;
                Explorer.FileSynchronizer.SetEnableSynchronization(true);
                SubscribeFolderEvents(Explorer.Folders, new FileChangedEventArgs<ObservableFolder<Recipe>>(Explorer.Folders.Files, FileChange.Add));
                RegenerateCode();
                IsLoading = false;
                return true;
            }
            return false;
        }

        private void EditAndGenerateNewRecipe(ObservableFolder<Recipe> folder)
        {
            tempFilePath = Path.GetTempFileName();
            RecipeCreator newRecipe = new RecipeCreator(tempFilePath) {
                Name = "NewRecipe",
            };
            newRecipe.Ingredients.Add(new Ingredient());
            newRecipe.IsDirty = false;
            newRecipe.ValidateProperty += (sender, propertyName) => OnValidate(sender, folder.Files, propertyName);
            Context.Validator.SetDefaultRepository(folder.Files);
            EditorForm.OpenItemEditor(newRecipe);
        }

        protected override void OnItemEdited(object sender, ItemEditedEventArgs<Recipe> e)
            => OnRecipeCreatorEdited(sender, new ItemEditedEventArgs<RecipeCreator>(e.Result, (RecipeCreator)e.CachedItem, (RecipeCreator)e.ActualItem));

        protected void OnRecipeCreatorEdited(object sender, ItemEditedEventArgs<RecipeCreator> e)
        {
            if (e.Result)
            {
                if (e.ActualItem.Validate().IsValid)
                {
                    Recipe recipe = e.ActualItem.Create();
                    string json = RecipeSerializer.Serialize(recipe, true);
                    string path = Path.Combine(ModPaths.RecipesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Modid), recipe.Name + ".json");
                    if (File.Exists(path))
                    {
                        throw new IOException($"File {path} already exists");
                    }
                    File.AppendAllText(path, json);
                }
            }
            FileInfo tempFile = new FileInfo(tempFilePath);
            if (tempFile.Exists)
            {
                tempFile.Delete();
            }
        }

        protected void SubscribeFolderEvents(object sender, FileChangedEventArgs<ObservableFolder<Recipe>> e)
        {
            if (e.Change == FileChange.Add)
            {
                foreach (ObservableFolder<Recipe> folder in e.Files)
                {
                    folder.FilesChanged += OnFilesChanged;
                }
            }
            else if (e.Change == FileChange.Remove)
            {
                foreach (ObservableFolder<Recipe> folder in e.Files)
                {
                    folder.FilesChanged -= OnFilesChanged;
                }
            }
        }

        protected virtual void OnFilesChanged(object sender, FileChangedEventArgs<Recipe> e) => RegenerateCode();

        protected override void RegenerateCode()
        {
            McMod mod = SessionContext.SelectedMod;
            Context.CodeGenerationService.RegenerateInitScript(SourceCodeLocator.Recipes(mod.ModInfo.Name, mod.Organization).ClassName, mod, Explorer.Folders.Files[0].Files);
        }
    }
}
