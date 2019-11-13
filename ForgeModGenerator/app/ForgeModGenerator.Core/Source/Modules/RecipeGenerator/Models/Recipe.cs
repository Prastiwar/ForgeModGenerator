using System;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class Recipe : FileObject
    {
        protected Recipe() { }
        public Recipe(string filePath) : base(filePath) { }

        public virtual string Type { get; } = "";

        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string group;
        public string Group {
            get => group;
            set => SetProperty(ref group, value);
        }

        public override object DeepClone()
        {
            Recipe recipe = (Recipe)Activator.CreateInstance(GetType(), true);
            recipe.CopyValues(this);
            return recipe;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Recipe recipe)
            {
                Name = recipe.Name;
                Group = recipe.Group;
                base.CopyValues(fromCopy);
                IsDirty = false;
                return true;
            }
            return false;
        }
    }
}
