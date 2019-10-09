using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Services;
using ForgeModGenerator.Utility;
using ForgeModGenerator.ViewModels;

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

        protected override Block CreateModelFromLine(string line)
        {
            int startIndex = line.IndexOf("new ") + 4;
            int endIndex = line.IndexOf("(", startIndex);
            string type = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf("\"", endIndex) + 1;
            endIndex = line.IndexOf("\"", startIndex);
            string name = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf(" ", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string material = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string soundType = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf("\"", endIndex) + 1;
            endIndex = line.IndexOf("\"", startIndex);
            string blockHarvestTool = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf(" ", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string blockHarvestLevel = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string hardness = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string resistance = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string lightLevel = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf("(", endIndex) + 1;
            endIndex = line.IndexOf(")", startIndex);
            string collidable = line.Substring(startIndex, endIndex - startIndex);

            string dropItem = "";
            if (type == "OreBase")
            {
                startIndex = line.IndexOf("(", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                dropItem = line.Substring(startIndex, endIndex - startIndex);
            }
            Block block = new Block();
            System.Globalization.CultureInfo invariancy = System.Globalization.CultureInfo.InvariantCulture;
            block.Type = type == "OreBase" ? BlockType.Ore : BlockType.Hard;
            block.Name = name;
            block.MaterialType = (MaterialType)System.Enum.Parse(typeof(MaterialType), material.Remove(0, "Material.".Length), true);
            block.SoundType = soundType;
            block.HarvestLevelTool = blockHarvestTool;
            block.HarvestLevel = int.Parse(blockHarvestLevel);
            block.Hardness = float.Parse(hardness.RemoveEnding(), invariancy);
            block.Resistance = float.Parse(resistance.RemoveEnding(), invariancy);
            block.LightLevel = (int)(float.Parse(lightLevel.RemoveEnding(), invariancy) * 15);
            block.ShouldMakeCollision = bool.Parse(collidable);
            block.DropItem = dropItem;
            return block;
        }
    }
}
