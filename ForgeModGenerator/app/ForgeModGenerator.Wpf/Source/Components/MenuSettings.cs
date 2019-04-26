using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.Components
{
    public struct MenuSettings : System.IEquatable<MenuSettings>
    {
        public Grid Grid { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public GridLengthVector InitialPosition { get; set; }
        public GridLengthVector TargetPosition { get; set; }
        public Duration Duration { get; set; }

        public MenuSettings(Grid grid, int column, int row, Vector offsetPosition, double secondsDuration = 1.0)
        {
            Grid = grid;
            Duration = new Duration(System.TimeSpan.FromSeconds(secondsDuration));

            Column = column > -1 && grid.ColumnDefinitions.Count > column
                ? column
                : -1;

            Row = row > -1 && grid.RowDefinitions.Count > row
                ? row
                : -1;

            InitialPosition = new GridLengthVector(
                Column > -1 ? grid.ColumnDefinitions[column].Width : GridLength.Auto,
                Row > -1 ? grid.RowDefinitions[row].Height : GridLength.Auto
            );

            TargetPosition = new GridLengthVector(
                new GridLength(InitialPosition.X.Value + offsetPosition.X, InitialPosition.X.GridUnitType),
                new GridLength(InitialPosition.Y.Value + offsetPosition.Y, InitialPosition.Y.GridUnitType)
            );
        }

        public override bool Equals(object obj)
        {
            if (obj is MenuSettings s)
            {
                return s.Column == Column &&
                    s.Duration == Duration &&
                    s.Grid == Grid &&
                    s.InitialPosition == InitialPosition &&
                    s.Row == Row &&
                    s.TargetPosition == TargetPosition;
            }
            return false;
        }

        public bool Equals(MenuSettings other) => other.Column == Column &&
                                                 other.Duration == Duration &&
                                                 other.Grid == Grid &&
                                                 other.InitialPosition == InitialPosition &&
                                                 other.Row == Row &&
                                                 other.TargetPosition == TargetPosition;

        public override int GetHashCode()
        {
            int hashCode = 1154872691;
            hashCode = hashCode * -1521134295 + EqualityComparer<Grid>.Default.GetHashCode(Grid);
            hashCode = hashCode * -1521134295 + Column.GetHashCode();
            hashCode = hashCode * -1521134295 + Row.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<GridLengthVector>.Default.GetHashCode(InitialPosition);
            hashCode = hashCode * -1521134295 + EqualityComparer<GridLengthVector>.Default.GetHashCode(TargetPosition);
            hashCode = hashCode * -1521134295 + EqualityComparer<Duration>.Default.GetHashCode(Duration);
            return hashCode;
        }

        public static bool operator ==(MenuSettings left, MenuSettings right) => left.Equals(right);
        public static bool operator !=(MenuSettings left, MenuSettings right) => !(left == right);
    }
}
