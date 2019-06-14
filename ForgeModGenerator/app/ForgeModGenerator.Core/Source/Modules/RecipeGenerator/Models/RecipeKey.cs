using System;
using System.Collections.Generic;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public struct RecipeKey : IEquatable<RecipeKey>, ICopiable
    {
        public RecipeKey(char key, string item)
        {
            Key = key;
            Item = item;
        }

        public char Key { get; set; }
        public string Item { get; set; }

        public object Clone() => MemberwiseClone();
        public object DeepClone() => new RecipeKey(Key, Item);

        public bool CopyValues(object fromCopy)
        {
            if (fromCopy is RecipeKey recipeKey)
            {
                Key = recipeKey.Key;
                Item = recipeKey.Item;
                return true;
            }
            return false;
        }

        public bool Equals(RecipeKey other) => Key == other.Key && Item == other.Item;
        public override bool Equals(object obj) => obj is RecipeKey key && Equals(key);

        public override int GetHashCode()
        {
            int hashCode = 1517006578;
            hashCode = hashCode * -1521134295 + Key.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Item);
            return hashCode;
        }

        public static bool operator ==(RecipeKey left, RecipeKey right) => left.Equals(right);
        public static bool operator !=(RecipeKey left, RecipeKey right) => !(left == right);
    }
}
