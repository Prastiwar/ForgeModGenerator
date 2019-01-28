﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.UserControls
{
    public partial class FileCollectionListBox : UserControl
    {
        public FileCollectionListBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FileItemTemplateProperty =
            DependencyProperty.Register("FileItemTemplate", typeof(DataTemplate), typeof(FileCollectionListBox), new PropertyMetadata(null));
        public DataTemplate FileItemTemplate {
            get => (DataTemplate)GetValue(FileItemTemplateProperty);
            set => SetValue(FileItemTemplateProperty, value);
        }

        public static readonly DependencyProperty FilesSourceProperty =
            DependencyProperty.Register("FilesSource", typeof(object), typeof(FileCollectionListBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public object FilesSource {
            get => GetValue(FilesSourceProperty);
            set => SetValue(FilesSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedFilesItemProperty =
            DependencyProperty.Register("SelectedFilesItem", typeof(object), typeof(FileCollectionListBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public object SelectedFilesItem {
            get => GetValue(SelectedFilesItemProperty);
            set => SetValue(SelectedFilesItemProperty, value);
        }

        public static readonly DependencyProperty EmptyMessageProperty =
            DependencyProperty.Register("EmptyMessage", typeof(string), typeof(FileCollectionListBox), new PropertyMetadata("Select mod"));
        public string EmptyMessage {
            get => (string)GetValue(EmptyMessageProperty);
            set => SetValue(EmptyMessageProperty, value);
        }

        public static readonly DependencyProperty ShowEmptyMessageProperty =
            DependencyProperty.Register("ShowEmptyMessage", typeof(bool), typeof(FileCollectionListBox), new PropertyMetadata(true));
        public bool ShowEmptyMessage {
            get => (bool)GetValue(ShowEmptyMessageProperty);
            set => SetValue(ShowEmptyMessageProperty, value);
        }

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(FileCollectionListBox), new PropertyMetadata(null));
        public ICommand AddCommand {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(FileCollectionListBox), new PropertyMetadata(null));
        public ICommand RemoveCommand {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(FileCollectionListBox), new PropertyMetadata(null));
        public ICommand EditCommand {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }
    }
}
