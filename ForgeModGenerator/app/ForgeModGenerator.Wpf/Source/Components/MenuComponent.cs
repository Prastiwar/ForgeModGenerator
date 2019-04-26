using ForgeModGenerator.Animations;
using Prism.Mvvm;
using System.Windows.Controls;

namespace ForgeModGenerator.Components
{
    public class MenuComponent : BindableBase
    {
        public MenuComponent(Grid grid, MenuSettings settings)
        {
            this.grid = grid;
            Settings = settings;
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

        private readonly bool shouldChangeColumn;
        private readonly bool shouldChangeRow;
        private readonly GridLengthAnimation columnAnim;
        private readonly GridLengthAnimation rowAnim;
        private readonly GridLengthAnimation columnAnimBack;
        private readonly GridLengthAnimation rowAnimBack;

        private readonly Grid grid;

        protected MenuSettings Settings { get; set; }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value);
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
                grid.ColumnDefinitions[Settings.Column].BeginAnimation(ColumnDefinition.WidthProperty, columnAnim);
            }
            if (shouldChangeRow)
            {
                grid.RowDefinitions[Settings.Row].BeginAnimation(RowDefinition.HeightProperty, rowAnim);
            }
        }

        public void UnFold()
        {
            if (shouldChangeColumn)
            {
                grid.ColumnDefinitions[Settings.Column].BeginAnimation(ColumnDefinition.WidthProperty, columnAnimBack);
            }
            if (shouldChangeRow)
            {
                grid.RowDefinitions[Settings.Row].BeginAnimation(RowDefinition.HeightProperty, rowAnimBack);
            }
        }
    }
}
