using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ItemGenerator.Models;
using ForgeModGenerator.Models;
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
            return unit;
        }
    }
}
