using System.Collections.Generic;
using System.Windows;

namespace ForgeModGenerator.Components
{
    public struct GridLengthVector : System.IEquatable<GridLengthVector>
    {
        public GridLength X { get; set; }
        public GridLength Y { get; set; }

        public GridLengthVector(GridLength x, GridLength y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj) => obj is GridLengthVector vector ? Equals(vector) : false;

        public bool Equals(GridLengthVector other) => other.X == X && other.Y == Y;

        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + EqualityComparer<GridLength>.Default.GetHashCode(X);
            hashCode = hashCode * -1521134295 + EqualityComparer<GridLength>.Default.GetHashCode(Y);
            return hashCode;
        }

        public static bool operator ==(GridLengthVector left, GridLengthVector right) => left.Equals(right);
        public static bool operator !=(GridLengthVector left, GridLengthVector right) => !(left == right);
    }
}
