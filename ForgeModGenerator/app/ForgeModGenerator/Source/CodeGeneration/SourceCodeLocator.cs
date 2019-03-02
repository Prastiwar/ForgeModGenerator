namespace ForgeModGenerator.CodeGeneration
{
    public struct ClassLocator
    {
        public ClassLocator(string importFullName)
        {
            ImportFullName = importFullName;
            RelativePath = importFullName.Replace('.', '/') + ".java";
            int lastDotIndex = importFullName.LastIndexOf('.');
            ClassName = lastDotIndex != -1
                ? importFullName.Substring(lastDotIndex + 1, importFullName.Length - lastDotIndex - 1)
                : importFullName;
        }

        /// <summary> Class and file Name </summary>
        public string ClassName { get; }

        /// <summary> Relative path to .../com/organization/projectname. </summary>
        public string RelativePath { get; }

        /// <summary> Full relative import path (com.organization.projectname.RelativePath.ClassName) </summary>
        public string ImportFullName { get; }

        public static implicit operator ClassLocator(string importFullName) => new ClassLocator(importFullName);
        public static string CombineImport(params string[] strings) => string.Join(".", strings);
    }

    public static class SourceCodeFolders
    {
        public const string Root = "generated";

        public const string Block = "block";

        public const string Item = "item";
        public const string Armor = "armor";
        public const string Bow = "bow";
        public const string Food = "food";
        public const string Tool = "tool";

        public const string Handler = "handler";
        public const string Proxy = "proxy";
        public const string Sound = "sound";
        public const string Gui = "gui";
    }

    public static class SourceCodeLocator
    {
        private const string Prefix = "FMG";

        public static readonly ClassLocator Manager = ClassLocator.CombineImport(SourceCodeFolders.Root, Prefix + "Manager");
        public static readonly ClassLocator Hook = ClassLocator.CombineImport(SourceCodeFolders.Root, Prefix + "Hook");

        public static readonly ClassLocator Items = ClassLocator.CombineImport(SourceCodeFolders.Root, Prefix + "Items");
        public static readonly ClassLocator ItemBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Item, "ItemBase");
        public static readonly ClassLocator BowBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Bow, "BowBase");
        public static readonly ClassLocator FoodBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Food, "FoodBase");
        public static readonly ClassLocator FoodEffectBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Food, "FoodEffectBase");
        public static readonly ClassLocator ArmorBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Armor, "ArmorBase");
        public static readonly ClassLocator SwordBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Tool, "SwordBase");
        public static readonly ClassLocator SpadeBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Tool, "SpadeBase");
        public static readonly ClassLocator PickaxeBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Tool, "PickaxeBase");
        public static readonly ClassLocator HoeBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Tool, "HoeBase");
        public static readonly ClassLocator AxeBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Item, SourceCodeFolders.Tool, "AxeBase");

        public static readonly ClassLocator Blocks = ClassLocator.CombineImport(SourceCodeFolders.Root, Prefix + "Blocks");
        public static readonly ClassLocator BlockBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Block, "BlockBase");
        public static readonly ClassLocator OreBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Block, "OreBase");

        public static readonly ClassLocator Sounds = ClassLocator.CombineImport(SourceCodeFolders.Root, Prefix + "Sounds");
        public static readonly ClassLocator SoundEventBase = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Sound, "SoundEventBase");

        public static readonly ClassLocator Recipes = ClassLocator.CombineImport(SourceCodeFolders.Root, Prefix + "Recipes");

        public static readonly ClassLocator CreativeTab = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Gui, Prefix + "CreativeTab");

        public static readonly ClassLocator CommonProxyInterface = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Proxy, "ICommonProxy");
        public static readonly ClassLocator ServerProxy = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Proxy, "ServerProxy");
        public static readonly ClassLocator ClientProxy = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Proxy, "ClientProxy");

        public static readonly ClassLocator ModelInterface = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Handler, "IHasModel");
        public static readonly ClassLocator RegistryHandler = ClassLocator.CombineImport(SourceCodeFolders.Root, SourceCodeFolders.Handler, "RegistryHandler");
    }
}
