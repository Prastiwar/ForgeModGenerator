using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CodeGeneration.JavaCodeDom;
using ForgeModGenerator.ModGenerator.Models;
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
            string folder = ModPaths.GeneratedItemFolder(Modname, Organization);
            ScriptFilePaths = new string[] {
                Path.Combine(folder, "ItemBase.java"),
                Path.Combine(folder, "bow", "BowBase.java"),
                Path.Combine(folder, "food", "FoodBase.java"),
                Path.Combine(folder, "armor", "ArmorBase.java"),
                Path.Combine(folder, "tool", "SwordBase.java"),
                Path.Combine(folder, "tool", "SpadeBase.java"),
                Path.Combine(folder, "tool", "PickaxeBase.java"),
                Path.Combine(folder, "tool", "HoeBase.java"),
                Path.Combine(folder, "tool", "AxeBase.java"),
            };
        }

        protected override string[] ScriptFilePaths { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit(string scriptPath)
        {
            KeyValuePair<string, string>[] parameters = null;
            CodeCompileUnit unit = null;
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);
            switch (fileName)
            {
                case "ItemBase":
                    return CreateBaseItemUnit(fileName, "Item", false);
                case "BowBase":
                    return CreateBaseItemUnit(fileName, "ItemBow", false);
                case "SwordBase":
                    return CreateBaseItemUnit(fileName, "ItemSword", true);
                case "SpadeBase":
                    return CreateBaseItemUnit(fileName, "ItemSpade", true);
                case "PickaxeBase":
                    return CreateBaseItemUnit(fileName, "ItemPickaxe", true);
                case "HoeBase":
                    return CreateBaseItemUnit(fileName, "ItemHoe", true);

                case "AxeBase":
                    unit = CreateBaseItemUnit(fileName, "ItemAxe", true);
                    CodeConstructor ctor = (CodeConstructor)unit.Namespaces[0].Types[0].Members[0];
                    CodeSuperConstructorInvokeExpression super = (CodeSuperConstructorInvokeExpression)((CodeExpressionStatement)ctor.Statements[0]).Expression;
                    super.AddParameter(6.0F);
                    super.AddParameter(-3.2F);
                    return unit;

                case "FoodBase":
                    parameters = new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("String", "name"),
                        new KeyValuePair<string, string>("int", "amount"),
                        new KeyValuePair<string, string>("float", "saturation"),
                        new KeyValuePair<string, string>("boolean", "isAnimalFood")
                    };
                    return CreateCustomItemUnit(fileName, "ItemFood", parameters);

                case "ArmorBase":
                    parameters = new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("String", "name"),
                        new KeyValuePair<string, string>("ArmorMaterial", "materialIn"),
                        new KeyValuePair<string, string>("int", "renderIndexIn"),
                        new KeyValuePair<string, string>("EntityEquipmentSlot", "equipmentSlotIn")
                    };
                    unit = CreateCustomItemUnit(fileName, "ItemArmor", parameters);
                    unit.Namespaces[0].Imports.Add(new CodeNamespaceImport("net.minecraft.inventory.EntityEquipmentSlot"));
                    return unit;
                default:
                    throw new NotImplementedException($"CodeCompileUnit for {fileName} not found");
            }
        }

        private CodeCompileUnit CreateBaseItemUnit(string className, string baseType, bool tool = false)
        {
            KeyValuePair<string, string>[] toolParameters = null;
            if (tool)
            {
                toolParameters = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("String", "mame"),
                    new KeyValuePair<string, string>("ToolMaterial", "material")
                };
            }
            else
            {
                toolParameters = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("String", "mame")
                };
            }
            return CreateCustomItemUnit(className, baseType, toolParameters);
        }

        private CodeCompileUnit CreateCustomItemUnit(string className, string baseType, params KeyValuePair<string, string>[] ctorParameters)
        {
            CodeTypeDeclaration clas = CreateBaseItemClass(className, baseType, ctorParameters);
            CodeNamespace package = GetDefaultPackage(clas, $"{GeneratedPackageName}.{Modname}",
                                                            $"{GeneratedPackageName}.gui.{Modname}CreativeTab",
                                                            $"{GeneratedPackageName}.{Modname}Items",
                                                            $"{GeneratedPackageName}.interface.IHasModel",
                                                            $"net.minecraft.item.{baseType}");
            return GetDefaultCodeUnit(package);
        }

        private CodeMemberMethod CreateItemRegisterModelsMethod()
        {
            // TODO: Add annotation @Override
            CodeMemberMethod method = new CodeMemberMethod() {
                Name = "registerModels",
                Attributes = MemberAttributes.Public
            };
            CodeMethodInvokeExpression getProxy = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression($"{Modname}"), "getProxy");
            CodeMethodInvokeExpression registerItemRenderer =
                new CodeMethodInvokeExpression(getProxy, "registerItemRenderer", new CodeVariableReferenceExpression("this"), new CodePrimitiveExpression(0), new CodePrimitiveExpression("inventory"));
            return method;
        }

        private CodeTypeDeclaration CreateBaseItemClass(string className, string baseClass, params KeyValuePair<string, string>[] ctorParameters)
        {
            CodeConstructor ctor = new CodeConstructor() {
                Name = className,
                Attributes = MemberAttributes.Public
            };
            string[] superParameters = null;
            if (ctorParameters != null && ctorParameters.Length > 0)
            {
                List<string> parameterNames = new List<string>(ctorParameters.Length);
                ctor.Parameters.Add(new CodeParameterDeclarationExpression(ctorParameters[0].Key, ctorParameters[0].Value)); // do not add first param to super arguments
                for (int i = 1; i < ctorParameters.Length; i++)
                {
                    ctor.Parameters.Add(new CodeParameterDeclarationExpression(ctorParameters[i].Key, ctorParameters[i].Value));
                    parameterNames.Add(ctorParameters[i].Value);
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
            CodeTypeDeclaration clas = GetDefaultClass(className, false);
            clas.BaseTypes.Add(new CodeTypeReference(baseClass));
            clas.BaseTypes.Add(new CodeTypeReference("IHasModel"));
            clas.Members.Add(ctor);
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
                    ctorArgsList.Add(new CodeVariableReferenceExpression(param));
                }
                ctorArgs = ctorArgsList.ToArray();
            }
            CodeSuperConstructorInvokeExpression super = new CodeSuperConstructorInvokeExpression(ctorArgs);
            CodeMethodInvokeExpression setUnlocalizedName = new CodeMethodInvokeExpression(null, "setUnlocalizedName", new CodeVariableReferenceExpression("name"));
            CodeMethodInvokeExpression setRegistryName = new CodeMethodInvokeExpression(null, "setRegistryName", new CodeVariableReferenceExpression("name"));
            CodeMethodInvokeExpression setCreativeTab = new CodeMethodInvokeExpression(null, "setCreativeTab", new CodeVariableReferenceExpression($"{Modname}CreativeTab.MODCEATIVETAB"));
            CodeFieldReferenceExpression list = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression($"{Modname}Items"), "ITEMS");
            CodeMethodInvokeExpression addToList = new CodeMethodInvokeExpression(list, "add", new CodeThisReferenceExpression());
            return new CodeExpression[] { super, setUnlocalizedName, setRegistryName, setCreativeTab, addToList };
        }
    }
}
