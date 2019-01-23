using System.IO;

namespace ForgeModGenerator.Core
{
    public static class ModPaths
    {
        public static readonly string FmgInfoFileName = "FmgModInfo.json";

        public static string ModRoot(string modname) => $"{AppPaths.Mods}/{modname}";
        public static string FmgModInfo(string modname) => $"{ModRoot(modname)}/{FmgInfoFileName}";

        public static string Resources(string modname) => $"{ModRoot(modname)}/src/main/resources";
        public static string McModInfo(string modname) => $"{Resources(modname)}/mcmod.info";
        public static string PackMeta(string modname) => $"{Resources(modname)}/pack.mcmeta";
        public static string Assets(string modname, string modid) => $"{Resources(modname)}/assets/{modid}";
        public static string Blockstates(string modname, string modid) => $"{Assets(modname, modid)}/blockstates";
        public static string Lang(string modname, string modid) => $"{Assets(modname, modid)}/lang";
        public static string ModelsItem(string modname, string modid) => $"{Assets(modname, modid)}/models/item";
        public static string Recipes(string modname, string modid) => $"{Assets(modname, modid)}/recipes";
        public static string SoundsFolder(string modname, string modid) => $"{Assets(modname, modid)}/sounds";
        public static string SoundsJson(string modname, string modid) => $"{Assets(modname, modid)}/sounds.json";
        public static string Textures(string modname, string modid) => $"{Assets(modname, modid)}/textures";
        public static string TexturesBlocks(string modname, string modid) => $"{Textures(modname, modid)}/blocks";
        public static string TexturesEntity(string modname, string modid) => $"{Textures(modname, modid)}/entity";
        public static string TexturesItems(string modname, string modid) => $"{Textures(modname, modid)}/items";
        public static string TexturesModelsArmor(string modname, string modid) => $"{Textures(modname, modid)}/models/armor";

        public static string JavaSource(string modname) => $"{ModRoot(modname)}/src/main/java/com/";
        public static string OrganizationRoot(string modname, string organization) => Path.Combine(JavaSource(modname), organization);
        public static string SourceCodeRoot(string modname, string organization) => Path.Combine(JavaSource(modname), organization, modname.ToLower());
        public static string GeneratedSourceCode(string modname, string organization) => Path.Combine(SourceCodeRoot(modname, organization), "generated");

        public static string GeneratedBlockFolder(string modname, string organization) => Path.Combine(GeneratedSourceCode(modname, organization), "block");
        public static string GeneratedItemFolder(string modname, string organization) => Path.Combine(GeneratedSourceCode(modname, organization), "item");
        public static string GeneratedProxyFolder(string modname, string organization) => Path.Combine(GeneratedSourceCode(modname, organization), "proxy");
        public static string GeneratedInterfaceFolder(string modname, string organization) => Path.Combine(GeneratedSourceCode(modname, organization), "interface");
        public static string GeneratedEnchantFolder(string modname, string organization) => Path.Combine(GeneratedSourceCode(modname, organization), "enchant");
        public static string GeneratedGuiFolder(string modname, string organization) => Path.Combine(GeneratedSourceCode(modname, organization), "gui");
        public static string GeneratedHandlerFolder(string modname, string organization) => Path.Combine(GeneratedSourceCode(modname, organization), "handler");
        public static string GeneratedSoundFolder(string modname, string organization) => Path.Combine(GeneratedSourceCode(modname, organization), "sound");
        public static string GeneratedWorldFolder(string modname, string organization) => Path.Combine(GeneratedSourceCode(modname, organization), "world");

        public static string GeneratedModManagerFile(string modname, string organization) => $"{GeneratedSourceCode(modname, organization)}/{modname}.java";
        public static string GeneratedModHookFile(string modname, string organization) => $"{GeneratedSourceCode(modname, organization)}/{modname}Hook.java";
        public static string GeneratedWorldGenFile(string modname, string organization) => $"{GeneratedWorldFolder(modname, organization)}/{modname}WorldGen.java";
        public static string GeneratedBlocksFile(string modname, string organization) => $"{GeneratedSourceCode(modname, organization)}/{modname}Blocks.java";
        public static string GeneratedItemsFile(string modname, string organization) => $"{GeneratedSourceCode(modname, organization)}/{modname}Items.java";
        public static string GeneratedRecipesFile(string modname, string organization) => $"{GeneratedSourceCode(modname, organization)}/{modname}Recipes.java";
        public static string GeneratedSoundsFile(string modname, string organization) => $"{GeneratedSourceCode(modname, organization)}/{modname}Sounds.java";
    }
}
