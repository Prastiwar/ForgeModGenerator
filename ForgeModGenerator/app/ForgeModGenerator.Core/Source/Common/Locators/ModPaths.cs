using System.IO;

namespace ForgeModGenerator
{
    /// <summary> Constains common mod path locations </summary>
    public static class ModPaths
    {
        public static readonly string FmgInfoFileName = "FmgModInfo.json";

        public static string ModRootFolder(string modname) => Path.Combine(AppPaths.Mods, modname);
        public static string FmgModInfoFile(string modname) => Path.Combine(ModRootFolder(modname), FmgInfoFileName);

        public static string ResourcesFolder(string modname) => Path.Combine(ModRootFolder(modname), "src", "main", "resources");
        public static string McModInfoFile(string modname) => Path.Combine(ResourcesFolder(modname), "mcmod.info");
        public static string PackMetaFile(string modname) => Path.Combine(ResourcesFolder(modname), "pack.mcmeta");

        public static string AssetsFolder(string modname) => Path.Combine(ResourcesFolder(modname), "assets");
        public static string AssetsFolder(string modname, string modid) => Path.Combine(AssetsFolder(modname), modid);

        public static string LangFolder(string modname, string modid) => Path.Combine(AssetsFolder(modname, modid), "lang");

        public static string Blockstates(string modname, string modid) => Path.Combine(AssetsFolder(modname, modid), "blockstates");
        public static string ModelsItemFolder(string modname, string modid) => Path.Combine(AssetsFolder(modname, modid), "models", "item");
        public static string RecipesFolder(string modname, string modid) => Path.Combine(AssetsFolder(modname, modid), "recipes");

        public static string SoundsFolder(string modname, string modid) => Path.Combine(AssetsFolder(modname, modid), "sounds");
        public static string SoundsJson(string modname, string modid) => Path.Combine(AssetsFolder(modname, modid), "sounds.json");

        public static string TexturesFolder(string modname, string modid) => Path.Combine(AssetsFolder(modname, modid), "textures");
        public static string TexturesBlocksFolder(string modname, string modid) => Path.Combine(TexturesFolder(modname, modid), "blocks");
        public static string TexturesEntityFolder(string modname, string modid) => Path.Combine(TexturesFolder(modname, modid), "entity");
        public static string TexturesItemsFolder(string modname, string modid) => Path.Combine(TexturesFolder(modname, modid), "items");
        public static string TexturesModelsArmorFolder(string modname, string modid) => Path.Combine(TexturesFolder(modname, modid), "models", "armor");

        public static string JavaSourceFolder(string modname) => Path.Combine(ModRootFolder(modname), "src", "main", "java", "com");
        public static string OrganizationRootFolder(string modname, string organization) => Path.Combine(JavaSourceFolder(modname), organization);
        public static string SourceCodeRootFolder(string modname, string organization) => Path.Combine(JavaSourceFolder(modname), organization, modname.ToLower());
    }
}
