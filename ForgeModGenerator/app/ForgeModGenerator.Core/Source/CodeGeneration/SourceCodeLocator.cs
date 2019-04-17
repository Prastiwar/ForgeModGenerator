using Microsoft.Extensions.Caching.Memory;
using System;
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
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "Manager")), cacheExpirationTime);
        }

        public static ClassLocator Hook(string modname, string organization)
        {
            string key = GetKey(nameof(Hook), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "Hook")), cacheExpirationTime);
        }

        public static InitClassLocator Items(string modname, string organization)
        {
            string key = GetKey(nameof(Items), modname, organization);
            if (cache.TryGetValue(key, out InitClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new InitClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "Items"), "ITEMS"), cacheExpirationTime);
        }

        public static ClassLocator ItemBase(string modname, string organization)
        {
            string key = GetKey(nameof(ItemBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, "ItemBase")), cacheExpirationTime);
        }

        public static ClassLocator BowBase(string modname, string organization)
        {
            string key = GetKey(nameof(BowBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Bow, "BowBase")), cacheExpirationTime);
        }

        public static ClassLocator FoodBase(string modname, string organization)
        {
            string key = GetKey(nameof(FoodBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Food, "FoodBase")), cacheExpirationTime);
        }

        public static ClassLocator FoodEffectBase(string modname, string organization)
        {
            string key = GetKey(nameof(FoodEffectBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Food, "FoodEffectBase")), cacheExpirationTime);
        }

        public static ClassLocator ArmorBase(string modname, string organization)
        {
            string key = GetKey(nameof(ArmorBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Armor, "ArmorBase")), cacheExpirationTime);
        }

        public static ClassLocator SwordBase(string modname, string organization)
        {
            string key = GetKey(nameof(SwordBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Tool, "SwordBase")), cacheExpirationTime);
        }

        public static ClassLocator SpadeBase(string modname, string organization)
        {
            string key = GetKey(nameof(SpadeBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Tool, "SpadeBase")), cacheExpirationTime);
        }

        public static ClassLocator PickaxeBase(string modname, string organization)
        {
            string key = GetKey(nameof(PickaxeBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Tool, "PickaxeBase")), cacheExpirationTime);
        }

        public static ClassLocator HoeBase(string modname, string organization)
        {
            string key = GetKey(nameof(HoeBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Tool, "HoeBase")), cacheExpirationTime);
        }

        public static ClassLocator AxeBase(string modname, string organization)
        {
            string key = GetKey(nameof(AxeBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Item, SourceCodeFolders.Tool, "AxeBase")), cacheExpirationTime);
        }

        public static InitClassLocator Blocks(string modname, string organization)
        {
            string key = GetKey(nameof(Blocks), modname, organization);
            if (cache.TryGetValue(key, out InitClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new InitClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "Blocks"), "BLOCKS"), cacheExpirationTime);
        }

        public static ClassLocator BlockBase(string modname, string organization)
        {
            string key = GetKey(nameof(BlockBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Block, "BlockBase")), cacheExpirationTime);
        }

        public static ClassLocator OreBase(string modname, string organization)
        {
            string key = GetKey(nameof(OreBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Block, "OreBase")), cacheExpirationTime);
        }

        public static InitClassLocator SoundEvents(string modname, string organization)
        {
            string key = GetKey(nameof(SoundEvents), modname, organization);
            if (cache.TryGetValue(key, out InitClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new InitClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "SoundEvents"), "SOUNDEVENTS"), cacheExpirationTime);
        }

        public static ClassLocator SoundEventBase(string modname, string organization)
        {
            string key = GetKey(nameof(SoundEventBase), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Sound, "SoundEventBase")), cacheExpirationTime);
        }

        public static InitClassLocator Recipes(string modname, string organization)
        {
            string key = GetKey(nameof(Recipes), modname, organization);
            if (cache.TryGetValue(key, out InitClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new InitClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), Prefix + "Recipes"), "RECIPES"), cacheExpirationTime);
        }

        public static ClassLocator CreativeTab(string modname, string organization)
        {
            string key = GetKey(nameof(CreativeTab), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Gui, Prefix + "CreativeTab")), cacheExpirationTime);
        }

        public static ClassLocator CommonProxyInterface(string modname, string organization)
        {
            string key = GetKey(nameof(CommonProxyInterface), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Proxy, "ICommonProxy")), cacheExpirationTime);
        }

        public static ClassLocator ServerProxy(string modname, string organization)
        {
            string key = GetKey(nameof(ServerProxy), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Proxy, "ServerProxy")), cacheExpirationTime);
        }

        public static ClassLocator ClientProxy(string modname, string organization)
        {
            string key = GetKey(nameof(ClientProxy), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Proxy, "ClientProxy")), cacheExpirationTime);
        }

        public static ClassLocator ModelInterface(string modname, string organization)
        {
            string key = GetKey(nameof(ModelInterface), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Handler, "IHasModel")), cacheExpirationTime);
        }

        public static ClassLocator RegistryHandler(string modname, string organization)
        {
            string key = GetKey(nameof(RegistryHandler), modname, organization);
            if (cache.TryGetValue(key, out ClassLocator value))
            {
                return value;
            }
            return cache.Set(key, new ClassLocator(ClassLocator.CombineImport(GetPackageName(modname, organization), SourceCodeFolders.Handler, "RegistryHandler")), cacheExpirationTime);
        }
        public static void Initialize(IMemoryCache cache)
        {
            SourceCodeLocator.cache = cache;
            isInitialized = true;
        }

        /// <summary> Returns package name in root com.organization.modname.root </summary>
        private static string GetPackageName(string modname, string organization) => ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root);

        /// <summary> Returns cache key for specific Mod and locator </summary>
        private static string GetKey(string locator, string modname, string organization)
        {
            InitCheck();
            keyBuilder.Clear();
            keyBuilder.Append(modname);
            keyBuilder.Append('.');
            keyBuilder.Append(organization);
            keyBuilder.Append('.');
            keyBuilder.Append(locator);
            return keyBuilder.ToString();
        }

        private static void InitCheck()
        {
            if (!isInitialized)
            {
                throw new ClassNotInitializedException(typeof(Log));
            }
        }
    }
}
