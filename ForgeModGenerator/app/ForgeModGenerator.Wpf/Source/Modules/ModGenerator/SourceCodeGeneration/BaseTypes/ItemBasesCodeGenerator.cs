using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CodeGeneration.CodeDom;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ItemBasesCodeGenerator : MultiScriptsCodeGenerator
    {
        public ItemBasesCodeGenerator(McMod mcMod) : base(mcMod) =>
            ScriptGenerators = new Dictionary<ClassLocator, GenerateDelegateHandler> {
                { SourceCodeLocator.ItemBase(Modname, Organization), ()=> CreateBaseItemUnit(SourceCodeLocator.ItemBase(Modname, Organization).PackageName, SourceCodeLocator.ItemBase(Modname, Organization).ClassName, "Item", false) },
                { SourceCodeLocator.BowBase(Modname, Organization), ()=> CreateBaseItemUnit(SourceCodeLocator.BowBase(Modname, Organization).PackageName, SourceCodeLocator.BowBase(Modname, Organization).ClassName, "ItemBow", false) },
                { SourceCodeLocator.SwordBase(Modname, Organization), ()=> CreateBaseItemUnit(SourceCodeLocator.SwordBase(Modname, Organization).PackageName, SourceCodeLocator.SwordBase(Modname, Organization).ClassName, "ItemSword", true) },
                { SourceCodeLocator.SpadeBase(Modname, Organization), ()=> CreateBaseItemUnit(SourceCodeLocator.SpadeBase(Modname, Organization).PackageName, SourceCodeLocator.SpadeBase(Modname, Organization).ClassName, "ItemSpade", true) },
                { SourceCodeLocator.PickaxeBase(Modname, Organization),()=> CreateBaseItemUnit(SourceCodeLocator.PickaxeBase(Modname, Organization).PackageName, SourceCodeLocator.PickaxeBase(Modname, Organization).ClassName, "ItemPickaxe", true) },
                { SourceCodeLocator.HoeBase(Modname, Organization), ()=> CreateBaseItemUnit(SourceCodeLocator.HoeBase(Modname, Organization).PackageName, SourceCodeLocator.HoeBase(Modname, Organization).ClassName, "ItemHoe", true) },
                { SourceCodeLocator.AxeBase(Modname, Organization), CreateAxeBase },
                { SourceCodeLocator.FoodBase(Modname, Organization), CreateFoodBase},
                { SourceCodeLocator.ArmorBase(Modname, Organization), CreateArmorBase},
            };

        public override Dictionary<ClassLocator, GenerateDelegateHandler> ScriptGenerators { get; }

        protected CodeCompileUnit CreateAxeBase()
        {
            CodeCompileUnit unit = CreateBaseItemUnit(SourceCodeLocator.AxeBase(Modname, Organization).PackageName, SourceCodeLocator.AxeBase(Modname, Organization).ClassName, "ItemAxe", true);
            CodeConstructor ctor = (CodeConstructor)unit.Namespaces[0].Types[0].Members[0];
            CodeSuperConstructorInvokeExpression super = (CodeSuperConstructorInvokeExpression)((CodeExpressionStatement)ctor.Statements[0]).Expression;
            super.AddParameter(6.0F);
            super.AddParameter(-3.2F);
            return unit;
        }

        protected CodeCompileUnit CreateFoodBase()
        {
            Parameter[] parameters = new Parameter[] {
                        new Parameter(typeof(string).FullName, "name"),
                        new Parameter(typeof(int).FullName, "amount"),
                        new Parameter(typeof(float).FullName, "saturation"),
                        new Parameter(typeof(bool).FullName, "isAnimalFood")
                    };
            return CreateCustomItemUnit(SourceCodeLocator.FoodBase(Modname, Organization).PackageName, SourceCodeLocator.FoodBase(Modname, Organization).ClassName, "ItemFood", parameters);
        }

        protected CodeCompileUnit CreateArmorBase()
        {
            Parameter[] parameters = new Parameter[] {
                        new Parameter(typeof(string).FullName, "name"),
                        new Parameter("ArmorMaterial", "materialIn"),
                        new Parameter(typeof(int).FullName, "renderIndexIn"),
                        new Parameter("EntityEquipmentSlot", "equipmentSlotIn")
                    };
            CodeCompileUnit unit = CreateCustomItemUnit(SourceCodeLocator.ArmorBase(Modname, Organization).PackageName, SourceCodeLocator.ArmorBase(Modname, Organization).ClassName, "ItemArmor", parameters);
            unit.Namespaces[0].Imports.Add(new CodeNamespaceImport("net.minecraft.inventory.EntityEquipmentSlot"));
            return unit;
        }

        private CodeCompileUnit CreateBaseItemUnit(string packageName, string className, string baseType, bool tool = false)
        {
            Parameter[] toolParameters = null;
            if (tool)
            {
                toolParameters = new Parameter[] {
                    new Parameter(typeof(string).FullName, "name"),
                    new Parameter("ToolMaterial", "material")
                };
            }
            else
            {
                toolParameters = new Parameter[] {
                    new Parameter(typeof(string).FullName, "name")
                };
            }
            return CreateCustomItemUnit(packageName, className, baseType, toolParameters);
        }

        private CodeCompileUnit CreateCustomItemUnit(string packageName, string className, string baseType, params Parameter[] ctorParameters)
        {
            CodeTypeDeclaration clas = CreateBaseItemClass(className, baseType, ctorParameters);
            // Package name differs
            return NewCodeUnit(packageName, clas,
                                     $"{SourceRootPackageName}.{SourceCodeLocator.Manager(Modname, Organization).ImportRelativeName}",
                                     $"{SourceRootPackageName}.{SourceCodeLocator.CreativeTab(Modname, Organization).ImportRelativeName}",
                                     $"{SourceRootPackageName}.{SourceCodeLocator.Items(Modname, Organization).ImportRelativeName}",
                                     $"{SourceRootPackageName}.{SourceCodeLocator.ModelInterface(Modname, Organization).ImportRelativeName}",
                                     $"net.minecraft.item.{baseType}");
        }

        private CodeMemberMethod CreateItemRegisterModelsMethod()
        {
            CodeMemberMethod method = NewMethod("registerModels", typeof(void).FullName, MemberAttributes.Public);
            method.CustomAttributes.Add(NewOverrideAnnotation());
            CodeMethodInvokeExpression getProxy = NewMethodInvokeVar(SourceCodeLocator.Manager(Modname, Organization).ClassName, "getProxy");
            method.Statements.Add(NewMethodInvoke(getProxy, "registerItemRenderer", NewThis(), NewPrimitive(0), NewPrimitive("inventory")));
            return method;
        }

        private CodeTypeDeclaration CreateBaseItemClass(string className, string baseClass, params Parameter[] ctorParameters)
        {
            CodeConstructor ctor = NewConstructor(className, MemberAttributes.Public);
            string[] superParameters = null;
            if (ctorParameters != null && ctorParameters.Length > 0)
            {
                List<string> parameterNames = new List<string>(ctorParameters.Length);
                ctor.Parameters.Add(NewParameter(ctorParameters[0].TypeName, ctorParameters[0].Name)); // do not add first param to super arguments
                for (int i = 1; i < ctorParameters.Length; i++)
                {
                    ctor.Parameters.Add(NewParameter(ctorParameters[i].TypeName, ctorParameters[i].Name));
                    parameterNames.Add(ctorParameters[i].Name);
                }
                superParameters = parameterNames.ToArray();
            }
            foreach (CodeExpression item in GetCtorInitializators(superParameters))
            {
                ctor.Statements.Add(item);
            }
            return CreateBaseItemClass(className, baseClass, ctor);
        }

        private CodeTypeDeclaration CreateBaseItemClass(string className, string baseClass, CodeConstructor ctor)
        {
            CodeTypeDeclaration clas = NewClassWithBases(className, baseClass, SourceCodeLocator.ModelInterface(Modname, Organization).ClassName);
            clas.Members.Add(ctor);
            clas.Members.Add(CreateItemRegisterModelsMethod());
            return clas;
        }

        private CodeExpression[] GetCtorInitializators(params string[] superParameters)
        {
            CodeExpression[] ctorArgs = null;
            if (superParameters != null && superParameters.Length > 0)
            {
                List<CodeExpression> ctorArgsList = new List<CodeExpression>(superParameters.Length);
                foreach (string param in superParameters)
                {
                    ctorArgsList.Add(NewVarReference(param));
                }
                ctorArgs = ctorArgsList.ToArray();
            }
            CodeSuperConstructorInvokeExpression super = new CodeSuperConstructorInvokeExpression(ctorArgs);
            CodeMethodInvokeExpression setUnlocalizedName = NewMethodInvoke("setUnlocalizedName", NewVarReference("name"));
            CodeMethodInvokeExpression setRegistryName = NewMethodInvoke("setRegistryName", NewVarReference("name"));
            CodeMethodInvokeExpression setCreativeTab = NewMethodInvoke("setCreativeTab", NewFieldReferenceType(SourceCodeLocator.CreativeTab(Modname, Organization).ClassName, "MODCEATIVETAB"));
            CodeMethodInvokeExpression addToList = NewMethodInvoke(NewFieldReferenceVar(SourceCodeLocator.Items(Modname, Organization).ClassName, SourceCodeLocator.Items(Modname, Organization).InitFieldName), "add", NewThis());
            return new CodeExpression[] { super, setUnlocalizedName, setRegistryName, setCreativeTab, addToList };
        }
    }
}
