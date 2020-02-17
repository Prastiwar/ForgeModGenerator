namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class Ingredient : ICopiable
    {
        public Ingredient() { }
        public Ingredient(string item, string tag)
        {
            Item = item;
            Tag = tag;
        }

        public string Item { get; set; }
        public string Tag { get; set; }

        public object Clone() => MemberwiseClone();
        public virtual object DeepClone() => new Ingredient(Item, Tag);

        public virtual bool CopyValues(object fromCopy)
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
