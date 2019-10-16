using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Services;
using ForgeModGenerator.Utility;
using ForgeModGenerator.ViewModels;
using System.IO;

namespace ForgeModGenerator.BlockGenerator.ViewModels
{
    /// <summary> BlockGenerator Business ViewModel </summary>
    public class BlockGeneratorViewModel : SimpleInitViewModelBase<Block>
    {
        public BlockGeneratorViewModel(ISessionContextService sessionContext,
                                        IEditorFormFactory<Block> editorFormFactory,
                                        ICodeGenerationService codeGenerationService)
            : base(sessionContext, editorFormFactory, codeGenerationService) { }

        protected override string ScriptFilePath => SourceCodeLocator.Blocks(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization).FullPath;

        protected override Block ParseModelFromJavaField(string line)
        {
            Block block = new Block();
            System.Globalization.CultureInfo invariancy = System.Globalization.CultureInfo.InvariantCulture;

            int startIndex = line.IndexOf("new ") + 4;
            int endIndex = line.IndexOf("(", startIndex);
            string type = line.Substring(startIndex, endIndex - startIndex);
            block.Type = type == "OreBase" ? BlockType.Ore : BlockType.Hard;

            startIndex = line.IndexOf("\"", endIndex) + 1;
            endIndex = line.IndexOf("\"", startIndex);
            string name = line.Substring(startIndex, endIndex - startIndex);
            block.Name = name;

            startIndex = line.IndexOf(" ", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string material = line.Substring(startIndex, endIndex - startIndex);
            block.MaterialType = (MaterialType)System.Enum.Parse(typeof(MaterialType), material.Remove(0, "Material.".Length), true);

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string soundType = line.Substring(startIndex, endIndex - startIndex);
            block.SoundType = soundType;

            startIndex = line.IndexOf("\"", endIndex) + 1;
            endIndex = line.IndexOf("\"", startIndex);
            string blockHarvestTool = line.Substring(startIndex, endIndex - startIndex);
            block.HarvestLevelTool = blockHarvestTool;

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
            block.LightLevel = (int)(float.Parse(lightLevel.RemoveEnding(), invariancy) * 15);

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

            string blockstatesPath = ModPaths.Blockstates(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization);
            string blockJsonPath = Path.Combine(blockstatesPath, name.ToLower() + ".json");
            if (File.Exists(blockJsonPath))
            {
                string jsonContent = File.ReadAllText(blockJsonPath);
                startIndex = jsonContent.IndexOf(SessionContext.SelectedMod.ModInfo.Modid, 1);
                endIndex = jsonContent.IndexOf("\"", startIndex);
                string textureName = jsonContent.Substring(startIndex, endIndex - startIndex);
                block.TextureName = textureName;

                startIndex = jsonContent.IndexOf(SessionContext.SelectedMod.ModInfo.Modid, endIndex);
                endIndex = jsonContent.IndexOf("\"", startIndex);
                string inventoryTextureName = jsonContent.Substring(startIndex, endIndex - startIndex);
                block.InventoryTextureName = inventoryTextureName;
            }

            block.IsDirty = false;
            return block;
        }
    }
}
