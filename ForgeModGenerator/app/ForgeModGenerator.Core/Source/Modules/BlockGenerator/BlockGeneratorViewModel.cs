using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Services;
using ForgeModGenerator.Utility;
using ForgeModGenerator.Validation;
using ForgeModGenerator.ViewModels;
using System.IO;

namespace ForgeModGenerator.BlockGenerator.ViewModels
{
    /// <summary> BlockGenerator Business ViewModel </summary>
    public class BlockGeneratorViewModel : SimpleInitViewModelBase<Block>
    {
        public BlockGeneratorViewModel(ISessionContextService sessionContext,
                                       IEditorFormFactory<Block> editorFormFactory,
                                       IUniqueValidator<Block> validator,
                                       ICodeGenerationService codeGenerationService)
            : base(sessionContext, editorFormFactory, validator, codeGenerationService) { }

        protected override string ScriptFilePath => SourceCodeLocator.Blocks(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization).FullPath;

        protected override void RemoveModel(Block model)
        {
            base.RemoveModel(model);
            string blockstatePath =
                Path.Combine(ModPaths.Blockstates(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Modid), model.Name + ".json");
            if (File.Exists(blockstatePath))
            {
                File.Delete(blockstatePath);
            }
        }

        protected override Block ParseModelFromJavaField(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return null;
            }

            Block block = new Block();
            System.Globalization.CultureInfo invariancy = System.Globalization.CultureInfo.InvariantCulture;

            int startIndex = line.IndexOf("new ") + 4;
            int endIndex = line.IndexOf("(", startIndex);
            string type = line.Substring(startIndex, endIndex - startIndex);
            block.Type = type == "OreBase" ? BlockType.Ore : BlockType.Hard;

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(",", startIndex);
            string name = line.Substring(startIndex, endIndex - startIndex);
            block.Name = name.Replace("\"", "");

            startIndex = line.IndexOf(" ", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string material = line.Substring(startIndex, endIndex - startIndex);
            block.MaterialType = material.Replace("\"", "");

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string soundType = line.Substring(startIndex, endIndex - startIndex);
            block.SoundType = soundType.Replace("\"", "");

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(",", startIndex);
            string blockHarvestTool = line.Substring(startIndex, endIndex - startIndex);
            block.HarvestLevelTool = blockHarvestTool.Replace("\"", "");

            startIndex = line.IndexOf(" ", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string blockHarvestLevel = line.Substring(startIndex, endIndex - startIndex);
            block.HarvestLevel = int.Parse(blockHarvestLevel);

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string hardness = line.Substring(startIndex, endIndex - startIndex);
            block.Hardness = float.Parse(hardness.RemoveEnding(), invariancy);

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string resistance = line.Substring(startIndex, endIndex - startIndex);
            block.Resistance = float.Parse(resistance.RemoveEnding(), invariancy);

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string lightLevel = line.Substring(startIndex, endIndex - startIndex);
            float lightLevelFloat = float.Parse(lightLevel.RemoveEnding(), invariancy);
            lightLevelFloat = lightLevelFloat > 0 ? lightLevelFloat * 15 : 0;
            block.LightLevel = (int)(lightLevelFloat);

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string collidable = line.Substring(startIndex, endIndex - startIndex);
            block.ShouldMakeCollision = bool.Parse(collidable);

            if (type == "OreBase")
            {
                startIndex = line.IndexOf("(", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string dropItem = line.Substring(startIndex, endIndex - startIndex);
                block.DropItem = dropItem;
            }

            string blockstatesPath = ModPaths.Blockstates(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            string blockJsonPath = Path.Combine(blockstatesPath, block.Name.ToLower() + ".json");
            if (File.Exists(blockJsonPath))
            {
                string jsonContent = File.ReadAllText(blockJsonPath);
                string keyword = "\"all\":";
                startIndex = jsonContent.IndexOf(keyword, 1) + keyword.Length;
                startIndex = jsonContent.IndexOf("\"", startIndex) + 1;
                endIndex = jsonContent.IndexOf("\"", startIndex);
                string textureName = jsonContent.Substring(startIndex, endIndex - startIndex);
                block.TextureName = textureName;

                startIndex = jsonContent.IndexOf(keyword, endIndex) + keyword.Length ;
                startIndex = jsonContent.IndexOf("\"", startIndex) + 1;
                endIndex = jsonContent.IndexOf("\"", startIndex);
                string inventoryTextureName = jsonContent.Substring(startIndex, endIndex - startIndex);
                block.InventoryTextureName = inventoryTextureName;
            }

            block.IsDirty = false;
            block.ValidateProperty += ValidateModel;
            return block;
        }
    }
}
