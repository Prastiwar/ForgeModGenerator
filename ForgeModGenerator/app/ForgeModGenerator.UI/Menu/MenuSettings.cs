using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator
{
    public struct GridLengthVector
    {
        public GridLength X;
        public GridLength Y;

        public GridLengthVector(GridLength x, GridLength y)
        {
            X = x;
            Y = y;
        }
    }

    public struct MenuSettings
    {
        public Grid Grid;
        public int Column;
        public int Row;
        public GridLengthVector InitialPosition;
        public GridLengthVector TargetPosition;
        public Duration Duration;

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
    }
}
