using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CodeGeneration.CodeDom;
using ForgeModGenerator.Models;
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
            CodeTypeDeclaration clas = NewClassWithBases("SoundEventBase", false, "SoundEvent");
            CodeConstructor ctor = NewConstructor("SoundEventBase", MemberAttributes.Public, new Parameter(typeof(string).FullName, "name"));
            ctor.Parameters.Add(NewParameter(typeof(string).FullName, "name"));
            ctor.Statements.Add(new CodeSuperConstructorInvokeExpression(NewObject("ResourceLocation", NewFieldReferenceType(Modname + "Hook", "MODID"), NewVarReference("name"))));
            ctor.Statements.Add(NewMethodInvoke("setRegistryName", NewVarReference("name")));
            ctor.Statements.Add(NewMethodInvoke("add", NewFieldReferenceType(Modname + "Sounds", "SOUNDS"), NewThis()));
            clas.Members.Add(ctor);
            CodeNamespace package = NewPackage(clas, $"{GeneratedPackageName}.{Modname}Hook",
                                                            $"{GeneratedPackageName}.{Modname}Sounds",
                                                            "net.minecraft.util.ResourceLocation",
                                                            "net.minecraft.util.SoundEvent");
            return NewCodeUnit(package);
        }

        private CodeCompileUnit CreateFoodEffectBase()
        {
            CodeTypeDeclaration clas = NewClassWithBases("FoodEffectBase", false, "FoodBase");
            clas.Members.Add(NewField("PotionEffect", "effect", MemberAttributes.Family));
            CodeConstructor ctor = NewConstructor("FoodEffectBase", MemberAttributes.Public, new Parameter(typeof(string).FullName, "name"),
                                                                                             new Parameter(typeof(int).FullName, "amount"),
                                                                                             new Parameter(typeof(float).FullName, "saturation"),
                                                                                             new Parameter(typeof(bool).FullName, "isAnimalFood"),
                                                                                             new Parameter("PotionEffect", "effect"));
            ctor.Statements.Add(NewSuper(NewVarReference("name"), NewVarReference("amount"), NewVarReference("saturation"), NewVarReference("isAnimalFood")));
            ctor.Statements.Add(NewMethodInvoke("setAlwaysEdible"));
            ctor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(NewThis(), "effect"), NewVarReference("effect")));
            clas.Members.Add(ctor);

            // TODO: Add annotation @Override
            CodeMemberMethod onFoodEaten = NewMethod("onFoodEaten", typeof(void).FullName, MemberAttributes.Family, new Parameter("ItemStack", "stack"),
                                                                                                                    new Parameter("World", "worldIn"),
                                                                                                                    new Parameter("EntityPlayer", "player"));
            CodeObjectCreateExpression potionEffect = NewObject("PotionEffect", NewMethodInvokeVar("effect", "getPotion"),
                                                                                NewMethodInvokeVar("effect", "getDuration"),
                                                                                NewMethodInvokeVar("effect", "getAmplifier"),
                                                                                NewMethodInvokeVar("effect", "getIsAmbient"),
                                                                                NewMethodInvokeVar("effect", "doesShowParticles"));
            CodeMethodInvokeExpression addPotionEffect = NewMethodInvokeVar("player", "addPotionEffect", potionEffect);
            onFoodEaten.Statements.Add(new CodeConditionStatement(new CodeSnippetExpression("!worldIn.isRemote"), new CodeExpressionStatement(addPotionEffect)));
            clas.Members.Add(onFoodEaten);

            // TODO: Add annotation @SideOnly(Side.CLIENT)
            CodeMemberMethod hasEffect = NewMethod("hasEffect", typeof(bool).FullName, MemberAttributes.Public, new Parameter("ItemStack", "stack"));
            hasEffect.Statements.Add(NewReturnPrimitive(true));
            clas.Members.Add(hasEffect);

            return NewCodeUnit(clas, "net.minecraft.entity.player.EntityPlayer",
                                     "net.minecraft.item.ItemStack",
                                     "net.minecraft.potion.PotionEffect",
                                     "net.minecraft.world.World",
                                     "net.minecraftforge.fml.relauncher.Side",
                                     "net.minecraftforge.fml.relauncher.SideOnly");
        }
    }
}
