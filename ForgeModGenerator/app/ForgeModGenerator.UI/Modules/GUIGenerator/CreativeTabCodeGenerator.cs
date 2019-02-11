using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ModGenerator.Models;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.GUIGenerator
{
    public class CreativeTabCodeGenerator : ScriptCodeGenerator
    {
        public CreativeTabCodeGenerator(Mod mod) : base(mod) => ScriptFilePath = Path.Combine(ModPaths.GeneratedGuiFolder(Modname, Organization), $"{Modname}CreativeTab.java");

        protected override string ScriptFilePath { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration clas = GetDefaultClass("CreativeTab", true);
            clas.Members.Add(GetCreativeTab());
            CodeNamespace package = GetDefaultPackage(clas, $"import {GeneratedPackageName}.{Modname}Items",
                                                            "import net.minecraft.creativetab.CreativeTabs",
                                                            "import net.minecraft.item.ItemStack",
                                                            "import net.minecraftforge.fml.relauncher.Side",
                                                            "import net.minecraftforge.fml.relauncher.SideOnly");
            return GetDefaultCodeUnit(package);
        }

        private CodeMemberField GetCreativeTab()
        {
            CodeMemberField field = new CodeMemberField("CreativeTabs", "MODCEATIVETAB") {
                Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final,
            };
            CodeObjectCreateExpression initExpression = new CodeObjectCreateExpression("CreativeTabs", new CodePrimitiveExpression($"{Modname}"));
            field.InitExpression = initExpression;
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
};"
            );
            return field;
        }
    }
}
