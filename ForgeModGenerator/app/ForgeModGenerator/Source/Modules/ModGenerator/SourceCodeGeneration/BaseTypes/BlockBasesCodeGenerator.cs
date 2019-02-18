using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class BlockBasesCodeGenerator : MultiScriptsCodeGenerator
    {
        public BlockBasesCodeGenerator(Mod mod) : base(mod)
        {
            string folder = Path.Combine(ModPaths.GeneratedSourceCodeFolder(Modname, Organization), "block");
            ScriptFilePaths = new string[] {
                Path.Combine(folder, "BlockBase.java"),
                Path.Combine(folder, "OreBase.java")
            };
        }

        protected override string[] ScriptFilePaths { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit(string scriptPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);
            switch (fileName)
            {
                case "BlockBase":
                    return CreateBlockBase();
                case "OreBase":
                    return CreateOreBase();
                default:
                    throw new NotImplementedException($"CodeCompileUnit for {fileName} not found");
            }
        }

        private CodeCompileUnit CreateBlockBase()
        {
            CodeTypeDeclaration clas = NewClassWithBases("BlockBase", false, "Block", "IHasModel");

            CodeConstructor ctor = NewConstructor("BlockBase", MemberAttributes.Public, new Parameter(typeof(string).FullName, "name"),
                                                                                        new Parameter("Material", "material"));

            ctor.Statements.Add(NewSuper(NewVarReference("material")));
            ctor.Statements.Add(NewMethodInvokeVar("name", "setUnlocalizedName"));
            ctor.Statements.Add(NewMethodInvokeVar("name", "setRegistryName"));
            ctor.Statements.Add(NewMethodInvoke("setCreativeTab", NewFieldReferenceType(Modname + "CreativeTab", "MODCEATIVETAB")));
            ctor.Statements.Add(NewMethodInvoke(NewFieldReferenceType(Modname + "Blocks", "BLOCKS"), "add", NewThis()));
            CodeMethodInvokeExpression setRegistryName = NewMethodInvoke(NewObject("ItemBlock", NewThis()), "setRegistryName", NewMethodInvoke(NewThis(), "getRegistryName"));
            ctor.Statements.Add(NewMethodInvoke(NewFieldReferenceType(Modname + "Items", "ITEMS"), "add", setRegistryName));
            clas.Members.Add(ctor);

            // TODO: Add annotation @Override
            CodeMemberMethod registerModels = NewMethod("registerModels", typeof(void).FullName, MemberAttributes.Public);
            CodeMethodInvokeExpression registerItemRenderer = NewMethodInvoke(NewMethodInvokeType(Modname, "getProxy"), "registerItemRenderer", NewMethodInvokeType("Item", "getItemFromBlock", NewThis()),
                                                                                                                                                NewPrimitive(0),
                                                                                                                                                NewPrimitive("inventory"));
            registerModels.Statements.Add(registerItemRenderer);
            clas.Members.Add(registerModels);

            CodeMemberMethod setSoundType = NewMethod("setSoundType", "BlockBase", MemberAttributes.Public, new Parameter("SoundType", "type"));
            setSoundType.Statements.Add(NewMethodInvoke(new CodeBaseReferenceExpression(), "setSoundType", NewVarReference("type")));
            setSoundType.Statements.Add(NewReturnThis());
            clas.Members.Add(setSoundType);

            CodeMemberMethod setBlockHarvestLevel = NewMethod("setBlockHarvestLevel", "BlockBase", MemberAttributes.Public, new Parameter(typeof(string).FullName, "toolClass"),
                                                                                                                            new Parameter(typeof(int).FullName, "level"));
            CodeMethodInvokeExpression baseSetHarvestLevel = NewMethodInvoke(new CodeBaseReferenceExpression(), "setBlockHarvestLevel", NewVarReference("toolClass"),
                                                                                                                                        NewVarReference("level"));
            setBlockHarvestLevel.Statements.Add(baseSetHarvestLevel);
            setBlockHarvestLevel.Statements.Add(NewReturnThis());
            clas.Members.Add(setBlockHarvestLevel);

            return NewCodeUnit(clas, $"{GeneratedPackageName}.{Modname}",
                                     $"{GeneratedPackageName}.gui.{Modname}CreativeTab",
                                     $"{GeneratedPackageName}.{Modname}Blocks",
                                     $"{GeneratedPackageName}.{Modname}Items",
                                     $"{GeneratedPackageName}.handler.IHasModel",
                                     "net.minecraft.block.Block",
                                     "net.minecraft.block.SoundType",
                                     "net.minecraft.block.material.Material",
                                     "net.minecraft.item.Item",
                                     "net.minecraft.item.ItemBlock");
        }

        private CodeCompileUnit CreateOreBase()
        {
            CodeTypeDeclaration clas = NewClassWithBases("OreBase", false, "BlockBase");
            clas.Members.Add(NewField("Item", "dropItem", MemberAttributes.Family));

            CodeConstructor ctor = NewConstructor("OreBase", MemberAttributes.Public, new Parameter(typeof(string).FullName, "name"),
                                                                                      new Parameter("Material", "material"));
            ctor.Statements.Add(NewSuper(NewVarReference("name"), NewVarReference("material")));
            clas.Members.Add(ctor);

            // TODO: Add annotation @Override
            CodeMemberMethod getItemDropped = NewMethod("getItemDropped", "Item", MemberAttributes.Public, new Parameter("IBlockState", "state"),
                                                                                                           new Parameter("Random", "rand"),
                                                                                                           new Parameter(typeof(int).FullName, "fortune"));
            getItemDropped.Statements.Add(NewReturnVar("dropItem"));
            clas.Members.Add(getItemDropped);

            // TODO: Add annotation @Override
            CodeMemberMethod quantityDropped = NewMethod("quantityDropped", typeof(int).FullName, MemberAttributes.Public, new Parameter("Random", "rand"));
            quantityDropped.Statements.Add(NewVariable(typeof(int).FullName, "max", NewPrimitive(4)));
            quantityDropped.Statements.Add(NewVariable(typeof(int).FullName, "min", NewPrimitive(1)));
            quantityDropped.Statements.Add(NewReturn(new CodeBinaryOperatorExpression(NewMethodInvokeVar("rand", "nextInt", NewVarReference("max")), CodeBinaryOperatorType.Add, NewVarReference("min"))));
            clas.Members.Add(quantityDropped);

            CodeMemberMethod setDropItem = NewMethod("setDropItem", "OreBase", MemberAttributes.Public, new Parameter("Item", "item"));
            setDropItem.Statements.Add(NewAssignVar("dropItem", "item"));
            setDropItem.Statements.Add(NewReturnThis());
            clas.Members.Add(setDropItem);

            return NewCodeUnit(clas, "java.util.Random",
                                     "net.minecraft.block.material.Material",
                                     "net.minecraft.block.state.IBlockState",
                                     "net.minecraft.item.Item");
        }
    }
}
