using System;
using System.Collections.Generic;

namespace ForgeModGenerator.CodeGeneration
{
    public struct Parameter : IEquatable<Parameter>
    {
        public string TypeName { get; }
        public string Name { get; }

        public Parameter(string type, string name)
        {
            TypeName = type;
            Name = name;
        }

        public override bool Equals(object obj) => obj is Parameter parameter && Equals(parameter);
        public bool Equals(Parameter other) => TypeName == other.TypeName && Name == other.Name;

        public override int GetHashCode()
        {
            int hashCode = -181803900;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TypeName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }

        public static bool operator ==(Parameter left, Parameter right) => left.Equals(right);
        public static bool operator !=(Parameter left, Parameter right) => !(left == right);
    }
}
