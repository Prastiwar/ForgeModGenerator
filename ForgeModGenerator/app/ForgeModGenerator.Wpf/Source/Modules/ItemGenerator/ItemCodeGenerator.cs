using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ItemGenerator.Models;
using ForgeModGenerator.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.ItemGenerator.CodeGeneration
{
    public class ItemCodeGenerator : InitVariablesCodeGenerator<Item>
    {
        public ItemCodeGenerator(McMod mcMod) : this(mcMod, Enumerable.Empty<Item>()) { }
        public ItemCodeGenerator(McMod mcMod, IEnumerable<Item> elements) : base(mcMod, elements) => ScriptLocator = SourceCodeLocator.Items(Modname, Organization);

        public override ClassLocator ScriptLocator { get; }

        protected override string GetElementName(Item element) => element.Name;

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeCompileUnit unit = CreateDefaultTargetCodeUnit(ScriptLocator.ClassName, "Item");
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.ItemBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.Materials(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.Hook(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.ItemBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.ArmorBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.BowBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.AxeBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.HoeBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.PickaxeBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.SpadeBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.SwordBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"java.util.ArrayList"));
            unit.Namespaces[0].Imports.Add(NewImport($"java.util.List"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.Item"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.Item.ToolMaterial"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.ItemArmor.ArmorMaterial"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.block.material.Material"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.init.SoundEvents"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.inventory.EntityEquipmentSlot"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.Item"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.ItemAxe"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.ItemHoe"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.ItemPickaxe"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.ItemSpade"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.ItemSword"));
            unit.Namespaces[0].Types[0].Members.Add(NewFieldGlobal("Item", "MODLOGO", NewObject("ItemBase", NewPrimitive("modlogo"))));
            foreach (Item item in Elements)
            {
                string newObjectType = $"{item.Type.ToString()}Base";
                string baseObjectType = null;
                List<CodeExpression> arguments = new List<CodeExpression>() { NewPrimitive(item.Name.ToLower()), NewPrimitive(item.Material) };
                switch (item.Type)
                {
                    case ItemType.Item:
                        baseObjectType = "Item";
                        break;
                    case ItemType.Hoe:
                        baseObjectType = "ItemHoe";
                        break;
                    case ItemType.Axe:
                        baseObjectType = "ItemAxe";
                        break;
                    case ItemType.Sword:
                        baseObjectType = "ItemSword";
                        break;
                    case ItemType.Spade:
                        baseObjectType = "ItemSpade";
                        break;
                    case ItemType.Pickaxe:
                        baseObjectType = "ItemPickaxe";
                        break;
                    case ItemType.Armor:
                        baseObjectType = "Item";
                        switch (item.ArmorType)
                        {
                            case ArmorType.Helmet:
                                arguments.Add(NewPrimitive(1));
                                arguments.Add(NewSnippetExpression("EntityEquipmentSlot.HEAD"));
                                break;
                            case ArmorType.Chestplate:
                                arguments.Add(NewPrimitive(1));
                                arguments.Add(NewSnippetExpression("EntityEquipmentSlot.CHEST"));
                                break;
                            case ArmorType.Leggings:
                                arguments.Add(NewPrimitive(2));
                                arguments.Add(NewSnippetExpression("EntityEquipmentSlot.LEGS"));
                                break;
                            case ArmorType.Boots:
                                arguments.Add(NewPrimitive(1));
                                arguments.Add(NewSnippetExpression("EntityEquipmentSlot.FEET"));
                                break;
                            default:
                                throw new NotImplementedException($"{item.ArmorType} was not implemented");
                        }
                        break;
                    default:
                        throw new NotImplementedException($"{item.Type} was not implemented");
                }
                CodeMemberField field = NewFieldGlobal(baseObjectType, item.Name.ToUpper(), NewObject(newObjectType, arguments.ToArray()));
                unit.Namespaces[0].Types[0].Members.Add(field);
                string jsonPath = Path.Combine(ModPaths.ModelsItemFolder(McMod.ModInfo.Name, McMod.Modid), item.Name.ToLower() + ".json");
                string parent = item.Type == ItemType.Armor ? "generated" : "handheld";
                string jsonText = $@"
{{
    ""parent"": ""item/{parent}"",
    ""textures"": {{
                    ""layer0"": ""{McMod.Modid}:{item.TextureName}""
    }}
}}
";
                // TODO: Do not hard-code json
                File.WriteAllText(jsonPath, jsonText);
            }
            return unit;
        }
    }
}
