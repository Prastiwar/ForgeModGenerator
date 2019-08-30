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
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.Item"));
            unit.Namespaces[0].Types[0].Members.Add(NewFieldGlobal("Item", "MODLOGO", NewObject("ItemBase", NewPrimitive("modlogo"))));
            foreach (Item item in Elements)
            {
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
                CodeMemberField materialField = null;
                if (item.Type == ItemType.Armor)
                {
                    ArmorMaterial material = (ArmorMaterial)item.Material;
                    materialField = NewFieldGlobal("ArmorMaterial", material.Name.ToUpper(), NewMethodInvokeType("EnumHelper", "addArmorMaterial",
                        NewPrimitive(material.Name.ToLower()),
                        NewPrimitive(material.TextureName),
                        NewPrimitive(material.Durability),
                        NewPrimitveArray(material.DamageReductionAmountArray),
                        NewPrimitive(material.Enchantability),
                        NewPrimitive(material.SoundEvent),
                        NewPrimitive(material.Toughness)
                        ));
                }
                else
                {
                    ToolMaterial material = (ToolMaterial)item.Material;
                    materialField = NewFieldGlobal("ToolMaterial", material.Name.ToUpper(), NewMethodInvokeType("EnumHelper", "addToolMaterial",
                        NewPrimitive(material.Name.ToLower()),
                        NewPrimitive(material.HarvestLevel),
                        NewPrimitive(material.MaxUses),
                        NewPrimitive(material.Efficiency),
                        NewPrimitive(material.AttackDamage),
                        NewPrimitive(material.Enchantability)
                        ));
                }
                CodeMemberField field = NewFieldGlobal(baseObjectType, item.Name.ToUpper(), NewObject(newObjectType, arguments.ToArray()));
                unit.Namespaces[0].Types[0].Members.Add(materialField);
                unit.Namespaces[0].Types[0].Members.Add(field);
                string jsonPath = Path.Combine(ModPaths.ModelsItemFolder(McMod.ModInfo.Name, McMod.Modid), item.Name.ToLower() + ".json");
                string parent = item.Type == ItemType.Armor ? "generated" : "handheld";
                string jsonText = $@"
{{
    ""parent"": ""item/{parent}"",
    ""textures"": {{
                    ""layer0"": ""modname:items/azurite_axe""
    }}
}}
";
                // TODO: Do not hard-code json
                // TODO: Generate json
            }
            return unit;
        }
    }
}
