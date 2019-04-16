using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.CodeDom;

namespace ForgeModGenerator.GUIGenerator
{
    public class CreativeTabCodeGenerator : ScriptCodeGenerator
    {
        public CreativeTabCodeGenerator(Mod mod) : base(mod) => ScriptLocator = SourceCodeLocator.CreativeTab(Modname, Organization);

        public override ClassLocator ScriptLocator { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration clas = NewClassWithMembers(ScriptLocator.ClassName);
            clas.Members.Add(GetCreativeTab());
            CodeNamespace package = NewPackage(SourceCodeLocator.CreativeTab(Modname, Organization).PackageName, clas,
                                                      $"{SourceRootPackageName}.{SourceCodeLocator.Items(Modname, Organization).ImportRelativeName}",
                                                      "net.minecraft.creativetab.CreativeTabs",
                                                      "net.minecraft.item.ItemStack",
                                                      "net.minecraftforge.fml.relauncher.Side",
                                                      "net.minecraftforge.fml.relauncher.SideOnly");
            return NewCodeUnit(package);
        }

        private CodeMemberField GetCreativeTab()
        {
            CodeMemberField field = new CodeMemberField("CreativeTabs", "MODCEATIVETAB") {
                Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final,
            };
            field.InitExpression = new CodeSnippetExpression(
$"new CreativeTabs(\"{Modname}\") {{" + @"
    @SideOnly(Side.CLIENT)
    @Override
    public ItemStack getTabIconItem() {" +
$"{System.Environment.NewLine}    	return new ItemStack({SourceCodeLocator.Items(Modname, Organization).ClassName}.MODLOGO, 1);" + @"
    }

    @SideOnly(Side.CLIENT)
    @Override
    public boolean hasSearchBar() {
    	return true;
    }
}"
            );
            return field;
        }
    }
}
