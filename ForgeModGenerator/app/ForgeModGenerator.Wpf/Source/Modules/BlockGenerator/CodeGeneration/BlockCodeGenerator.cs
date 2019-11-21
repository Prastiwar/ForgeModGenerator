using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.BlockGenerator.CodeGeneration
{
    public class BlockCodeGenerator : InitVariablesCodeGenerator<Block>
    {
        public BlockCodeGenerator(McMod mcMod) : this(mcMod, Enumerable.Empty<Block>()) { }
        public BlockCodeGenerator(McMod mcMod, IEnumerable<Block> elements) : base(mcMod, elements) => ScriptLocator = SourceCodeLocator.Blocks(Modname, Organization);

        public override ClassLocator ScriptLocator { get; }

        protected override string GetElementName(Block element) => element.Name;

        protected override CodeMemberField CreateElementField(Block block)
        {
            CodeObjectCreateExpression assignObject = NewObject("BlockBase", NewPrimitive(block.Name.ToLower()), NewPrimitive(block.MaterialType));
            CodeMethodInvokeExpression setSoundType = NewMethodInvoke(assignObject, "setSoundType", NewPrimitive(block.SoundType));
            CodeMethodInvokeExpression setBlockHarvestLevel = NewMethodInvoke(setSoundType, "setBlockHarvestLevel", NewPrimitive(block.HarvestLevelTool.ToString()), NewPrimitive(block.HarvestLevel));
            CodeMethodInvokeExpression setHardness = NewMethodInvoke(setBlockHarvestLevel, "setHardness", NewPrimitive(block.Hardness));
            CodeMethodInvokeExpression setResistance = NewMethodInvoke(setHardness, "setResistance", NewPrimitive(block.Resistance));
            float lightLevel = (float)System.Math.Round(((float)block.LightLevel / 15), 2);
            CodeMethodInvokeExpression setLightLevel = NewMethodInvoke(setResistance, "setLightLevel", NewPrimitive(lightLevel));
            CodeMethodInvokeExpression setCollidable = NewMethodInvoke(setLightLevel, "setCollidable", NewPrimitive(block.ShouldMakeCollision));
            CodeExpression initExpression = setCollidable;
            if (block.Type == BlockType.Ore)
            {
                CodeMethodInvokeExpression setDropItem = NewMethodInvoke(setCollidable, "setDropItem", NewPrimitive(block.DropItem));
                initExpression = setDropItem;
            }
            CreateBlockstateJson(block);
            return NewFieldGlobal("Block", block.Name.ToUpper(), initExpression);
        }

        private void CreateBlockstateJson(Block block)
        {
            string jsonPath = Path.Combine(ModPaths.Blockstates(McMod.ModInfo.Name, McMod.Modid), block.Name.ToLower() + ".json");
            string jsonText = $@"
{{
    ""forge_marker"": 1,
    ""defaults"": {{ ""textures"": {{ ""all"": ""{block.TextureName}"" }} }},
	""variants"": {{
                ""normal"": {{ ""model"": ""cube_all"" }},
                ""inventory"": {{ 
                                ""model"": ""cube_all"",
                                ""textures"": {{ ""all"": ""{block.InventoryTextureName}"" }}
                }}
    }}
}}
";              // TODO: Do not hard-code json
            File.WriteAllText(jsonPath, jsonText);
        }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeCompileUnit unit = CreateDefaultTargetCodeUnit(ScriptLocator.ClassName, "Block");
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.BlockBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.block.Block"));
            unit.Namespaces[0].Imports.Add(NewImport($"java.util.ArrayList"));
            unit.Namespaces[0].Imports.Add(NewImport($"java.util.List"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.block.Block"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.block.SoundType"));
            return unit;
        }
    }
}
