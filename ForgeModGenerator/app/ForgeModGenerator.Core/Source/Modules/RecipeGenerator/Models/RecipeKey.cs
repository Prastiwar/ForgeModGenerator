using System;
using System.Collections.Generic;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public struct RecipeKey : IEquatable<RecipeKey>
    {
        public char Key { get; set; }
        public int Data { get; set; }
        public string Type { get; set; }
        public string Ore { get; set; }
        public string Item { get; set; }

        public override bool Equals(object obj) => obj is RecipeKey key && Equals(key);
        public bool Equals(RecipeKey other) => Key == other.Key && Data == other.Data && Type == other.Type && Ore == other.Ore && Item == other.Item;

        public override int GetHashCode()
        {
            int hashCode = 1317068616;
            hashCode = hashCode * -1521134295 + Key.GetHashCode();
            hashCode = hashCode * -1521134295 + Data.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Ore);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Item);
            return hashCode;
        }

        public static bool operator ==(RecipeKey left, RecipeKey right) => left.Equals(right);
        public static bool operator !=(RecipeKey left, RecipeKey right) => !(left == right);
    }
}
