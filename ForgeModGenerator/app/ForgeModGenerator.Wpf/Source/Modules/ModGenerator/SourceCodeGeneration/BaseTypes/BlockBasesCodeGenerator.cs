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
            string sourceFolder = Path.Combine(ModPaths.SourceCodeRootFolder(Modname, Organization));
            ScriptFilePaths = new string[] {
                Path.Combine(sourceFolder, SourceCodeLocator.BlockBase.RelativePath),
                Path.Combine(sourceFolder, SourceCodeLocator.OreBase.RelativePath)
            };
        }

        protected override string[] ScriptFilePaths { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit(string scriptPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);
            if (fileName == SourceCodeLocator.BlockBase.ClassName)
            {
                return CreateBlockBase();
            }
            else if (fileName == SourceCodeLocator.OreBase.ClassName)
            {
                return CreateOreBase();
            }
            else
            {
                throw new NotImplementedException($"CodeCompileUnit for {fileName} not found");
            }
        }

        private CodeCompileUnit CreateBlockBase()
        {
            CodeTypeDeclaration clas = NewClassWithBases(SourceCodeLocator.BlockBase.ClassName, "Block", SourceCodeLocator.ModelInterface.ClassName);

            CodeConstructor ctor = NewConstructor(SourceCodeLocator.BlockBase.ClassName, MemberAttributes.Public, new Parameter(typeof(string).FullName, "name"),
                                                                                        new Parameter("Material", "material"));

            ctor.Statements.Add(NewSuper(NewVarReference("material")));
            ctor.Statements.Add(NewMethodInvokeVar("name", "setUnlocalizedName"));
            ctor.Statements.Add(NewMethodInvokeVar("name", "setRegistryName"));
            ctor.Statements.Add(NewMethodInvoke("setCreativeTab", NewFieldReferenceType(SourceCodeLocator.CreativeTab.ClassName, "MODCEATIVETAB")));
            ctor.Statements.Add(NewMethodInvoke(NewFieldReferenceType(SourceCodeLocator.Blocks.ClassName, SourceCodeLocator.Blocks.InitFieldName), "add", NewThis()));
            CodeMethodInvokeExpression setRegistryName = NewMethodInvoke(NewObject("ItemBlock", NewThis()), "setRegistryName", NewMethodInvoke(NewThis(), "getRegistryName"));
            ctor.Statements.Add(NewMethodInvoke(NewFieldReferenceType(SourceCodeLocator.Items.ClassName, SourceCodeLocator.Items.InitFieldName), "add", setRegistryName));
            clas.Members.Add(ctor);
            
            CodeMemberMethod registerModels = NewMethod("registerModels", typeof(void).FullName, MemberAttributes.Public);
            registerModels.CustomAttributes.Add(NewOverrideAnnotation());
            CodeMethodInvokeExpression registerItemRenderer = NewMethodInvoke(NewMethodInvokeType(SourceCodeLocator.Manager.ClassName, "getProxy"), "registerItemRenderer", NewMethodInvokeType("Item", "getItemFromBlock", NewThis()),
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

            return NewCodeUnit(clas, $"{PackageName}.{SourceCodeLocator.Manager.ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.CreativeTab.ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.Blocks.ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.Items.ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.ModelInterface.ImportFullName}",
                                     "net.minecraft.block.Block",
                                     "net.minecraft.block.SoundType",
                                     "net.minecraft.block.material.Material",
                                     "net.minecraft.item.Item",
                                     "net.minecraft.item.ItemBlock");
        }

        private CodeCompileUnit CreateOreBase()
        {
            CodeTypeDeclaration clas = NewClassWithBases(SourceCodeLocator.OreBase.ClassName, SourceCodeLocator.BlockBase.ClassName);
            clas.Members.Add(NewField("Item", "dropItem", MemberAttributes.Family));

            CodeConstructor ctor = NewConstructor(SourceCodeLocator.OreBase.ClassName, MemberAttributes.Public, new Parameter(typeof(string).FullName, "name"),
                                                                                                                new Parameter("Material", "material"));
            ctor.Statements.Add(NewSuper(NewVarReference("name"), NewVarReference("material")));
            clas.Members.Add(ctor);
            
            CodeMemberMethod getItemDropped = NewMethod("getItemDropped", "Item", MemberAttributes.Public, new Parameter("IBlockState", "state"),
                                                                                                           new Parameter("Random", "rand"),
                                                                                                           new Parameter(typeof(int).FullName, "fortune"));
            getItemDropped.CustomAttributes.Add(NewOverrideAnnotation());
            getItemDropped.Statements.Add(NewReturnVar("dropItem"));
            clas.Members.Add(getItemDropped);
            
            CodeMemberMethod quantityDropped = NewMethod("quantityDropped", typeof(int).FullName, MemberAttributes.Public, new Parameter("Random", "rand"));
            quantityDropped.CustomAttributes.Add(NewOverrideAnnotation());
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
