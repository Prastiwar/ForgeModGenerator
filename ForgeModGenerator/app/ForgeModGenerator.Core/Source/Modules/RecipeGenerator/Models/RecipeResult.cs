namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class RecipeResult
    {
        private int count = 1;
        public int Count {
            get => count;
            set => count = Math.Clamp(value, 1, int.MaxValue);
        }

        public string Item { get; set; }
    }
}
