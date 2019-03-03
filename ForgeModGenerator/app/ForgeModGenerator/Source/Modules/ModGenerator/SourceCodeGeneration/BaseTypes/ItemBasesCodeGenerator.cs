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
        public ItemBasesCodeGenerator(Mod mod) : base(mod)
        {
            string sourceFolder = Path.Combine(ModPaths.SourceCodeRootFolder(Modname, Organization));
            ScriptFilePaths = new string[] {
                Path.Combine(sourceFolder, SourceCodeLocator.ItemBase.RelativePath),
                Path.Combine(sourceFolder, SourceCodeLocator.BowBase.RelativePath),
                Path.Combine(sourceFolder, SourceCodeLocator.FoodBase.RelativePath),
                Path.Combine(sourceFolder, SourceCodeLocator.ArmorBase.RelativePath),
                Path.Combine(sourceFolder, SourceCodeLocator.SwordBase.RelativePath),
                Path.Combine(sourceFolder, SourceCodeLocator.SpadeBase.RelativePath),
                Path.Combine(sourceFolder, SourceCodeLocator.PickaxeBase.RelativePath),
                Path.Combine(sourceFolder, SourceCodeLocator.HoeBase.RelativePath),
                Path.Combine(sourceFolder, SourceCodeLocator.AxeBase.RelativePath),
            };
        }

        protected override string[] ScriptFilePaths { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit(string scriptPath)
        {
            Parameter[] parameters = null;
            CodeCompileUnit unit = null;
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);
            if (fileName == SourceCodeLocator.ItemBase.ClassName)
            {
                return CreateBaseItemUnit(fileName, "Item", false);
            }
            else if (fileName == SourceCodeLocator.BowBase.ClassName)
            {
                return CreateBaseItemUnit(fileName, "ItemBow", false);
            }
            else if (fileName == SourceCodeLocator.SwordBase.ClassName)
            {
                return CreateBaseItemUnit(fileName, "ItemSword", true);
            }
            else if (fileName == SourceCodeLocator.SpadeBase.ClassName)
            {
                return CreateBaseItemUnit(fileName, "ItemSpade", true);
            }
            else if (fileName == SourceCodeLocator.PickaxeBase.ClassName)
            {
                return CreateBaseItemUnit(fileName, "ItemPickaxe", true);
            }
            else if (fileName == SourceCodeLocator.HoeBase.ClassName)
            {
                return CreateBaseItemUnit(fileName, "ItemHoe", true);
            }
            else if (fileName == SourceCodeLocator.AxeBase.ClassName)
            {
                unit = CreateBaseItemUnit(fileName, "ItemAxe", true);
                CodeConstructor ctor = (CodeConstructor)unit.Namespaces[0].Types[0].Members[0];
                CodeSuperConstructorInvokeExpression super = (CodeSuperConstructorInvokeExpression)((CodeExpressionStatement)ctor.Statements[0]).Expression;
                super.AddParameter(6.0F);
                super.AddParameter(-3.2F);
                return unit;
            }

            else if (fileName == SourceCodeLocator.FoodBase.ClassName)
            {
                parameters = new Parameter[] {
                        new Parameter(typeof(string).FullName, "name"),
                        new Parameter(typeof(int).FullName, "amount"),
                        new Parameter(typeof(float).FullName, "saturation"),
                        new Parameter(typeof(bool).FullName, "isAnimalFood")
                    };
                return CreateCustomItemUnit(fileName, "ItemFood", parameters);
            }
            else if (fileName == SourceCodeLocator.ArmorBase.ClassName)
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
            return NewCodeUnit(clas, $"{PackageName}.{SourceCodeLocator.Manager.ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.CreativeTab.ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.Items.ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.ModelInterface.ImportFullName}",
                                     $"net.minecraft.item.{baseType}");
        }

        private CodeMemberMethod CreateItemRegisterModelsMethod()
        {
            CodeMemberMethod method = NewMethod("registerModels", typeof(void).FullName, MemberAttributes.Public);
            method.CustomAttributes.Add(NewOverrideAnnotation());
            CodeMethodInvokeExpression getProxy = NewMethodInvokeVar(SourceCodeLocator.Manager.ClassName, "getProxy");
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
            CodeTypeDeclaration clas = NewClassWithBases(className, baseClass, SourceCodeLocator.ModelInterface.ClassName);
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
            CodeMethodInvokeExpression setCreativeTab = NewMethodInvoke("setCreativeTab", NewFieldReferenceType(SourceCodeLocator.CreativeTab.ClassName, "MODCEATIVETAB"));
            CodeMethodInvokeExpression addToList = NewMethodInvoke(NewFieldReferenceVar(SourceCodeLocator.Items.ClassName, SourceCodeLocator.Items.InitFieldName), "add", NewThis());
            return new CodeExpression[] { super, setUnlocalizedName, setRegistryName, setCreativeTab, addToList };
        }
    }
}
