using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CodeGeneration.CodeDom;
using ForgeModGenerator.Models;
using ForgeModGenerator.RecipeGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace ForgeModGenerator.RecipeGenerator.CodeGeneration
{
    public class RecipeCodeGenerator : InitVariablesCodeGenerator<Recipe>
    {
        public RecipeCodeGenerator(McMod mcMod) : this(mcMod, Enumerable.Empty<Recipe>()) { }
        public RecipeCodeGenerator(McMod mcMod, IEnumerable<Recipe> elements) : base(mcMod, elements) => ScriptLocator = SourceCodeLocator.Recipes(Modname, Organization);

        public override ClassLocator ScriptLocator { get; }

        protected override string GetElementName(Recipe element) => null;

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeMemberMethod initMethod = NewMethod("init", typeof(void).FullName, JavaAttributes.StaticOnly);
            CodeCompileUnit unit = NewCodeUnit(ScriptLocator.PackageName, NewClassWithMembers(ScriptLocator.ClassName, initMethod));
            foreach (Recipe recipe in Elements)
            {
                if (recipe is SmeltingRecipe smeltingRecipe)
                {
                    // TODO: REVIEW: json factory (mc 1.13 change)
                    CodeMethodInvokeExpression addSmelting = NewMethodInvokeType("GameRegistry", "addSmelting");
                    //addSmelting.Parameters.Add(NewPrimitive(smeltingRecipe.Ingredients[0].Item));
                    //addSmelting.Parameters.Add(NewPrimitive(smeltingRecipe.Result.Item));
                    //addSmelting.Parameters.Add(NewPrimitive(smeltingRecipe.Result.Count));
                    //addSmelting.Parameters.Add(NewPrimitive(smeltingRecipe.CookingTime));
                    initMethod.Statements.Add(addSmelting);
                }
            }
            return unit;
        }
    }
}
