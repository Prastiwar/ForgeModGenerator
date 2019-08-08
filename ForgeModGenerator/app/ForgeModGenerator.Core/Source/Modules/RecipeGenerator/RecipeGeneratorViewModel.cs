using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.Services;
using ForgeModGenerator.Validation;
using ForgeModGenerator.ViewModels;
using Prism.Commands;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgeModGenerator.RecipeGenerator.ViewModels
{
    /// <summary> RecipeGenerator Business ViewModel </summary>
    public class RecipeGeneratorViewModel : FoldersWatcherViewModelBase<ObservableFolder<Recipe>, Recipe>
    {
        public RecipeGeneratorViewModel(ISessionContextService sessionContext,
                                        IFoldersExplorerFactory<ObservableFolder<Recipe>, Recipe> factory,
                                        IEditorFormFactory<RecipeCreator> editorFormFactory,
                                        IUniqueValidator<Recipe> recipeValidator,
                                        ISerializer<Recipe> recipeSerializer,
                                        ICodeGenerationService codeGenerationService)
            : base(sessionContext, factory)
        {
            RecipeValidator = recipeValidator;
            RecipeSerializer = recipeSerializer;
            CodeGenerationService = codeGenerationService;
            EditorForm = editorFormFactory.Create();
            EditorForm.ItemEdited += CreateRecipe;

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

        protected ICodeGenerationService CodeGenerationService { get; }
        protected IEditorForm<RecipeCreator> EditorForm { get; }
        protected IUniqueValidator<Recipe> RecipeValidator { get; }
        protected ISerializer<Recipe> RecipeSerializer { get; }

        private ICommand openRecipeEditor;
        public ICommand OpenRecipeEditor => openRecipeEditor ?? (openRecipeEditor = new DelegateCommand<ObservableFolder<Recipe>>(EditAndGenerateNewRecipe));

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
                RegenerateRecipes();
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
            newRecipe.Ingredients.Add(new Ingredient("minecraft:", ""));
            newRecipe.IsDirty = false;
            newRecipe.ValidateProperty += (sender, propertyName) => ValidateRecipe(sender, folder.Files, propertyName);
            RecipeValidator.SetDefaultRepository(folder.Files);
            EditorForm.OpenItemEditor(newRecipe);
        }

        protected string ValidateRecipe(Recipe sender, IEnumerable<Recipe> instances, string propertyName) => RecipeValidator.Validate(sender, instances, propertyName).Error;

        protected void CreateRecipe(object sender, ItemEditedEventArgs<RecipeCreator> e)
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
            new FileInfo(tempFilePath).Delete();
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

        protected void RegenerateRecipes()
        {
            McMod mod = SessionContext.SelectedMod;
            CodeGenerationService.RegenerateInitScript(SourceCodeLocator.Recipes(mod.ModInfo.Name, mod.Organization).ClassName, mod, Explorer.Folders.Files[0].Files);
        }

        protected virtual void OnFilesChanged(object sender, FileChangedEventArgs<Recipe> e) => RegenerateRecipes();
    }
}
