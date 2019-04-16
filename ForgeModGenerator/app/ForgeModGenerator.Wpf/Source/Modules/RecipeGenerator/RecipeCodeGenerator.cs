using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using ForgeModGenerator.RecipeGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;

namespace ForgeModGenerator.RecipeGenerator.CodeGeneration
{
    public class RecipeCodeGenerator : InitVariablesCodeGenerator<Recipe>
    {
        public RecipeCodeGenerator(Mod mod) : this(mod, null) { }
        public RecipeCodeGenerator(Mod mod, IEnumerable<Recipe> elements) : base(mod, elements) => ScriptLocator = SourceCodeLocator.Recipes(Modname, Organization);

        public override ClassLocator ScriptLocator { get; }

        protected override string GetElementName(Recipe element) => null;

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeMemberMethod initMethod = NewMethod("init", typeof(void).FullName, MemberAttributes.Public | MemberAttributes.Static);
            CodeCompileUnit unit = NewCodeUnit(ScriptLocator.PackageName, NewClassWithMembers(ScriptLocator.ClassName, initMethod));
            return unit;
        }
    }
}
