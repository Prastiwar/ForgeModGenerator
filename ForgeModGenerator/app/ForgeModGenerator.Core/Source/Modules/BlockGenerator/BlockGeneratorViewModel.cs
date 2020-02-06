using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Utility;
using ForgeModGenerator.ViewModels;
using System;
using System.IO;

namespace ForgeModGenerator.BlockGenerator.ViewModels
{
    /// <summary> BlockGenerator Business ViewModel </summary>
    public class BlockGeneratorViewModel : JavaInitViewModelBase<Block>
    {
        public BlockGeneratorViewModel(GeneratorContext<Block> context) : base(context) { }

        protected override string InitFilePath => SourceCodeLocator.Blocks(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization).FullPath;

        protected override void RemoveItem(Block item)
        {
            base.RemoveItem(item);
            string blockstatesFolder = ModPaths.Blockstates(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            string blockstatePath = Path.Combine(blockstatesFolder, item.Name + ".json");
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
            string[] args = Array.Empty<string>();

            int startIndex = line.IndexOf("new ") + "new ".Length;
            int endIndex = line.IndexOf("(", startIndex);
            string type = line.Substring(startIndex, endIndex - startIndex);
            block.Type = type == "OreBase" ? BlockType.Ore : BlockType.Hard;

            string soundType = GetParenthesesContentFor(line, "setSoundType");
            block.SoundType = soundType.Replace("\"", "");

            string hardness = GetParenthesesContentFor(line, "setHardness");
            block.Hardness = float.Parse(hardness.RemoveEnding(), invariancy);

            string resistance = GetParenthesesContentFor(line, "setResistance");
            block.Resistance = float.Parse(resistance.RemoveEnding(), invariancy);

            string collidable = GetParenthesesContentFor(line, "setCollidable");
            block.ShouldMakeCollision = bool.Parse(collidable);

            args = SplitParenthesesContentFor(line, type);
            block.Name = args[0].Replace("\"", "");
            block.MaterialType = args[1];

            args = SplitParenthesesContentFor(line, "setBlockHarvestLevel", endIndex);
            block.HarvestLevelTool = ReflectionHelper.ParseEnum<HarvestToolType>(args[0].Replace("\"", ""));
            block.HarvestLevel = int.Parse(args[1]);

            string lightLevel = GetParenthesesContentFor(line, "setLightLevel");
            float lightLevelFloat = float.Parse(lightLevel.RemoveEnding(), invariancy);
            lightLevelFloat = lightLevelFloat > 0 ? lightLevelFloat * 15 : 0;
            block.LightLevel = (int)(lightLevelFloat);

            if (type == "OreBase")
            {
                string dropItem = GetParenthesesContentFor(line, "setDropItem");
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

                startIndex = jsonContent.IndexOf(keyword, endIndex) + keyword.Length;
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
