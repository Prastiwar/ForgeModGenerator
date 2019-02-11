using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CodeGeneration.JavaCodeDom;
using ForgeModGenerator.ModGenerator.Models;
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
            CodeTypeDeclaration clas = GetDefaultClass("BlockBase");
            clas.BaseTypes.Add("Block");
            clas.BaseTypes.Add("IHasModel");

            CodeConstructor ctor = new CodeConstructor() {
                Name = "BlockBase",
                Attributes = MemberAttributes.Public
            };
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression("Material", "material"));
            ctor.Statements.Add(new CodeSuperConstructorInvokeExpression(new CodeVariableReferenceExpression("material")));
            ctor.Statements.Add(new CodeMethodInvokeExpression(null, "setUnlocalizedName", new CodeVariableReferenceExpression("name")));
            ctor.Statements.Add(new CodeMethodInvokeExpression(null, "setRegistryName", new CodeVariableReferenceExpression("name")));
            ctor.Statements.Add(new CodeMethodInvokeExpression(null, "setCreativeTab", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression($"{Modname}CreativeTab"), "MODCEATIVETAB")));
            ctor.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression($"{Modname}Blocks"), "BLOCKS"), "add", new CodeThisReferenceExpression()));
            CodeObjectCreateExpression itemBlock = new CodeObjectCreateExpression("ItemBlock", new CodeThisReferenceExpression());
            CodeMethodInvokeExpression registryName = new CodeMethodInvokeExpression(itemBlock, "setRegistryName", new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "getRegistryName"));
            ctor.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression($"{Modname}Items"), "ITEMS"), "add", registryName));
            clas.Members.Add(ctor);

            // TODO: Add annotation @Override
            CodeMemberMethod registerModels = new CodeMemberMethod() {
                Name = "registerModels",
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference("void")
            };
            CodeMethodInvokeExpression getProxy = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression($"{Modname}"), "getProxy");
            CodeMethodInvokeExpression registerItemRenderer = new CodeMethodInvokeExpression(getProxy, "registerItemRenderer",
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Item"), "getItemFromBlock", new CodeThisReferenceExpression()),
                new CodePrimitiveExpression(0),
                new CodePrimitiveExpression("inventory")
            );
            registerModels.Statements.Add(registerItemRenderer);
            clas.Members.Add(registerModels);

            CodeMemberMethod setSoundType = new CodeMemberMethod() {
                Name = "setSoundType",
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference("BlockBase")
            };
            setSoundType.Parameters.Add(new CodeParameterDeclarationExpression("SoundType", "type"));
            CodeMethodInvokeExpression baseSetSoundType = new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "setSoundType", new CodeVariableReferenceExpression("type"));
            CodeMethodReturnStatement returnThis = new CodeMethodReturnStatement(new CodeThisReferenceExpression());
            setSoundType.Statements.Add(baseSetSoundType);
            setSoundType.Statements.Add(returnThis);
            clas.Members.Add(setSoundType);

            CodeMemberMethod setBlockHarvestLevel = new CodeMemberMethod() {
                Name = "setBlockHarvestLevel",
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference("BlockBase")
            };
            setBlockHarvestLevel.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "toolClass"));
            setBlockHarvestLevel.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "level"));
            CodeMethodInvokeExpression baseSetHarvestLevel = new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "setBlockHarvestLevel",
                new CodeVariableReferenceExpression("toolClass"),
                new CodeVariableReferenceExpression("level")
            );
            setBlockHarvestLevel.Statements.Add(baseSetHarvestLevel);
            setBlockHarvestLevel.Statements.Add(returnThis);
            clas.Members.Add(setBlockHarvestLevel);

            CodeNamespace package = GetDefaultPackage(clas, $"{GeneratedPackageName}.{Modname}",
                                                            $"{GeneratedPackageName}.gui.{Modname}CreativeTab",
                                                            $"{GeneratedPackageName}.{Modname}Blocks",
                                                            $"{GeneratedPackageName}.{Modname}Items",
                                                            $"{GeneratedPackageName}.handler.IHasModel",
                                                            "net.minecraft.block.Block",
                                                            "net.minecraft.block.SoundType",
                                                            "net.minecraft.block.material.Material",
                                                            "net.minecraft.item.Item",
                                                            "net.minecraft.item.ItemBlock");
            return GetDefaultCodeUnit(package);
        }

        private CodeCompileUnit CreateOreBase()
        {
            CodeTypeDeclaration clas = GetDefaultClass("OreBase");
            clas.BaseTypes.Add("BlockBase");
            clas.Members.Add(new CodeMemberField("Item", "dropItem") { Attributes = MemberAttributes.Family });

            CodeConstructor ctor = new CodeConstructor() {
                Name = "OreBase",
                Attributes = MemberAttributes.Public
            };
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression("Material", "material"));
            ctor.Statements.Add(new CodeSuperConstructorInvokeExpression(new CodeVariableReferenceExpression("name"), new CodeVariableReferenceExpression("material")));
            clas.Members.Add(ctor);

            // TODO: Add annotation @Override
            CodeMemberMethod getItemDropped = new CodeMemberMethod() {
                Name = "getItemDropped",
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference("Item")
            };
            getItemDropped.Parameters.Add(new CodeParameterDeclarationExpression("IBlockState", "state"));
            getItemDropped.Parameters.Add(new CodeParameterDeclarationExpression("Random", "rand"));
            getItemDropped.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "fortune"));
            getItemDropped.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("dropItem")));
            clas.Members.Add(getItemDropped);

            // TODO: Add annotation @Override
            CodeMemberMethod quantityDropped = new CodeMemberMethod() {
                Name = "getItemDropped",
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference(typeof(int))
            };
            quantityDropped.Parameters.Add(new CodeParameterDeclarationExpression("Random", "rand"));
            quantityDropped.Statements.Add(new CodeVariableDeclarationStatement(typeof(int), "max", new CodePrimitiveExpression(4)));
            quantityDropped.Statements.Add(new CodeVariableDeclarationStatement(typeof(int), "min", new CodePrimitiveExpression(1)));
            quantityDropped.Statements.Add(new CodeMethodReturnStatement(
                new CodeBinaryOperatorExpression(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("rand"), "nextInt", new CodeVariableReferenceExpression("max")),
                                                 CodeBinaryOperatorType.Add,
                                                 new CodeVariableReferenceExpression("min")
                                                 )));
            clas.Members.Add(quantityDropped);

            CodeMemberMethod setDropItem = new CodeMemberMethod() {
                Name = "setDropItem",
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference("OreBase")
            };
            setDropItem.Parameters.Add(new CodeParameterDeclarationExpression("Item", "item"));
            setDropItem.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("dropItem"), new CodeVariableReferenceExpression("item")));
            setDropItem.Statements.Add(new CodeMethodReturnStatement(new CodeThisReferenceExpression()));
            clas.Members.Add(setDropItem);

            CodeNamespace package = GetDefaultPackage(clas, "java.util.Random",
                                                            "net.minecraft.block.material.Material",
                                                            "net.minecraft.block.state.IBlockState",
                                                            "net.minecraft.item.Item");
            return GetDefaultCodeUnit(package);
        }
    }
}
