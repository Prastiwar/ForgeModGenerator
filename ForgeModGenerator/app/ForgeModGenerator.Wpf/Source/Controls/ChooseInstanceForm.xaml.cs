using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.Controls
{
    public partial class ChooseInstanceForm : UserControl, IUIChoice
    {
        public ChooseInstanceForm()
        {
            InitializeComponent();
            UpdateGrid();
        }

        public static readonly DependencyProperty TypesProperty =
            DependencyProperty.Register("Types", typeof(IEnumerable<Type>), typeof(ChooseInstanceForm), new PropertyMetadata(Enumerable.Empty<Type>(), OnTypesChanged));
        public IEnumerable<Type> Types {
            get => (IEnumerable<Type>)GetValue(TypesProperty);
            set => SetValue(TypesProperty, value);
        }
        private static void OnTypesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChooseInstanceForm form = (ChooseInstanceForm)d;
            form.UpdateGrid();
        }

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(ChooseInstanceForm), new PropertyMetadata(null));
        public object SelectedValue {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public static readonly DependencyProperty MaxColumnAmountProperty =
            DependencyProperty.Register("MaxColumnAmount", typeof(int), typeof(ChooseInstanceForm), new PropertyMetadata(-1));
        public int MaxColumnAmount {
            get => (int)GetValue(MaxColumnAmountProperty);
            set => SetValue(MaxColumnAmountProperty, value);
        }

        public static readonly DependencyProperty MaxRowAmountProperty =
            DependencyProperty.Register("MaxRowAmount", typeof(int), typeof(ChooseInstanceForm), new PropertyMetadata(-1));
        public int MaxRowAmount {
            get => (int)GetValue(MaxRowAmountProperty);
            set => SetValue(MaxRowAmountProperty, value);
        }

        private void UpdateGrid()
        {
            GetGridCellCounts(out int columnsCount, out int rowsCount);
            UpdateGridDefinitions(columnsCount, rowsCount);
            UpdateGridButtons(columnsCount, rowsCount);
        }

        private void GetGridCellCounts(out int columnsCount, out int rowsCount)
        {
            int buttonsCount = Types.Count();
            columnsCount = IsInfinity(MaxColumnAmount) ? buttonsCount :
                buttonsCount >= MaxColumnAmount
                ? MaxColumnAmount
                : buttonsCount;
            columnsCount = columnsCount <= 0 ? 1 : columnsCount;

            buttonsCount -= columnsCount; // remaining buttons
            rowsCount = IsInfinity(MaxRowAmount) ? buttonsCount :
                buttonsCount >= MaxRowAmount
                ? MaxRowAmount
                : buttonsCount;
            rowsCount = rowsCount <= 0 ? 1 : rowsCount;
        }

        private bool IsInfinity(int amount) => amount <= -1 || amount >= int.MaxValue;

        private void UpdateGridDefinitions(int columnsCount, int rowsCount)
        {
            ButtonsStackPanel.ColumnDefinitions.Clear();
            for (int i = 0; i < columnsCount; i++)
            {
                ButtonsStackPanel.ColumnDefinitions.Add(new ColumnDefinition());
            }

            ButtonsStackPanel.RowDefinitions.Clear();
            for (int i = 0; i < rowsCount; i++)
            {
                ButtonsStackPanel.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void UpdateGridButtons(int columnsCount, int rowsCount)
        {
            int index = 0;
            int columnPos = 0;
            int rowPos = 0;
            ButtonsStackPanel.Children.Clear();
            foreach (Type type in Types)
            {
                Button button = CreateButton(type);
                Grid.SetColumn(button, columnPos);
                Grid.SetRow(button, rowPos);
                if (columnPos < columnsCount)
                {
                    columnPos++;
                }
                else
                {
                    columnPos = 0;
                    if (rowPos < rowsCount)
                    {
                        rowPos++;
                    }
                }
                ButtonsStackPanel.Children.Add(button);
                index++;
            }
        }

        protected virtual Button CreateButton(Type type)
        {
            Button button = new Button {
                Tag = type,
                Content = type.Name,
                MinHeight = 128,
                MinWidth = 128,
                Margin = new Thickness(10)
            };
            button.Click += (s, e) => Choose_Click(type);
            return button;
        }

        public void SetDataContext(object context)
        {
            DataContext = context;
            UpdateGrid();
        }

        private void Choose_Click(Type type)
        {
            SelectedValue = Activator.CreateInstance(type);
            DialogHost.CloseDialogCommand.Execute(true, null);
        }
    }
}
