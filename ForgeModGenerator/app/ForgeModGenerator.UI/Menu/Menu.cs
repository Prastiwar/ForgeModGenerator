using System.Windows.Controls;

namespace ForgeModGenerator
{
    public class Menu
    {
        public bool IsActive { get; set; }

        protected MenuSettings settings;

        private readonly bool shouldChangeColumn;
        private readonly bool shouldChangeRow;
        private readonly GridLengthAnimation columnAnim;
        private readonly GridLengthAnimation rowAnim;
        private readonly GridLengthAnimation columnAnimBack;
        private readonly GridLengthAnimation rowAnimBack;

        private Grid grid;

        public Menu(Grid grid, MenuSettings settings)
        {
            this.grid = grid;
            this.settings = settings;
            shouldChangeColumn = settings.Column > -1;
            shouldChangeRow = settings.Row > -1;

            if (shouldChangeColumn)
            {
                columnAnim = new GridLengthAnimation(settings.InitialPosition.X, settings.TargetPosition.X, settings.Duration);
                columnAnimBack = new GridLengthAnimation(settings.TargetPosition.X, settings.InitialPosition.X, settings.Duration);
            }

            if (shouldChangeRow)
            {
                rowAnim = new GridLengthAnimation(settings.InitialPosition.Y, settings.TargetPosition.Y, settings.Duration);
                rowAnimBack = new GridLengthAnimation(settings.TargetPosition.Y, settings.InitialPosition.Y, settings.Duration);
            }
        }

        public void Toggle()
        {
            IsActive = !IsActive;
            SetFold(IsActive);
        }

        public void SetFold(bool fold)
        {
            if (fold)
            {
                Fold();
            }
            else
            {
                UnFold();
            }
        }

        public void Fold()
        {
            if (shouldChangeColumn)
            {
                grid.ColumnDefinitions[settings.Column].BeginAnimation(ColumnDefinition.WidthProperty, columnAnim);
            }
            if (shouldChangeRow)
            {
                grid.RowDefinitions[settings.Row].BeginAnimation(RowDefinition.HeightProperty, rowAnim);
            }
        }

        public void UnFold()
        {
            if (shouldChangeColumn)
            {
                grid.ColumnDefinitions[settings.Column].BeginAnimation(ColumnDefinition.WidthProperty, columnAnimBack);
            }
            if (shouldChangeRow)
            {
                grid.RowDefinitions[settings.Row].BeginAnimation(RowDefinition.HeightProperty, rowAnimBack);
            }
        }
    }
}
