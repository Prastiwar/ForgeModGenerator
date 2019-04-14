namespace ForgeModGenerator.CodeGeneration
{
    /// <summary> Constains locators for all classes in source code </summary>
    public static class SourceCodeLocator
    {
        internal const string Prefix = "FMG";

        public static ClassLocator Manager(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, Prefix + "Manager"));

        public static ClassLocator Hook(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, Prefix + "Hook"));

        public static InitClassLocator Items(string modname, string organization) =>
            new InitClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, Prefix + "Items"), "ITEMS");

        public static ClassLocator ItemBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Item, "ItemBase"));

        public static ClassLocator BowBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Bow, "BowBase"));

        public static ClassLocator FoodBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Food, "FoodBase"));

        public static ClassLocator FoodEffectBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Food, "FoodEffectBase"));

        public static ClassLocator ArmorBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Armor, "ArmorBase"));

        public static ClassLocator SwordBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Tool, "SwordBase"));

        public static ClassLocator SpadeBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Tool, "SpadeBase"));

        public static ClassLocator PickaxeBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Tool, "PickaxeBase"));

        public static ClassLocator HoeBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Tool, "HoeBase"));

        public static ClassLocator AxeBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Tool, "AxeBase"));

        public static InitClassLocator Blocks(string modname, string organization) =>
            new InitClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, Prefix + "Blocks"), "BLOCKS");

        public static ClassLocator BlockBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Block, "BlockBase"));

        public static ClassLocator OreBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Block, "OreBase"));

        public static InitClassLocator SoundEvents(string modname, string organization) =>
            new InitClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, Prefix + "SoundEvents"), "SOUNDEVENTS");

        public static ClassLocator SoundEventBase(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Sound, "SoundEventBase"));

        public static InitClassLocator Recipes(string modname, string organization) =>
            new InitClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, Prefix + "Recipes"), "RECIPES");

        public static ClassLocator CreativeTab(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Gui, Prefix + "CreativeTab"));

        public static ClassLocator CommonProxyInterface(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Proxy, "ICommonProxy"));

        public static ClassLocator ServerProxy(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Proxy, "ServerProxy"));

        public static ClassLocator ClientProxy(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Proxy, "ClientProxy"));

        public static ClassLocator ModelInterface(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Handler, "IHasModel"));

        public static ClassLocator RegistryHandler(string modname, string organization) =>
            new ClassLocator(ClassLocator.CombineImport("com", organization, modname, SourceCodeFolders.Root, SourceCodeFolders.Handler, "RegistryHandler"));
    }
}
