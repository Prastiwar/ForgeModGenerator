﻿using Prism.Mvvm;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public enum RecipeType
    {
        crafting_shaped
    }

    public class Recipe : BindableBase
    {
        private RecipeType type;
        public RecipeType Type {
            get => type;
            set => SetProperty(ref type, value);
        }

        private char[] pattern;
        public char[] Pattern {
            get => pattern;
            set => SetProperty(ref pattern, value);
        }

        private RecipeKey[] keys;
        public RecipeKey[] Keys {
            get => keys;
            set => SetProperty(ref keys, value);
        }

        private RecipeResult result;
        public RecipeResult Result {
            get => result;
            set => SetProperty(ref result, value);
        }

    }
}
