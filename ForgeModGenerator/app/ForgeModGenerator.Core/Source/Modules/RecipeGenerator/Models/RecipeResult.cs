using System;
using System.Collections.Generic;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public struct RecipeResult : IEquatable<RecipeResult>
    {
        public int Data { get; set; }
        public int Count { get; set; }
        public string Item { get; set; }

        public override bool Equals(object obj) => obj is RecipeResult result && Equals(result);
        public bool Equals(RecipeResult other) => Data == other.Data && Count == other.Count && Item == other.Item;

        public override int GetHashCode()
        {
            int hashCode = -1050907569;
            hashCode = hashCode * -1521134295 + Data.GetHashCode();
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Item);
            return hashCode;
        }

        public static bool operator ==(RecipeResult left, RecipeResult right) => left.Equals(right);
        public static bool operator !=(RecipeResult left, RecipeResult right) => !(left == right);
    }
}
