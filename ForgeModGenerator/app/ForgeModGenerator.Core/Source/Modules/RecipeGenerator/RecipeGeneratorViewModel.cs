using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator.RecipeGenerator.ViewModels
{
    public class RecipeGeneratorViewModel : JsonFileInitViewModelBase<Recipe>
    {
        public RecipeGeneratorViewModel(GeneratorContext<Recipe> context,
                                        ISynchronizeInvoke synchronizeInvoke,
                                        IJsonUpdaterFactory<Recipe> jsonUpdaterFactory,
                                        IChoiceFormFactory<Recipe> chooseRecipeFormFactory)
            : base(context, synchronizeInvoke, jsonUpdaterFactory)
        {
            FileSearchPatterns = "*.json";
            ChooseRecipeForm = chooseRecipeFormFactory.Create();
        }

        protected override string InitFilePath
            => SourceCodeLocator.Recipes(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization).FullPath;

        public override string DirectoryRootPath => SessionContext.SelectedMod != null
            ? ModPaths.RecipesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid)
            : null;

        public IChoiceForm<Recipe> ChooseRecipeForm { get; }

        protected override string GetModelFullPath(Recipe model)
            => Path.Combine(ModPaths.RecipesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid), model.Name + ".json");

        protected override Recipe CreateNewEmptyModel()
        {
            Recipe model = new Recipe();
            model.ValidateProperty += ValidateModel;
            model.IsDirty = false;
            return model;
        }

        protected override async void CreateNewModel(ObservableCollection<Recipe> collection)
        {
            Recipe newModel = (Recipe)await ChooseRecipeForm.OpenChoicesAsync().ConfigureAwait(true);
            if (newModel != null)
            {
                newModel.ValidateProperty += ValidateModel;
                newModel.IsDirty = false;
                EditorForm.OpenItemEditor(newModel);
            }
        }

        protected override void OnItemEdited(object sender, ItemEditedEventArgs<Recipe> e)
        {
            if (e.Result)
            {
                Recipe actualRecipe = e.ActualItem;
                bool wasSynchronizing = Synchronizer.IsEnabled;
                Synchronizer.SetEnableSynchronization(false);
                if (!ModelsRepository.Contains(actualRecipe))
                {
                    ModelsRepository.Add(actualRecipe);
                }
                else
                {
                    Context.FileSystem.DeleteFile(GetModelFullPath(e.CachedItem), true);
                }
                RegenerateCode(actualRecipe);
                Synchronizer.SetEnableSynchronization(wasSynchronizing);
            }
            else
            {
                e.ActualItem.CopyValues(e.CachedItem);
            }
        }
    }
}
