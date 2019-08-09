using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System;
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

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeCompileUnit unit = CreateDefaultTargetCodeUnit(ScriptLocator.ClassName, "Block");
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.BlockBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.block.Block"));
            foreach (Block block in Elements)
            {
                CodeExpression initExpression = null;
                CodeObjectCreateExpression assignObject = NewObject("BlockBase", NewPrimitive(block.Name.ToLower()), NewPrimitive(block.MaterialType));
                CodeMethodInvokeExpression setSoundType = NewMethodInvoke(assignObject, "setSoundType", NewPrimitive(block.SoundType));
                CodeMethodInvokeExpression setBlockHarvestLevel = NewMethodInvoke(setSoundType, "setBlockHarvestLevel", NewPrimitive(block.HarvestLevel.Key), NewPrimitive(block.HarvestLevel.Value));
                CodeMethodInvokeExpression setHardness = NewMethodInvoke(setBlockHarvestLevel, "setHardness", NewPrimitive(block.Hardness));
                CodeMethodInvokeExpression setResistance = NewMethodInvoke(setHardness, "setResistance", NewPrimitive(block.Resistance));
                CodeMethodInvokeExpression setLightLevel = NewMethodInvoke(setResistance, "setLightLevel", NewPrimitive(block.LightLevel / 15));
                switch (block.Type)
                {
                    case BlockType.Hard:
                        initExpression = setLightLevel;
                        break;
                    case BlockType.Ore:
                        CodeMethodInvokeExpression setDropItem = NewMethodInvoke(setResistance, "setDropItem", NewPrimitive(block.DropItem)); // FIX: DropItem is not primitive
                        initExpression = setDropItem;
                        break;
                    default:
                        throw new NotImplementedException($"{block.Type} was not implemented");
                }
                unit.Namespaces[0].Types[0].Members.Add(NewFieldGlobal("Block", block.Name.ToUpper(), initExpression));
                string jsonPath = Path.Combine(ModPaths.Blockstates(McMod.ModInfo.Name, McMod.Modid), block.Name.ToLower() + ".json");
                string jsonText = $@"
{{
    ""forge_marker"": 1,
    ""defaults"": {{ ""textures"": {{ ""all"": ""{McMod.ModInfo.Name}:{block.InventoryTexturePath.Remove(0, block.TexturePath.IndexOf("textures\\") + 8).Replace("\\", "/")}"" }} }},
	""variants"": {{{{
                ""normal"": {{ ""model"": ""cube_all"" }},
                ""inventory"": {{ 
                                ""model"": ""cube_all"",
                                ""textures"": {{ ""all"": ""{McMod.ModInfo.Name}:{block.InventoryTexturePath.Remove(0, block.InventoryTexturePath.IndexOf("textures\\") + 8).Replace("\\", "/")}"" }}
                }}
    }}
}}
";              // TODO: Do not hard-code json
                // TODO: Generate json
                // TODO: Implement block.ShouldMakeCollision
            }
            return unit;
        }
    }
}
