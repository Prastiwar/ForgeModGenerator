using ForgeModGenerator.Utility;
using System;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class RecipeCreator : Recipe
    {
        protected RecipeCreator() => Initialize();
        public RecipeCreator(string filePath) : base(filePath) => Initialize();

        private void Initialize()
        {
            RecipeType = typeof(ShapedRecipe);
            Keys = new ObservableCollection<RecipeKey>();
            Ingredients = new ObservableCollection<Ingredient>();
            Result = new RecipeResult();
        }

        private Type recipeType;
        public Type RecipeType {
            get => recipeType;
            set => SetProperty(ref recipeType, value);
        }

        public char[] Pattern { get; } = new char[9];

        private ObservableCollection<RecipeKey> keys;
        public ObservableCollection<RecipeKey> Keys {
            get => keys;
            protected set => SetProperty(ref keys, value);
        }

        private ObservableCollection<Ingredient> ingredients;
        public ObservableCollection<Ingredient> Ingredients {
            get => ingredients;
            protected set => SetProperty(ref ingredients, value);
        }

        private RecipeResult result;
        public RecipeResult Result {
            get => result;
            protected set => SetProperty(ref result, value);
        }

        private int cookingTime;
        public int CookingTime {
            get => cookingTime;
            set => SetProperty(ref cookingTime, value);
        }

        private float experience;
        public float Experience {
            get => experience;
            set => SetProperty(ref experience, value);
        }

        public T Create<T>() where T : Recipe => Create(typeof(T)) as T;

        public Recipe Create() => Create(RecipeType);

        public Recipe Create(Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (type.IsAssignableFrom(typeof(ShapedRecipe)))
            {
                ShapedRecipe recipe = new ShapedRecipe(Info.FullName) {
                    Name = Name,
                    Group = Group,
                    Keys = Keys.DeepCollectionClone<ObservableCollection<RecipeKey>, RecipeKey>(),
                    Result = new RecipeResult {
                        Count = Result.Count,
                        Item = Result.Item
                    },
                    IsDirty = false
                };
                Array.Copy(recipe.Pattern, Pattern, Pattern.Length);
                return recipe;
            }
            else if (type.IsAssignableFrom(typeof(ShapelessRecipe)))
            {
                ShapelessRecipe recipe = new ShapelessRecipe(Info.FullName) {
                    Name = Name,
                    Group = Group,
                    Result = new RecipeResult {
                        Count = Result.Count,
                        Item = Result.Item
                    },
                    Ingredients = Ingredients,
                    IsDirty = false
                };
                return recipe;
            }
            else if (type.IsAssignableFrom(typeof(SmeltingRecipe)))
            {
                SmeltingRecipe recipe = new SmeltingRecipe(Info.FullName) {
                    Name = Name,
                    Group = Group,
                    Result = new RecipeResult {
                        Count = Result.Count,
                        Item = Result.Item
                    },
                    CookingTime = CookingTime,
                    Experience = Experience,
                    Ingredients = Ingredients,
                    IsDirty = false
                };
                return recipe;
            }
            else
            {
                return null;
            }
        }
    }
}
