using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.RecipeGenerator.ViewModels
{
    public class RecipeGeneratorViewModel : JsonFileInitViewModelBase<Recipe>
    {
        public RecipeGeneratorViewModel(GeneratorContext<Recipe> context, ISynchronizeInvoke synchronizeInvoke, IJsonUpdaterFactory<Recipe> jsonUpdaterFactory)
            : base(context, synchronizeInvoke, jsonUpdaterFactory) => FileSearchPatterns = "*.json";

        protected override string InitFilePath
            => SourceCodeLocator.Recipes(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization).FullPath;

        public override string DirectoryRootPath => SessionContext.SelectedMod != null
            ? ModPaths.RecipesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid)
            : null;

        protected override string GetModelFullPath(Recipe model)
            => Path.Combine(ModPaths.RecipesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid), model.Name + ".json");

        protected override void CreateNewModel(ObservableCollection<Recipe> collection)
        {
            Recipe newModel = new RecipeCreator();
            EditorForm.OpenItemEditor(newModel);
        }

        protected override void EditItem(Recipe item)
        {
            RecipeCreator recipeCreator = new RecipeCreator(item);
            EditorForm.OpenItemEditor(recipeCreator);
        }
        protected override void RegenerateCode(Recipe item)
        {
            if (item is RecipeCreator creator)
            {
                item = creator.Create();
            }
            base.RegenerateCode(item);
        }

        protected override void RegenerateCodeBatched(IEnumerable<Recipe> models)
        {
            if (models.Any(recipe => recipe is RecipeCreator))
            {
                throw new ArgumentException($"Argument cannot contain type: {typeof(RecipeCreator)}", nameof(models));
            }
            base.RegenerateCodeBatched(models);
        }
    }
}
