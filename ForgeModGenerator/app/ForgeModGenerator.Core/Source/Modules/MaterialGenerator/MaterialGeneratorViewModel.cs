using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.MaterialGenerator.Models;
using ForgeModGenerator.Services;
using ForgeModGenerator.ViewModels;

namespace ForgeModGenerator.MaterialGenerator.ViewModels
{
    /// <summary> MaterialGenerator Business ViewModel </summary>
    public class MaterialGeneratorViewModel : SimpleInitViewModelBase<Material>
    {
        public MaterialGeneratorViewModel(ISessionContextService sessionContext, IEditorFormFactory<Material> editorFormFactory, ICodeGenerationService codeGenerationService)
            : base(sessionContext, editorFormFactory, codeGenerationService) { }

        protected override string ScriptFilePath => SourceCodeLocator.Materials(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization).FullPath;

        protected override Material CreateModelFromLine(string line)
        {
            Material material = new Material();
            System.Globalization.CultureInfo invariancy = System.Globalization.CultureInfo.InvariantCulture;

            material.IsDirty = false;
            return material;
        }
    }
}
