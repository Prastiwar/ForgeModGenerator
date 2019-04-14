using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CodeGeneration.CodeDom;
using ForgeModGenerator.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ItemBasesCodeGenerator : MultiScriptsCodeGenerator
    {
        public ItemBasesCodeGenerator(Mod mod) : base(mod) =>
            ScriptLocators = new ClassLocator[] {
                SourceCodeLocator.ItemBase(Modname, Organization),
                SourceCodeLocator.BowBase(Modname, Organization) ,
                SourceCodeLocator.FoodBase(Modname, Organization),
                SourceCodeLocator.ArmorBase(Modname, Organization),
                SourceCodeLocator.SwordBase(Modname, Organization),
                SourceCodeLocator.SpadeBase(Modname, Organization),
                SourceCodeLocator.PickaxeBase(Modname, Organization),
                SourceCodeLocator.HoeBase(Modname, Organization),
                SourceCodeLocator.AxeBase(Modname, Organization),
            };

        public override ClassLocator[] ScriptLocators { get; }

        public override ClassLocator ScriptLocator => ScriptLocators[0];

        protected override CodeCompileUnit CreateTargetCodeUnit(string scriptPath)
        {
            Parameter[] parameters = null;
            CodeCompileUnit unit = null;
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);
            if (fileName == SourceCodeLocator.ItemBase(Modname, Organization).ClassName)
            {
                return CreateBaseItemUnit(fileName, "Item", false);
            }
            else if (fileName == SourceCodeLocator.BowBase(Modname, Organization).ClassName)
            {
                return CreateBaseItemUnit(fileName, "ItemBow", false);
            }
            else if (fileName == SourceCodeLocator.SwordBase(Modname, Organization).ClassName)
            {
                return CreateBaseItemUnit(fileName, "ItemSword", true);
            }
            else if (fileName == SourceCodeLocator.SpadeBase(Modname, Organization).ClassName)
            {
                return CreateBaseItemUnit(fileName, "ItemSpade", true);
            }
            else if (fileName == SourceCodeLocator.PickaxeBase(Modname, Organization).ClassName)
            {
                return CreateBaseItemUnit(fileName, "ItemPickaxe", true);
            }
            else if (fileName == SourceCodeLocator.HoeBase(Modname, Organization).ClassName)
            {
                return CreateBaseItemUnit(fileName, "ItemHoe", true);
            }
            else if (fileName == SourceCodeLocator.AxeBase(Modname, Organization).ClassName)
            {
                unit = CreateBaseItemUnit(fileName, "ItemAxe", true);
                CodeConstructor ctor = (CodeConstructor)unit.Namespaces[0].Types[0].Members[0];
                CodeSuperConstructorInvokeExpression super = (CodeSuperConstructorInvokeExpression)((CodeExpressionStatement)ctor.Statements[0]).Expression;
                super.AddParameter(6.0F);
                super.AddParameter(-3.2F);
                return unit;
            }

            else if (fileName == SourceCodeLocator.FoodBase(Modname, Organization).ClassName)
            {
                parameters = new Parameter[] {
                        new Parameter(typeof(string).FullName, "name"),
                        new Parameter(typeof(int).FullName, "amount"),
                        new Parameter(typeof(float).FullName, "saturation"),
                        new Parameter(typeof(bool).FullName, "isAnimalFood")
                    };
                return CreateCustomItemUnit(fileName, "ItemFood", parameters);
            }
            else if (fileName == SourceCodeLocator.ArmorBase(Modname, Organization).ClassName)
            {
                parameters = new Parameter[] {
                        new Parameter(typeof(string).FullName, "name"),
                        new Parameter("ArmorMaterial", "materialIn"),
                        new Parameter(typeof(int).FullName, "renderIndexIn"),
                        new Parameter("EntityEquipmentSlot", "equipmentSlotIn")
                    };
                unit = CreateCustomItemUnit(fileName, "ItemArmor", parameters);
                unit.Namespaces[0].Imports.Add(new CodeNamespaceImport("net.minecraft.inventory.EntityEquipmentSlot"));
                return unit;
            }
            else
            {
                throw new NotImplementedException($"CodeCompileUnit for {fileName} not found");
            }
        }

        private CodeCompileUnit CreateBaseItemUnit(string className, string baseType, bool tool = false)
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
            return CreateCustomItemUnit(className, baseType, toolParameters);
        }

        private CodeCompileUnit CreateCustomItemUnit(string className, string baseType, params Parameter[] ctorParameters)
        {
            CodeTypeDeclaration clas = CreateBaseItemClass(className, baseType, ctorParameters);
            return NewCodeUnit(clas, $"{PackageName}.{SourceCodeLocator.Manager(Modname, Organization).ImportRelativeName}",
                                     $"{PackageName}.{SourceCodeLocator.CreativeTab(Modname, Organization).ImportRelativeName}",
                                     $"{PackageName}.{SourceCodeLocator.Items(Modname, Organization).ImportRelativeName}",
                                     $"{PackageName}.{SourceCodeLocator.ModelInterface(Modname, Organization).ImportRelativeName}",
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
