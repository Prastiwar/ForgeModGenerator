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
            ScriptLocators = new ClassLocator[] {
                SourceCodeLocator.BlockBase(Modname, Organization),
                SourceCodeLocator.OreBase(Modname, Organization)
            };
        }

        public override ClassLocator[] ScriptLocators { get; }

        public override ClassLocator ScriptLocator => ScriptLocators[0];

        protected override CodeCompileUnit CreateTargetCodeUnit(string scriptPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);
            if (fileName == SourceCodeLocator.BlockBase(Modname, Organization).ClassName)
            {
                return CreateBlockBase();
            }
            else if (fileName == SourceCodeLocator.OreBase(Modname, Organization).ClassName)
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
            CodeTypeDeclaration clas = NewClassWithBases(SourceCodeLocator.BlockBase(Modname, Organization).ClassName, "Block", SourceCodeLocator.ModelInterface(Modname, Organization).ClassName);

            CodeConstructor ctor = NewConstructor(SourceCodeLocator.BlockBase(Modname, Organization).ClassName, MemberAttributes.Public, new Parameter(typeof(string).FullName, "name"),
                                                                                        new Parameter("Material", "material"));

            ctor.Statements.Add(NewSuper(NewVarReference("material")));
            ctor.Statements.Add(NewMethodInvokeVar("name", "setUnlocalizedName"));
            ctor.Statements.Add(NewMethodInvokeVar("name", "setRegistryName"));
            ctor.Statements.Add(NewMethodInvoke("setCreativeTab", NewFieldReferenceType(SourceCodeLocator.CreativeTab(Modname, Organization).ClassName, "MODCEATIVETAB")));
            ctor.Statements.Add(NewMethodInvoke(NewFieldReferenceType(SourceCodeLocator.Blocks(Modname, Organization).ClassName, SourceCodeLocator.Blocks(Modname, Organization).InitFieldName), "add", NewThis()));
            CodeMethodInvokeExpression setRegistryName = NewMethodInvoke(NewObject("ItemBlock", NewThis()), "setRegistryName", NewMethodInvoke(NewThis(), "getRegistryName"));
            ctor.Statements.Add(NewMethodInvoke(NewFieldReferenceType(SourceCodeLocator.Items(Modname, Organization).ClassName, SourceCodeLocator.Items(Modname, Organization).InitFieldName), "add", setRegistryName));
            clas.Members.Add(ctor);
            
            CodeMemberMethod registerModels = NewMethod("registerModels", typeof(void).FullName, MemberAttributes.Public);
            registerModels.CustomAttributes.Add(NewOverrideAnnotation());
            CodeMethodInvokeExpression registerItemRenderer = NewMethodInvoke(NewMethodInvokeType(SourceCodeLocator.Manager(Modname, Organization).ClassName, "getProxy"), "registerItemRenderer", NewMethodInvokeType("Item", "getItemFromBlock", NewThis()),
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

            return NewCodeUnit(clas, $"{PackageName}.{SourceCodeLocator.Manager(Modname, Organization).ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.CreativeTab(Modname, Organization).ImportRelativeName}",
                                     $"{PackageName}.{SourceCodeLocator.Blocks(Modname, Organization).ImportRelativeName}",
                                     $"{PackageName}.{SourceCodeLocator.Items(Modname, Organization).ImportRelativeName}",
                                     $"{PackageName}.{SourceCodeLocator.ModelInterface(Modname, Organization).ImportRelativeName}",
                                     "net.minecraft.block.Block",
                                     "net.minecraft.block.SoundType",
                                     "net.minecraft.block.material.Material",
                                     "net.minecraft.item.Item",
                                     "net.minecraft.item.ItemBlock");
        }

        private CodeCompileUnit CreateOreBase()
        {
            CodeTypeDeclaration clas = NewClassWithBases(SourceCodeLocator.OreBase(Modname, Organization).ClassName, SourceCodeLocator.BlockBase(Modname, Organization).ClassName);
            clas.Members.Add(NewField("Item", "dropItem", MemberAttributes.Family));

            CodeConstructor ctor = NewConstructor(SourceCodeLocator.OreBase(Modname, Organization).ClassName, MemberAttributes.Public, new Parameter(typeof(string).FullName, "name"),
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
