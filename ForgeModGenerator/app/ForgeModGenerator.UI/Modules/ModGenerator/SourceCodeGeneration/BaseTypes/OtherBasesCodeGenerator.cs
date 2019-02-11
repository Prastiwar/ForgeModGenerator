using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CodeGeneration.JavaCodeDom;
using ForgeModGenerator.ModGenerator.Models;
using System;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class OtherBasesCodeGenerator : MultiScriptsCodeGenerator
    {
        public OtherBasesCodeGenerator(Mod mod) : base(mod)
            => ScriptFilePaths = new string[] {
                Path.Combine(ModPaths.GeneratedSourceCodeFolder(Modname, Organization), "sound", "SoundEventBase.java"),
                Path.Combine(ModPaths.GeneratedSourceCodeFolder(Modname, Organization), "item", "food", "FoodEffectBase.java")
            };

        protected override string[] ScriptFilePaths { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit(string scriptPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);
            switch (fileName)
            {
                case "SoundEventBase":
                    return CreateSoundEventBase();
                case "FoodEffectBase":
                    return CreateFoodEffectBase();
                default:
                    throw new NotImplementedException($"CodeCompileUnit for {fileName} not found");
            }
        }

        private CodeCompileUnit CreateSoundEventBase()
        {
            CodeTypeDeclaration clas = GetDefaultClass("SoundEventBase");
            clas.BaseTypes.Add("SoundEvent");
            CodeConstructor ctor = new CodeConstructor() {
                Name = "SoundEventBase",
                Attributes = MemberAttributes.Public
            };
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
            ctor.Statements.Add(new CodeSuperConstructorInvokeExpression(new CodeObjectCreateExpression("ResourceLocation",
                                        new CodeFieldReferenceExpression(new CodeTypeReferenceExpression($"{Modname}Hook"), "MODID"), new CodeVariableReferenceExpression("name")))
                                        );
            ctor.Statements.Add(new CodeMethodInvokeExpression(null, "setRegistryName", new CodeVariableReferenceExpression("name")));
            ctor.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression($"{Modname}Sounds"), "SOUNDS"), "add", new CodeThisReferenceExpression()));
            clas.Members.Add(ctor);
            CodeNamespace package = GetDefaultPackage(clas, $"{GeneratedPackageName}.{Modname}Hook",
                                                            $"{GeneratedPackageName}.{Modname}Sounds",
                                                            "net.minecraft.util.ResourceLocation",
                                                            "net.minecraft.util.SoundEvent");
            return GetDefaultCodeUnit(package);
        }

        private CodeCompileUnit CreateFoodEffectBase()
        {
            CodeTypeDeclaration clas = GetDefaultClass("FoodEffectBase");
            clas.BaseTypes.Add("FoodBase");
            clas.Members.Add(new CodeMemberField("PotionEffect", "effect"));
            CodeConstructor ctor = new CodeConstructor() {
                Name = "SoundEventBase",
                Attributes = MemberAttributes.Public
            };
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "amount"));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression("float", "saturation"));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression("boolean", "isAnimalFood"));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression("PotionEffect", "effect"));
            ctor.Statements.Add(new CodeSuperConstructorInvokeExpression(new CodeVariableReferenceExpression("name"),
                                                                            new CodeVariableReferenceExpression("amount"),
                                                                            new CodeVariableReferenceExpression("saturation"),
                                                                            new CodeVariableReferenceExpression("isAnimalFood")));
            ctor.Statements.Add(new CodeMethodInvokeExpression(null, "setAlwaysEdible"));
            ctor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "effect"), new CodeVariableReferenceExpression("effect")));
            clas.Members.Add(ctor);

            // TODO: Add annotation @Override
            CodeMemberMethod onFoodEaten = new CodeMemberMethod() {
                Name = "onFoodEaten",
                Attributes = MemberAttributes.Family,
                ReturnType = new CodeTypeReference("void")
            };
            onFoodEaten.Parameters.Add(new CodeParameterDeclarationExpression("ItemStack", "stack"));
            onFoodEaten.Parameters.Add(new CodeParameterDeclarationExpression("World", "worldIn"));
            onFoodEaten.Parameters.Add(new CodeParameterDeclarationExpression("EntityPlayer", "player"));
            CodeObjectCreateExpression potionEffect = new CodeObjectCreateExpression("PotionEffect",
                new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("effect"), "getPotion"),
                new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("effect"), "getDuration"),
                new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("effect"), "getAmplifier"),
                new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("effect"), "getIsAmbient"),
                new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("effect"), "doesShowParticles")
            );
            CodeMethodInvokeExpression addPotionEffect = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("player"), "addPotionEffect", potionEffect);
            CodeConditionStatement condition = new CodeConditionStatement(new CodeSnippetExpression("!worldIn.isRemote"), new CodeExpressionStatement(addPotionEffect));
            onFoodEaten.Statements.Add(condition);
            clas.Members.Add(onFoodEaten);

            // TODO: Add annotation @SideOnly(Side.CLIENT)
            CodeMemberMethod hasEffect = new CodeMemberMethod() {
                Name = "hasEffect",
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference("boolean")
            };
            hasEffect.Parameters.Add(new CodeParameterDeclarationExpression("ItemStack", "stack"));
            hasEffect.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(true)));
            clas.Members.Add(hasEffect);

            CodeNamespace package = GetDefaultPackage(clas, "net.minecraft.entity.player.EntityPlayer",
                                                            "net.minecraft.item.ItemStack",
                                                            "net.minecraft.potion.PotionEffect",
                                                            "net.minecraft.world.World",
                                                            "net.minecraftforge.fml.relauncher.Side",
                                                            "net.minecraftforge.fml.relauncher.SideOnly");
            return GetDefaultCodeUnit(package);
        }
    }
}
