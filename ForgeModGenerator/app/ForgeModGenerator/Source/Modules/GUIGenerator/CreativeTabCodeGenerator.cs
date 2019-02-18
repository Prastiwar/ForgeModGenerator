using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.GUIGenerator
{
    public class CreativeTabCodeGenerator : ScriptCodeGenerator
    {
        public CreativeTabCodeGenerator(Mod mod) : base(mod) => ScriptFilePath = Path.Combine(ModPaths.GeneratedSourceCodeFolder(Modname, Organization), "gui", Modname + "CreativeTab.java");

        protected override string ScriptFilePath { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration clas = NewClassWithMembers("CreativeTab", true);
            clas.Members.Add(GetCreativeTab());
            CodeNamespace package = NewPackage(clas, $"{GeneratedPackageName}.{Modname}Items",
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
$"public static final CreativeTabs MODCEATIVETAB = new CreativeTabs(\"{Modname}\") {{" + @"
    @SideOnly(Side.CLIENT)
    @Override
    public ItemStack getTabIconItem() {" +
$"    	return new ItemStack({Modname}Items.{Modname}LOGO, 1);" + @"
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
