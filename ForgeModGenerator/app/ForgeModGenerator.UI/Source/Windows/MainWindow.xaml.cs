using System;
using System.Windows;

namespace ForgeModGenerator.UI.Windows
{
    public partial class MainWindow : Window
    {
        public static Window ActiveWindow { get; protected set; }

        public MainWindow()
        {
            InitializeComponent();
            GenMenu.InitializeMenu(ContentGrid, 0, 0);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ActiveWindow = this;
        }
    }
}
