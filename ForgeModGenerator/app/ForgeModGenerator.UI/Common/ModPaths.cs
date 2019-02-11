using System.IO;

namespace ForgeModGenerator
{
    public static class ModPaths
    {
        public static readonly string FmgInfoFileName = "FmgModInfo.json";

        public static string ModRoot(string modname) => Path.Combine(AppPaths.Mods, modname);
        public static string FmgModInfo(string modname) => Path.Combine(ModRoot(modname), FmgInfoFileName);

        public static string Resources(string modname) => Path.Combine(ModRoot(modname), "src", "main", "resources");
        public static string McModInfo(string modname) => Path.Combine(Resources(modname), "mcmod.info");
        public static string PackMeta(string modname) => Path.Combine(Resources(modname), "pack.mcmeta");
        public static string Assets(string modname) => Path.Combine(Resources(modname), "assets");
        public static string Assets(string modname, string modid) => Path.Combine(Assets(modname), modid);
        public static string Blockstates(string modname, string modid) => Path.Combine(Assets(modname, modid), "blockstates");
        public static string Lang(string modname, string modid) => Path.Combine(Assets(modname, modid), "lang");
        public static string ModelsItem(string modname, string modid) => Path.Combine(Assets(modname, modid), "models", "item");
        public static string Recipes(string modname, string modid) => Path.Combine(Assets(modname, modid), "recipes");
        public static string SoundsFolder(string modname, string modid) => Path.Combine(Assets(modname, modid), "sounds");
        public static string SoundsJson(string modname, string modid) => Path.Combine(Assets(modname, modid), "sounds.json");
        public static string Textures(string modname, string modid) => Path.Combine(Assets(modname, modid), "textures");
        public static string TexturesBlocks(string modname, string modid) => Path.Combine(Textures(modname, modid), "blocks");
        public static string TexturesEntity(string modname, string modid) => Path.Combine(Textures(modname, modid), "entity");
        public static string TexturesItems(string modname, string modid) => Path.Combine(Textures(modname, modid), "items");
        public static string TexturesModelsArmor(string modname, string modid) => Path.Combine(Textures(modname, modid), "models", "armor");

        public static string JavaSource(string modname) => Path.Combine(ModRoot(modname), "src", "main", "java", "com");
        public static string OrganizationRoot(string modname, string organization) => Path.Combine(JavaSource(modname), organization);
        public static string SourceCodeRoot(string modname, string organization) => Path.Combine(JavaSource(modname), organization, modname.ToLower());
        public static string GeneratedSourceCodeFolder(string modname, string organization) => Path.Combine(SourceCodeRoot(modname, organization), "generated");
    }
}
