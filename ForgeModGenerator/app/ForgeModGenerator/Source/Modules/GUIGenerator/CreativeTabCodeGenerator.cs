using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.GUIGenerator
{
    public class CreativeTabCodeGenerator : ScriptCodeGenerator
    {
        public CreativeTabCodeGenerator(Mod mod) : base(mod) 
            => ScriptFilePath = Path.Combine(ModPaths.SourceCodeRootFolder(Modname, Organization), SourceCodeLocator.CreativeTab.RelativePath);

        protected override string ScriptFilePath { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration clas = NewClassWithMembers(SourceCodeLocator.CreativeTab.ClassName);
            clas.Members.Add(GetCreativeTab());
            CodeNamespace package = NewPackage(clas, $"{PackageName}.{SourceCodeLocator.Items.ImportFullName}",
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
$"{System.Environment.NewLine}    	return new ItemStack({SourceCodeLocator.Items.ClassName}.MODLOGO, 1);" + @"
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
