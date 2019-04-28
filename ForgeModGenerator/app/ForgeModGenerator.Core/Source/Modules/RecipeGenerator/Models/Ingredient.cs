namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class Ingredient : ICopiable
    {
        public string Item { get; set; }
        public string Tag { get; set; }

        public object Clone() => MemberwiseClone();
        public object DeepClone() => new Ingredient { Item = Item, Tag = Tag };

        public bool CopyValues(object fromCopy)
        {
            if (fromCopy is Ingredient ingredient)
            {
                Item = ingredient.Item;
                Tag = ingredient.Tag;
                return true;
            }
            return false;
        }
    }
}
