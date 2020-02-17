using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.ViewModels;
using System;
using System.Collections.Generic;
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

        protected override Recipe CreateNewEmptyModel()
        {
            Recipe model = new RecipeCreator();
            model.ValidateProperty += ValidateModel;
            model.IsDirty = false;
            return model;
        }

        protected override void OnItemEdited(object sender, ItemEditedEventArgs<Recipe> e)
        {
            if (e.Result)
            {
                Recipe actualRecipe = e.ActualItem;
                if (e.ActualItem is RecipeCreator creator)
                {
                    actualRecipe = creator.Create();
                }
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

        protected override string ValidateModel(object sender, string propertyName)
        {
            if (sender is Recipe recipe)
            {
                return Context.Validator?.Validate(recipe, ModelsRepository, propertyName).Error;
            }
            else if (sender is RecipeCreator creator)
            {
                return Context.Validator?.Validate(creator.Create(), ModelsRepository, propertyName).Error;
            }
            else
            {
                return "";
            }
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
