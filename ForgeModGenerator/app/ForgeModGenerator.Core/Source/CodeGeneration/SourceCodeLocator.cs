using ForgeModGenerator.Utility;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Text;

namespace ForgeModGenerator.CodeGeneration
{
    /// <summary> Constains locators for all classes in source code </summary>
    public static class SourceCodeLocator
    {
        internal const string Prefix = "FMG";

        private static bool isInitialized;
        private static IMemoryCache cache;

        private static readonly StringBuilder keyBuilder = new StringBuilder(32);
        private static readonly TimeSpan cacheExpirationTime = TimeSpan.FromMinutes(10);

        public static ClassLocator Manager(string modname, string organization)
        {
            string key = GetKey(nameof(Manager), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "Manager"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator Hook(string modname, string organization)
        {
            string key = GetKey(nameof(Hook), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "Hook"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator Commands(string modname, string organization)
        {
            string key = GetKey(nameof(Commands), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "Commands"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator CustomCommand(string modname, string organization, string className)
        {
            string key = GetKey(nameof(CustomCommand), modname, organization, className);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Command, className));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        #region Item locators
        public static InitClassLocator Items(string modname, string organization)
        {
            string key = GetKey(nameof(Items), modname, organization);
            if (cache.TryGetValue(key, out InitClassLocator value))
            {
                return value;
            }
            InitClassLocator locator = new InitClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "Items"), "ITEMS");
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator ItemBase(string modname, string organization)
        {
            string key = GetKey(nameof(ItemBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, "ItemBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator BowBase(string modname, string organization)
        {
            string key = GetKey(nameof(BowBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Bow, "BowBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator FoodBase(string modname, string organization)
        {
            string key = GetKey(nameof(FoodBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Food, "FoodBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator FoodEffectBase(string modname, string organization)
        {
            string key = GetKey(nameof(FoodEffectBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Food, "FoodEffectBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator ArmorBase(string modname, string organization)
        {
            string key = GetKey(nameof(ArmorBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Armor, "ArmorBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator SwordBase(string modname, string organization)
        {
            string key = GetKey(nameof(SwordBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Tool, "SwordBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator SpadeBase(string modname, string organization)
        {
            string key = GetKey(nameof(SpadeBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Tool, "SpadeBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator PickaxeBase(string modname, string organization)
        {
            string key = GetKey(nameof(PickaxeBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Tool, "PickaxeBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator HoeBase(string modname, string organization)
        {
            string key = GetKey(nameof(HoeBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Tool, "HoeBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator AxeBase(string modname, string organization)
        {
            string key = GetKey(nameof(AxeBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Tool, "AxeBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }
        #endregion

        #region Block locators
        public static InitClassLocator Blocks(string modname, string organization)
        {
            string key = GetKey(nameof(Blocks), modname, organization);
            if (cache.TryGetValue(key, out InitClassLocator value))
            {
                return value;
            }
            InitClassLocator locator = new InitClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "Blocks"), "BLOCKS");
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator BlockBase(string modname, string organization)
        {
            string key = GetKey(nameof(BlockBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Block, "BlockBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator OreBase(string modname, string organization)
        {
            string key = GetKey(nameof(OreBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Block, "OreBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }
        #endregion

        public static InitClassLocator SoundEvents(string modname, string organization)
        {
            string key = GetKey(nameof(SoundEvents), modname, organization);
            if (cache.TryGetValue(key, out InitClassLocator value))
            {
                return value;
            }
            InitClassLocator locator = new InitClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "SoundEvents"), "SOUNDEVENTS");
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator SoundEventBase(string modname, string organization)
        {
            string key = GetKey(nameof(SoundEventBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Sound, "SoundEventBase"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static InitClassLocator Recipes(string modname, string organization)
        {
            string key = GetKey(nameof(Recipes), modname, organization);
            if (cache.TryGetValue(key, out InitClassLocator value))
            {
                return value;
            }
            InitClassLocator locator = new InitClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "Recipes"), "RECIPES");
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator CreativeTab(string modname, string organization)
        {
            string key = GetKey(nameof(CreativeTab), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Gui, Prefix + "CreativeTab"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator CommonProxyInterface(string modname, string organization)
        {
            string key = GetKey(nameof(CommonProxyInterface), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Proxy, "ICommonProxy"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator ServerProxy(string modname, string organization)
        {
            string key = GetKey(nameof(ServerProxy), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Proxy, "ServerProxy"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator ClientProxy(string modname, string organization)
        {
            string key = GetKey(nameof(ClientProxy), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Proxy, "ClientProxy"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator ModelInterface(string modname, string organization)
        {
            string key = GetKey(nameof(ModelInterface), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Handler, "IHasModel"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static ClassLocator RegistryHandler(string modname, string organization)
        {
            string key = GetKey(nameof(RegistryHandler), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            ClassLocator locator = new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Handler, "RegistryHandler"));
            CreateIfNotExist(locator.FullPath);
            return cache.Set(key, locator, cacheExpirationTime);
        }

        public static void Initialize(IMemoryCache cache)
        {
            SourceCodeLocator.cache = cache;
            isInitialized = true;
        }

        /// <summary> Returns package name in root com.organization.modname.root </summary>
        private static string GetPackageName(string modname, string organization) => ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root);

        /// <summary> Returns cache key for specific Mod and locator </summary>
        private static string GetKey(params string[] ids)
        {
            InitCheck();
            keyBuilder.Clear();
            int lastIndex = ids.Length - 1;
            for (int i = 0; i < ids.Length; i++)
            {
                keyBuilder.Append(ids[i]);
                if (i < lastIndex)
                {
                    keyBuilder.Append('.');
                }
            }
            return keyBuilder.ToString();
        }

        private static void InitCheck()
        {
            if (!isInitialized)
            {
                throw new ClassNotInitializedException(typeof(SourceCodeLocator).FullName);
            }
        }

        private static void CreateIfNotExist(string path)
        {
            string dirPath = IOHelper.GetDirectoryPath(path);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
    }
}
