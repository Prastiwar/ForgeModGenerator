using GalaSoft.MvvmLight;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public enum RecipeType
    {
        crafting_shaped
    }

    public struct RecipeResult
    {
        public int Data;
        public int Count;
        public string Item;
    }

    public struct RecipeKey
    {
        public char Char;
        public int Data;
        public string Type;
        public string Ore;
        public string Item;
    }

    public class Recipe : ObservableObject
    {
        private RecipeType type;
        public RecipeType Type {
            get => type;
            set => Set(ref type, value);
        }

        private char[] pattern;
        public char[] Pattern {
            get => pattern;
            set => Set(ref pattern, value);
        }

        private RecipeKey[] keys;
        public RecipeKey[] Keys {
            get => keys;
            set => Set(ref keys, value);
        }

        private RecipeResult result;
        public RecipeResult Result {
            get => result;
            set => Set(ref result, value);
        }

    }
}
