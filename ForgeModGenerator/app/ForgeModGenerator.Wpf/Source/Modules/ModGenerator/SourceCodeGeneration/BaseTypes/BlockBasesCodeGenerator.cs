using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class BlockBasesCodeGenerator : MultiScriptsCodeGenerator
    {
        public BlockBasesCodeGenerator(McMod mcMod) : base(mcMod) => ScriptLocators = new ClassLocator[] {
                SourceCodeLocator.BlockBase(Modname, Organization),
                SourceCodeLocator.OreBase(Modname, Organization)
            };

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
            ctor.Statements.Add(NewMethodInvoke("setUnlocalizedName", NewVarReference("name")));
            ctor.Statements.Add(NewMethodInvoke("setRegistryName", NewVarReference("name")));
            ctor.Statements.Add(NewMethodInvoke("setCreativeTab", NewFieldReferenceType(SourceCodeLocator.CreativeTab(Modname, Organization).ClassName, "MODCEATIVETAB")));
            ctor.Statements.Add(NewMethodInvoke(NewFieldReferenceType(SourceCodeLocator.Blocks(Modname, Organization).ClassName, SourceCodeLocator.Blocks(Modname, Organization).InitFieldName), "add", NewThis()));
            CodeMethodInvokeExpression setRegistryName = NewMethodInvoke(NewObject("ItemBlock", NewThis()), "setRegistryName", NewMethodInvoke(NewThis(), "getRegistryName"));
            ctor.Statements.Add(NewMethodInvoke(NewFieldReferenceType(SourceCodeLocator.Items(Modname, Organization).ClassName, SourceCodeLocator.Items(Modname, Organization).InitFieldName), "add", setRegistryName));
            clas.Members.Add(ctor);

            clas.Members.Add(NewField(typeof(bool).FullName, "shouldMakeCollision", MemberAttributes.FamilyOrAssembly));

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
            CodeMethodInvokeExpression baseSetHarvestLevel = NewMethodInvoke(new CodeBaseReferenceExpression(), "setHarvestLevel", NewVarReference("toolClass"),
                                                                                                                                   NewVarReference("level"));
            setBlockHarvestLevel.Statements.Add(baseSetHarvestLevel);
            setBlockHarvestLevel.Statements.Add(NewReturnThis());
            clas.Members.Add(setBlockHarvestLevel);

            CodeMemberMethod setCollidable = NewMethod("setCollidable", "BlockBase", MemberAttributes.Public, new Parameter(typeof(bool).FullName, "isCollidable"));
            CodeAssignStatement assignCollidable = NewAssignVar("shouldMakeCollision", "isCollidable");
            setCollidable.Statements.Add(assignCollidable);
            setCollidable.Statements.Add(NewReturnThis());
            clas.Members.Add(setCollidable);

            CodeMemberMethod addCollisionBoxToList = NewMethod("addCollisionBoxToList", typeof(void).FullName, MemberAttributes.Public,
                new Parameter("IBlockState", "state"),
                new Parameter("World", "worldIn"),
                new Parameter("BlockPos", "pos"),
                new Parameter("AxisAlignedBB", "entityBox"),
                new Parameter("List<AxisAlignedBB>", "collidingBoxes"),
                new Parameter("@Nullable Entity", "entityIn"),
                new Parameter(typeof(bool).FullName, "isActualState")
                );
            addCollisionBoxToList.CustomAttributes.Add(NewOverrideAnnotation());
            CodeMethodInvokeExpression baseAddCollisionBoxToList = NewMethodInvoke(new CodeBaseReferenceExpression(), "addCollisionBoxToList",
                NewVarReference("state"), NewVarReference("worldIn"), NewVarReference("pos"), NewVarReference("entityBox"), NewVarReference("collidingBoxes"), NewVarReference("entityIn"), NewVarReference("isActualState")
                );
            CodeConditionStatement ifStatement = new CodeConditionStatement(NewVarReference("shouldMakeCollision"), new CodeExpressionStatement(baseAddCollisionBoxToList));
            addCollisionBoxToList.Statements.Add(ifStatement);
            clas.Members.Add(addCollisionBoxToList);

            return NewCodeUnit(SourceCodeLocator.BlockBase(Modname, Organization).PackageName, clas,
                                     $"{SourceRootPackageName}.{SourceCodeLocator.Manager(Modname, Organization).ImportRelativeName}",
                                     $"{SourceRootPackageName}.{SourceCodeLocator.CreativeTab(Modname, Organization).ImportRelativeName}",
                                     $"{SourceRootPackageName}.{SourceCodeLocator.Blocks(Modname, Organization).ImportRelativeName}",
                                     $"{SourceRootPackageName}.{SourceCodeLocator.Items(Modname, Organization).ImportRelativeName}",
                                     $"{SourceRootPackageName}.{SourceCodeLocator.ModelInterface(Modname, Organization).ImportRelativeName}",
                                     "java.util.List",
                                     "javax.annotation.Nullable",
                                     "net.minecraft.block.Block",
                                     "net.minecraft.block.SoundType",
                                     "net.minecraft.block.material.Material",
                                     "net.minecraft.block.state.IBlockState",
                                     "net.minecraft.entity.Entity",
                                     "net.minecraft.item.Item",
                                     "net.minecraft.item.ItemBlock",
                                     "net.minecraft.util.math.AxisAlignedBB",
                                     "net.minecraft.util.math.BlockPos",
                                     "net.minecraft.world.World"
                                     );

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

            return NewCodeUnit(SourceCodeLocator.OreBase(Modname, Organization).PackageName, clas,
                                     "java.util.Random",
                                     "net.minecraft.block.material.Material",
                                     "net.minecraft.block.state.IBlockState",
                                     "net.minecraft.item.Item");
        }
    }
}
