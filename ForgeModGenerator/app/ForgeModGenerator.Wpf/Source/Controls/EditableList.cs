using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.Controls
{
    public class EditableStringList : EditableList<string>
    {
        protected override string DefaultItem => "New Item";
    }

    public class EditableList<T> : ContentControl
    {
        static EditableList() => DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableList<T>), new FrameworkPropertyMetadata(typeof(EditableList<T>)));

        public EditableList()
        {
            if (AddCommand == null)
            {
                AddCommand = new DelegateCommand<ObservableCollection<T>>(DefaultAdd);
            }
            if (RemoveCommand == null)
            {
                RemoveCommand = new DelegateCommand<Tuple<ObservableCollection<T>, T>>(DefaultRemove);
            }
        }

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(EditableList<T>), new PropertyMetadata(null, AddCommandChanged));

        private static void AddCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                EditableList<T> list = (EditableList<T>)d;
                list.AddCommand = new DelegateCommand<ObservableCollection<T>>(list.DefaultAdd);
            }
        }

        public ICommand AddCommand {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(EditableList<T>), new PropertyMetadata(null, RemoveCommandChanged));

        private static void RemoveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                EditableList<T> list = (EditableList<T>)d;
                list.RemoveCommand = new DelegateCommand<Tuple<ObservableCollection<T>, T>>(list.DefaultRemove);
            }
        }
        public ICommand RemoveCommand {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<T>), typeof(EditableList<T>), new PropertyMetadata(null));
        public ObservableCollection<T> ItemsSource {
            get => (ObservableCollection<T>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        protected virtual T DefaultItem => default;

        protected void DefaultAdd(ObservableCollection<T> collection) => collection.Add(DefaultItem);
        protected void DefaultRemove(Tuple<ObservableCollection<T>, T> param) => param.Item1.Remove(param.Item2);
    }
}
