using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ItemGenerator.Models;
using ForgeModGenerator.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
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
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.Item"));
            unit.Namespaces[0].Types[0].Members.Add(NewFieldGlobal("Item", "MODLOGO", NewObject("ItemBase", NewPrimitive("modlogo"))));
            foreach (Item item in Elements)
            {
                // TODO: Consider material,
                string newObjectType = $"{item.Type.ToString()}Base";
                string baseObjectType = null;
                List<CodeExpression> arguments = new List<CodeExpression>() { NewPrimitive(item.Name.ToLower()) };
                CodeExpression materialExpression = null;
                switch (item.Type)
                {
                    case ItemType.Item:
                        baseObjectType = "Item";
                        arguments.Add(materialExpression);
                        break;
                    case ItemType.Hoe:
                        baseObjectType = "ItemHoe";
                        arguments.Add(materialExpression);
                        break;
                    case ItemType.Axe:
                        baseObjectType = "ItemAxe";
                        arguments.Add(materialExpression);
                        break;
                    case ItemType.Sword:
                        baseObjectType = "ItemSword";
                        arguments.Add(materialExpression);
                        break;
                    case ItemType.Spade:
                        baseObjectType = "ItemSpade";
                        arguments.Add(materialExpression);
                        break;
                    case ItemType.Pickaxe:
                        baseObjectType = "ItemPickaxe";
                        arguments.Add(materialExpression);
                        break;
                    case ItemType.Armor:
                        baseObjectType = "Item";
                        switch (item.ArmorType)
                        {
                            case ArmorType.Helmet:
                                arguments.Add(materialExpression);
                                arguments.Add(NewPrimitive(1));
                                arguments.Add(NewSnippetExpression("EntityEquipmentSlot.HEAD"));
                                break;
                            case ArmorType.Chestplate:
                                arguments.Add(materialExpression);
                                arguments.Add(NewPrimitive(1));
                                arguments.Add(NewSnippetExpression("EntityEquipmentSlot.CHEST"));
                                break;
                            case ArmorType.Leggings:
                                arguments.Add(materialExpression);
                                arguments.Add(NewPrimitive(2));
                                arguments.Add(NewSnippetExpression("EntityEquipmentSlot.LEGS"));
                                break;
                            case ArmorType.Boots:
                                arguments.Add(materialExpression);
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
            }
            return unit;
        }
    }
}
