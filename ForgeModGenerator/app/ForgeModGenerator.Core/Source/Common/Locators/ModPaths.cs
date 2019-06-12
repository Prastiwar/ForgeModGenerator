using System.IO;

namespace ForgeModGenerator
{
    /// <summary> Constains common mod path locations </summary>
    public static class ModPaths
    {
        public static readonly string FmgInfoFileName = "FmgModInfo.json";

        public static string ModRootFolder(string modname) => Path.Combine(AppPaths.Mods, modname);
        public static string FmgModInfoFile(string modname) => Path.Combine(ModRootFolder(modname), FmgInfoFileName);

        public static string ResourcesFolder(string modname)
        {
            string path = Path.Combine(ModRootFolder(modname), "src", "main", "resources");
            CreateIfNotExist(path);
            return path;
        }

        public static string McModInfoFile(string modname) => Path.Combine(ResourcesFolder(modname), "mcmod.info");
        public static string PackMetaFile(string modname) => Path.Combine(ResourcesFolder(modname), "pack.mcmeta");

        public static string AssetsFolder(string modname)
        {
            string path = Path.Combine(ResourcesFolder(modname), "assets");
            CreateIfNotExist(path);
            return path;
        }

        public static string AssetsFolder(string modname, string modid)
        {
            string path = Path.Combine(AssetsFolder(modname), modid);
            CreateIfNotExist(path);
            return path;
        }

        public static string LangFolder(string modname, string modid)
        {
            string path = Path.Combine(AssetsFolder(modname, modid), "lang");
            CreateIfNotExist(path);
            return path;
        }

        public static string Blockstates(string modname, string modid)
        {
            string path = Path.Combine(AssetsFolder(modname, modid), "blockstates");
            CreateIfNotExist(path);
            return path;
        }

        public static string ModelsItemFolder(string modname, string modid)
        {
            string path = Path.Combine(AssetsFolder(modname, modid), "models", "item");
            CreateIfNotExist(path);
            return path;
        }

        public static string RecipesFolder(string modname, string modid)
        {
            string path = Path.Combine(AssetsFolder(modname, modid), "recipes");
            CreateIfNotExist(path);
            return path;
        }

        public static string SoundsFolder(string modname, string modid)
        {
            string path = Path.Combine(AssetsFolder(modname, modid), "sounds");
            CreateIfNotExist(path);
            return path;
        }

        public static string SoundsJson(string modname, string modid) => Path.Combine(AssetsFolder(modname, modid), "sounds.json");

        public static string TexturesFolder(string modname, string modid)
        {
            string path = Path.Combine(AssetsFolder(modname, modid), "textures");
            CreateIfNotExist(path);
            return path;
        }

        public static string TexturesBlocksFolder(string modname, string modid)
        {
            string path = Path.Combine(TexturesFolder(modname, modid), "blocks");
            CreateIfNotExist(path);
            return path;
        }

        public static string TexturesEntityFolder(string modname, string modid)
        {
            string path = Path.Combine(TexturesFolder(modname, modid), "entity");
            CreateIfNotExist(path);
            return path;
        }

        public static string TexturesItemsFolder(string modname, string modid)
        {
            string path = Path.Combine(TexturesFolder(modname, modid), "items");
            CreateIfNotExist(path);
            return path;
        }

        public static string TexturesModelsArmorFolder(string modname, string modid)
        {
            string path = Path.Combine(TexturesFolder(modname, modid), "models", "armor");
            CreateIfNotExist(path);
            return path;
        }

        public static string JavaSourceFolder(string modname)
        {
            string path = Path.Combine(ModRootFolder(modname), "src", "main", "java", "com");
            CreateIfNotExist(path);
            return path;
        }

        public static string OrganizationRootFolder(string modname, string organization) => Path.Combine(JavaSourceFolder(modname), organization);
        public static string SourceCodeRootFolder(string modname, string organization) => Path.Combine(JavaSourceFolder(modname), organization, modname.ToLower());

        private static void CreateIfNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
