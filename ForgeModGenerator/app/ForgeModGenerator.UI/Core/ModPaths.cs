using System.IO;

namespace ForgeModGenerator.Core
{
    public static class ModPaths
    {
        public static string FmgModInfo(string modname) => $"{AppPaths.Mods}/{modname}/FmgModInfo.json";

        public static string Resources(string modname) => $"{AppPaths.Mods}/{modname}/src/main/resources";
        public static string McModInfo(string modname) => $"{Resources(modname)}/mcmod.info";
        public static string PackMeta(string modname) => $"{Resources(modname)}/ pack.mcmeta";
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

        public static string SourceCodeRoot(string modname, string modid, string organization) => $"{AppPaths.Mods}/{modname}/src/main/java/com/{organization}/{modid}";
        public static string GeneratedSourceCode(string modname, string modid, string organization) => Path.Combine(SourceCodeRoot(modname, modid, organization), "generated");
        public static string GeneratedModManagerFile(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/{modname}.java";
        public static string GeneratedModHookFile(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/{modname}Hook.java";

        public static string GeneratedBlockFolder(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/block";
        public static string GeneratedItemFolder(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/item";
        public static string GeneratedProxyFolder(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/proxy";
        public static string GeneratedInterfaceFolder(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/interface";
        public static string GeneratedEnchantFolder(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/enchant";
        public static string GeneratedGuiFolder(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/gui";
        public static string GeneratedHandlerFolder(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/handler";
        public static string GeneratedSoundFolder(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/sound";

        public static string GeneratedWorldGenFile(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/world/{modname}WorldGen.java";
        public static string GeneratedBlocksFile(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/{modname}Blocks.java";
        public static string GeneratedItemsFile(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/{modname}Items.java";
        public static string GeneratedRecipesFile(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/{modname}Recipes.java";
        public static string GeneratedSoundsFile(string modname, string modid, string organization) => $"{GeneratedSourceCode(modname, modid, organization)}/{modname}Sounds.java";
    }
}
