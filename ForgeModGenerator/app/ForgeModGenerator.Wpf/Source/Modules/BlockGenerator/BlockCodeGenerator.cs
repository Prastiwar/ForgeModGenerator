using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace ForgeModGenerator.BlockGenerator.CodeGeneration
{
    public class BlockCodeGenerator : InitVariablesCodeGenerator<Block>
    {
        public BlockCodeGenerator(McMod mcMod) : this(mcMod, Enumerable.Empty<Block>()) { }
        public BlockCodeGenerator(McMod mcMod, IEnumerable<Block> elements) : base(mcMod, elements) => ScriptLocator = SourceCodeLocator.Blocks(Modname, Organization);

        public override ClassLocator ScriptLocator { get; }

        protected override string GetElementName(Block element) => element.Name;

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeCompileUnit unit = CreateDefaultTargetCodeUnit(ScriptLocator.ClassName, "Block");
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.BlockBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.block.Block"));
            return unit;
        }
    }
}
