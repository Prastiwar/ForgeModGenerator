using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.ViewModels;
using System.ComponentModel;
using System.IO;

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
    }
}
