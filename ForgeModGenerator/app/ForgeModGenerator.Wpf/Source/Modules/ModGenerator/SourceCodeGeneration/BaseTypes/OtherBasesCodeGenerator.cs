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
        public OtherBasesCodeGenerator(McMod mcMod) : base(mcMod)
            => ScriptLocators = new ClassLocator[] {
                SourceCodeLocator.SoundEventBase(Modname, Organization),
                SourceCodeLocator.FoodEffectBase(Modname, Organization)
            };

        public override ClassLocator[] ScriptLocators { get; }

        public override ClassLocator ScriptLocator => ScriptLocators[0];

        protected override CodeCompileUnit CreateTargetCodeUnit(string scriptPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);
            if (fileName == SourceCodeLocator.SoundEventBase(Modname, Organization).ClassName)
            {
                return CreateSoundEventBase();
            }
            else if (fileName == SourceCodeLocator.FoodEffectBase(Modname, Organization).ClassName)
            {
                return CreateFoodEffectBase();
            }
            else
            {
                throw new NotImplementedException($"CodeCompileUnit for {fileName} not found");
            }
        }

        private CodeCompileUnit CreateSoundEventBase()
        {
            CodeTypeDeclaration clas = NewClassWithBases(SourceCodeLocator.SoundEventBase(Modname, Organization).ClassName, "SoundEvent");
            CodeConstructor ctor = NewConstructor(SourceCodeLocator.SoundEventBase(Modname, Organization).ClassName, MemberAttributes.Public, new Parameter(typeof(string).FullName, "name"));
            ctor.Statements.Add(new CodeSuperConstructorInvokeExpression(NewObject("ResourceLocation", NewFieldReferenceType(SourceCodeLocator.Hook(Modname, Organization).ClassName, "MODID"), NewVarReference("name"))));
            ctor.Statements.Add(NewMethodInvoke("setRegistryName", NewVarReference("name")));
            ctor.Statements.Add(NewMethodInvoke(NewFieldReferenceType(SourceCodeLocator.SoundEvents(Modname, Organization).ClassName, SourceCodeLocator.SoundEvents(Modname, Organization).InitFieldName), "add", NewThis()));
            clas.Members.Add(ctor);
            CodeNamespace package = NewPackage(SourceCodeLocator.SoundEventBase(Modname, Organization).PackageName, clas,
                                                     $"{SourceRootPackageName}.{SourceCodeLocator.Hook(Modname, Organization).ImportRelativeName}",
                                                     $"{SourceRootPackageName}.{SourceCodeLocator.SoundEvents(Modname, Organization).ImportRelativeName}",
                                                     "net.minecraft.util.ResourceLocation",
                                                     "net.minecraft.util.SoundEvent");
            return NewCodeUnit(package);
        }

        private CodeCompileUnit CreateFoodEffectBase()
        {
            CodeTypeDeclaration clas = NewClassWithBases(SourceCodeLocator.FoodEffectBase(Modname, Organization).ClassName, "FoodBase");
            clas.Members.Add(NewField("PotionEffect", "effect", MemberAttributes.Family));
            CodeConstructor ctor = NewConstructor(SourceCodeLocator.FoodEffectBase(Modname, Organization).ClassName, MemberAttributes.Public, new Parameter(typeof(string).FullName, "name"),
                                                                                             new Parameter(typeof(int).FullName, "amount"),
                                                                                             new Parameter(typeof(float).FullName, "saturation"),
                                                                                             new Parameter(typeof(bool).FullName, "isAnimalFood"),
                                                                                             new Parameter("PotionEffect", "effect"));
            ctor.Statements.Add(NewSuper(NewVarReference("name"), NewVarReference("amount"), NewVarReference("saturation"), NewVarReference("isAnimalFood")));
            ctor.Statements.Add(NewMethodInvoke("setAlwaysEdible"));
            ctor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(NewThis(), "effect"), NewVarReference("effect")));
            clas.Members.Add(ctor);

            CodeMemberMethod onFoodEaten = NewMethod("onFoodEaten", typeof(void).FullName, MemberAttributes.Family, new Parameter("ItemStack", "stack"),
                                                                                                                    new Parameter("World", "worldIn"),
                                                                                                                    new Parameter("EntityPlayer", "player"));
            onFoodEaten.CustomAttributes.Add(NewOverrideAnnotation());
            CodeObjectCreateExpression potionEffect = NewObject("PotionEffect", NewMethodInvokeVar("effect", "getPotion"),
                                                                                NewMethodInvokeVar("effect", "getDuration"),
                                                                                NewMethodInvokeVar("effect", "getAmplifier"),
                                                                                NewMethodInvokeVar("effect", "getIsAmbient"),
                                                                                NewMethodInvokeVar("effect", "doesShowParticles"));
            CodeMethodInvokeExpression addPotionEffect = NewMethodInvokeVar("player", "addPotionEffect", potionEffect);
            onFoodEaten.Statements.Add(new CodeConditionStatement(new CodeSnippetExpression("!worldIn.isRemote"), new CodeExpressionStatement(addPotionEffect)));
            clas.Members.Add(onFoodEaten);

            CodeMemberMethod hasEffect = NewMethod("hasEffect", typeof(bool).FullName, MemberAttributes.Public, new Parameter("ItemStack", "stack"));
            hasEffect.CustomAttributes.Add(NewSideAnnotation(ModSide.Client));
            hasEffect.Statements.Add(NewReturnPrimitive(true));
            clas.Members.Add(hasEffect);

            return NewCodeUnit(SourceCodeLocator.FoodEffectBase(Modname, Organization).PackageName, clas,
                                     "net.minecraft.entity.player.EntityPlayer",
                                     "net.minecraft.item.ItemStack",
                                     "net.minecraft.potion.PotionEffect",
                                     "net.minecraft.world.World",
                                     "net.minecraftforge.fml.relauncher.Side",
                                     "net.minecraftforge.fml.relauncher.SideOnly");
        }
    }
}
