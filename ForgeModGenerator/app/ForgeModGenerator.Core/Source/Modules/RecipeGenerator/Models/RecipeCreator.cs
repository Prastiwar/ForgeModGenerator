using ForgeModGenerator.Utility;
using System;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class RecipeCreator : Recipe
    {
        public RecipeCreator() => Initialize();
        public RecipeCreator(Recipe recipe)
        {
            Initialize();
            Name = recipe.Name;
            Group = recipe.Group;
            Result = new RecipeResult {
                Count = recipe.Result.Count,
                Item = recipe.Result.Item
            };
            if (recipe is ShapedRecipe shaped)
            {
                Keys = shaped.Keys?.DeepCollectionClone<RecipeKeyCollection, RecipeKey>();
                Pattern.Set(shaped.Pattern);
            }
            else if (recipe is SmeltingRecipe smelting)
            {
                Ingredients = smelting.Ingredients?.DeepCollectionClone<ObservableCollection<Ingredient>, Ingredient>();
                CookingTime = smelting.CookingTime;
                Experience = smelting.Experience;
            }
            else if (recipe is ShapelessRecipe shapeless)
            {
                Ingredients = shapeless.Ingredients?.DeepCollectionClone<ObservableCollection<Ingredient>, Ingredient>();
            }
            RecipeType = recipe.GetType();
            cachedCreatedRecipe = recipe;
            IsDirty = false;
        }

        private void Initialize()
        {
            RecipeType = typeof(ShapedRecipe);
            Keys = new RecipeKeyCollection();
            Ingredients = new ObservableCollection<Ingredient>();
            Result = new RecipeResult();
        }

        private Recipe cachedCreatedRecipe;

        private Type recipeType;
        public Type RecipeType {
            get => recipeType;
            set => SetProperty(ref recipeType, value);
        }

        public ShapedPattern Pattern { get; }

        private RecipeKeyCollection keys;
        public RecipeKeyCollection Keys {
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

        public override object DeepClone()
        {
            RecipeCreator creator = new RecipeCreator {
                Ingredients = Ingredients.DeepCollectionClone<ObservableCollection<Ingredient>, Ingredient>(),
                Keys = Keys.DeepCollectionClone<RecipeKeyCollection, RecipeKey>(),
                Result = new RecipeResult {
                    Count = Result.Count,
                    Item = Result.Item
                }
            };
            creator.Pattern.Set(Pattern);
            return creator;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Recipe)
            {
                if (fromCopy is ShapedRecipe shapedRecipe)
                {
                    Pattern.Set(shapedRecipe.Pattern);
                    Keys = shapedRecipe.Keys;
                }
                else if (fromCopy is SmeltingRecipe smeltingRecipe)
                {
                    Ingredients = smeltingRecipe.Ingredients;
                    CookingTime = smeltingRecipe.CookingTime;
                    Experience = smeltingRecipe.Experience;
                }
                else if (fromCopy is ShapelessRecipe shapelessRecipe)
                {
                    Ingredients = shapelessRecipe.Ingredients;
                }
                else if (fromCopy is RecipeCreator creator)
                {
                    Pattern.Set(creator.Pattern);
                    Keys = creator.Keys;
                    Ingredients = creator.Ingredients;
                    CookingTime = creator.CookingTime;
                    Experience = creator.Experience;
                }
                base.CopyValues(fromCopy);
                return true;
            }
            return false;
        }

        public T Create<T>() where T : Recipe => Create(typeof(T)) as T;

        public Recipe Create() => Create(RecipeType);

        public Recipe Create(Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (!IsDirty && cachedCreatedRecipe != null)
            {
                return cachedCreatedRecipe;
            }
            if (type.IsAssignableFrom(typeof(ShapedRecipe)))
            {
                ShapedRecipe recipe = new ShapedRecipe() {
                    Name = Name,
                    Group = Group,
                    Keys = Keys.DeepCollectionClone<RecipeKeyCollection, RecipeKey>(),
                    Result = new RecipeResult {
                        Count = Result.Count,
                        Item = Result.Item
                    },
                    IsDirty = false
                };
                recipe.Pattern.Set(Pattern);
                cachedCreatedRecipe = recipe;
                return recipe;
            }
            else if (type.IsAssignableFrom(typeof(ShapelessRecipe)))
            {
                ShapelessRecipe recipe = new ShapelessRecipe() {
                    Name = Name,
                    Group = Group,
                    Result = new RecipeResult {
                        Count = Result.Count,
                        Item = Result.Item
                    },
                    Ingredients = Ingredients,
                    IsDirty = false
                };
                cachedCreatedRecipe = recipe;
                return recipe;
            }
            else if (type.IsAssignableFrom(typeof(SmeltingRecipe)))
            {
                SmeltingRecipe recipe = new SmeltingRecipe() {
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
                cachedCreatedRecipe = recipe;
                return recipe;
            }
            throw new NotSupportedException($"Type of {type} is not supported by this creator");
        }
    }
}
