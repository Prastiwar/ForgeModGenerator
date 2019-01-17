using ForgeModGenerator.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.Controls
{
    public partial class TextureList : UserControl
    {
        public TextureList()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(TextureList), new PropertyMetadata("Images:"));
        public string HeaderText {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly DependencyProperty TexturesProperty =
            DependencyProperty.Register("Textures", typeof(TextureCollection), typeof(TextureList), new PropertyMetadata(default(TextureCollection)));
        public TextureCollection Textures {
            get => (TextureCollection)GetValue(TexturesProperty);
            set => SetValue(TexturesProperty, value);
        }

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(TextureList), new PropertyMetadata(null));
        public ICommand AddCommand {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(TextureList), new PropertyMetadata(null));
        public ICommand RemoveCommand {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        private void ShowContainer(object sender, RoutedEventArgs e) => System.Diagnostics.Process.Start(Textures.DestinationPath);

    }
}
