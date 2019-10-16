using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CodeGeneration.CodeDom;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class OtherBasesCodeGenerator : MultiScriptsCodeGenerator
    {

        public OtherBasesCodeGenerator(McMod mcMod) : base(mcMod)
            => ScriptGenerators = new Dictionary<ClassLocator, GenerateDelegateHandler>{
                { SourceCodeLocator.SoundEventBase(Modname, Organization), CreateSoundEventBase},
                { SourceCodeLocator.FoodEffectBase(Modname, Organization), CreateFoodEffectBase},
                { SourceCodeLocator.MaterialBase(Modname, Organization), CreateMaterialBase}
            };

        public override Dictionary<ClassLocator, GenerateDelegateHandler> ScriptGenerators { get; }

        private CodeCompileUnit CreateMaterialBase()
        {
            string baseClassName = SourceCodeLocator.MaterialBase(Modname, Organization).ClassName;
            CodeTypeDeclaration clas = NewClassWithBases(baseClassName, "Material");
            CodeConstructor ctor = NewConstructor(baseClassName, MemberAttributes.Public, new Parameter("MapColor", "color"));
            ctor.Statements.Add(NewSuper(NewVarReference("color")));
            ctor.Statements.Add(NewMethodInvoke(NewFieldReferenceType(SourceCodeLocator.Materials(Modname, Organization).ClassName, SourceCodeLocator.Materials(Modname, Organization).InitFieldName), "add", NewThis()));
            clas.Members.Add(ctor);

            clas.Members.Add(NewField(typeof(bool).FullName, "liquid", MemberAttributes.Family));
            clas.Members.Add(NewField(typeof(bool).FullName, "solid", MemberAttributes.Family));
            clas.Members.Add(NewField(typeof(bool).FullName, "shouldBlockLight", MemberAttributes.Family));
            clas.Members.Add(NewField(typeof(bool).FullName, "shouldBlockMovement", MemberAttributes.Family));
            clas.Members.Add(NewField(typeof(bool).FullName, "canBurn", MemberAttributes.Family));
            clas.Members.Add(NewField(typeof(bool).FullName, "translucent", MemberAttributes.Family));
            clas.Members.Add(NewField(typeof(bool).FullName, "requiresNoTool", MemberAttributes.Family));
            clas.Members.Add(NewField(typeof(bool).FullName, "replaceable", MemberAttributes.Family));
            clas.Members.Add(NewField(typeof(bool).FullName, "adventureModeExempt", MemberAttributes.Family));
            clas.Members.Add(NewField("EnumPushReaction", "mobilityFlag", MemberAttributes.Family));

            JavaCodeMemberMethod method = NewMethod("isLiquid", typeof(bool).FullName, MemberAttributes.Public);
            method.CustomAttributes.Add(NewOverrideAnnotation());
            method.Statements.Add(NewReturnVar("liquid"));
            clas.Members.Add(method);

            method = NewMethod("isSolid", typeof(bool).FullName, MemberAttributes.Public);
            method.CustomAttributes.Add(NewOverrideAnnotation());
            method.Statements.Add(NewReturnVar("solid"));
            clas.Members.Add(method);

            method = NewMethod("blocksLight", typeof(bool).FullName, MemberAttributes.Public);
            method.CustomAttributes.Add(NewOverrideAnnotation());
            method.Statements.Add(NewReturnVar("shouldBlockLight"));
            clas.Members.Add(method);

            method = NewMethod("blocksMovement", typeof(bool).FullName, MemberAttributes.Public);
            method.CustomAttributes.Add(NewOverrideAnnotation());
            method.Statements.Add(NewReturnVar("shouldBlockMovement"));
            clas.Members.Add(method);

            method = NewMethod("getCanBurn", typeof(bool).FullName, MemberAttributes.Public);
            method.CustomAttributes.Add(NewOverrideAnnotation());
            method.Statements.Add(NewReturnVar("canBurn"));
            clas.Members.Add(method);

            method = NewMethod("isToolNotRequired", typeof(bool).FullName, MemberAttributes.Public);
            method.CustomAttributes.Add(NewOverrideAnnotation());
            method.Statements.Add(NewReturnVar("requiresNoTool"));
            clas.Members.Add(method);

            method = NewMethod("isReplaceable", typeof(bool).FullName, MemberAttributes.Public);
            method.CustomAttributes.Add(NewOverrideAnnotation());
            method.Statements.Add(NewReturnVar("replaceable"));
            clas.Members.Add(method);

            method = NewMethod("getMobilityFlag", "EnumPushReaction", MemberAttributes.Public);
            method.CustomAttributes.Add(NewOverrideAnnotation());
            method.Statements.Add(NewReturnVar("mobilityFlag"));
            clas.Members.Add(method);

            method = NewMethod("isOpaque", typeof(bool).FullName, MemberAttributes.Public);
            method.CustomAttributes.Add(NewOverrideAnnotation());
            method.Statements.Add(NewSnippetStatement("		return this.translucent ? false : this.blocksMovement();"));
            clas.Members.Add(method);

            method = NewMethod("setSolid", baseClassName, MemberAttributes.Public, new Parameter(typeof(bool).FullName, "value"));
            method.Statements.Add(NewAssignVar("solid", "value"));
            method.Statements.Add(NewReturnThis());
            clas.Members.Add(method);

            method = NewMethod("setLiquid", baseClassName, MemberAttributes.Public, new Parameter(typeof(bool).FullName, "value"));
            method.Statements.Add(NewAssignVar("liquid", "value"));
            method.Statements.Add(NewReturnThis());
            clas.Members.Add(method);

            method = NewMethod("setBlockLight", baseClassName, MemberAttributes.Public, new Parameter(typeof(bool).FullName, "value"));
            method.Statements.Add(NewAssignVar("shouldBlockLight", "value"));
            method.Statements.Add(NewReturnThis());
            clas.Members.Add(method);

            method = NewMethod("setBlockMovement", baseClassName, MemberAttributes.Public, new Parameter(typeof(bool).FullName, "value"));
            method.Statements.Add(NewAssignVar("shouldBlockMovement", "value"));
            method.Statements.Add(NewReturnThis());
            clas.Members.Add(method);

            method = NewMethod("setTranslucent", baseClassName, MemberAttributes.Public, new Parameter(typeof(bool).FullName, "value"));
            method.Statements.Add(NewAssignVar("translucent", "value"));
            method.Statements.Add(NewReturnThis());
            clas.Members.Add(method);

            method = NewMethod("setRequiresNoTool", baseClassName, MemberAttributes.Public, new Parameter(typeof(bool).FullName, "value"));
            method.Statements.Add(NewAssignVar("requiresNoTool", "value"));
            method.Statements.Add(NewReturnThis());
            clas.Members.Add(method);

            method = NewMethod("setBurning", baseClassName, MemberAttributes.Public, new Parameter(typeof(bool).FullName, "value"));
            method.Statements.Add(NewAssignVar("canBurn", "value"));
            method.Statements.Add(NewReturnThis());
            clas.Members.Add(method);

            method = NewMethod("setReplaceable", baseClassName, MemberAttributes.Public, new Parameter(typeof(bool).FullName, "value"));
            method.Statements.Add(NewAssignVar("replaceable", "value"));
            method.Statements.Add(NewReturnThis());
            clas.Members.Add(method);

            method = NewMethod("setAdventureModeExempt", baseClassName, MemberAttributes.Public, new Parameter(typeof(bool).FullName, "value"));
            method.Statements.Add(NewAssignVar("adventureModeExempt", "value"));
            method.Statements.Add(NewReturnThis());
            clas.Members.Add(method);

            method = NewMethod("setMobilityFlag", baseClassName, MemberAttributes.Public, new Parameter("EnumPushReaction", "value"));
            method.Statements.Add(NewAssignVar("mobilityFlag", "value"));
            method.Statements.Add(NewReturnThis());
            clas.Members.Add(method);

            method = NewMethod("setReplaceable", "Material", MemberAttributes.Family);
            method.Statements.Add(NewAssignPrimitive("replaceable", true));
            method.Statements.Add(NewReturnThis());
            method.CustomAttributes.Add(NewOverrideAnnotation());
            clas.Members.Add(method);

            method = NewMethod("setNoPushMobility", "Material", MemberAttributes.Family);
            method.Statements.Add(NewAssignVar("mobilityFlag", "EnumPushReaction.DESTROY"));
            method.Statements.Add(NewReturnThis());
            method.CustomAttributes.Add(NewOverrideAnnotation());
            clas.Members.Add(method);

            method = NewMethod("setImmovableMobility", "Material", MemberAttributes.Family);
            method.Statements.Add(NewAssignVar("mobilityFlag", "EnumPushReaction.BLOCK"));
            method.Statements.Add(NewReturnThis());
            method.CustomAttributes.Add(NewOverrideAnnotation());
            clas.Members.Add(method);

            method = NewMethod("setAdventureModeExempt", "Material", MemberAttributes.Family);
            method.Statements.Add(NewAssignPrimitive("adventureModeExempt", true));
            method.Statements.Add(NewReturnThis());
            method.CustomAttributes.Add(NewOverrideAnnotation());
            clas.Members.Add(method);

            method = NewMethod("setRequiresTool", "Material", MemberAttributes.Family);
            method.Statements.Add(NewAssignPrimitive("requiresNoTool", false));
            method.Statements.Add(NewReturnThis());
            method.CustomAttributes.Add(NewOverrideAnnotation());
            clas.Members.Add(method);

            method = NewMethod("setBurning", "Material", MemberAttributes.Family);
            method.Statements.Add(NewAssignPrimitive("canBurn", true));
            method.Statements.Add(NewReturnThis());
            method.CustomAttributes.Add(NewOverrideAnnotation());
            clas.Members.Add(method);

            CodeNamespace package = NewPackage(SourceCodeLocator.MaterialBase(Modname, Organization).PackageName, clas,
                                                                           "net.minecraft.block.material.EnumPushReaction",
                                                                           "net.minecraft.block.material.MapColor",
                                                                           "net.minecraft.block.material.Material");
            return NewCodeUnit(package);
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
