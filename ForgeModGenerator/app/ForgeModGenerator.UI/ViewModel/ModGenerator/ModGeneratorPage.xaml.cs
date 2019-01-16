using System.Windows.Controls;
using System.Windows.Media;

namespace ForgeModGenerator.ViewModel
{
    public class TextBoxValidVisual
    {
        private readonly Brush normalBackground;
        private readonly TextBox box;

        public TextBoxValidVisual(TextBox box)
        {
            this.box = box;
            normalBackground = box.Background;
        }

        public void SetValid(bool valid)
        {
            if (!valid)
            {
                if (box.Background != Brushes.IndianRed)
                {
                    box.Background = Brushes.DarkRed;
                }
            }
            else
            {
                if (box.Background != normalBackground)
                {
                    box.Background = normalBackground;
                }
            }
        }
    }

    /// <summary> ModGenerator UI View-ViewModel </summary>
    public partial class ModGeneratorPage : Page
    {
        private TextBoxValidVisual modidValidVisual;
        protected TextBoxValidVisual ModidValidVisual => modidValidVisual ?? (modidValidVisual = new TextBoxValidVisual(ModidBox));

        private TextBoxValidVisual organizationdValidVisual;
        protected TextBoxValidVisual OrganizationdValidVisual => organizationdValidVisual ?? (organizationdValidVisual = new TextBoxValidVisual(OrganizationBox));

        private TextBoxValidVisual nameValidVisual;
        protected TextBoxValidVisual NameValidVisual => nameValidVisual ?? (nameValidVisual = new TextBoxValidVisual(NameBox));

        protected ModGeneratorViewModel ViewModel => DataContext as ModGeneratorViewModel;

        public ModGeneratorPage()
        {
            InitializeComponent();
        }

        private void ValidateModid(object sender, TextChangedEventArgs e)
        {
            TextBox text = sender as TextBox;
            text.Text = text.Text.ToLower();
            ModidValidVisual.SetValid(ViewModel.Validator.IsValidModid(text.Text));
        }

        private void ValidateOrganization(object sender, TextChangedEventArgs e)
        {
            TextBox text = sender as TextBox;
            text.Text = text.Text.ToLower();
            OrganizationdValidVisual.SetValid(ViewModel.Validator.IsValidOrganization(text.Text));
        }

        private void ValidateNameAndTryFillModif(object sender, TextChangedEventArgs e)
        {
            TextBox text = sender as TextBox;
            NameValidVisual.SetValid(ViewModel.Validator.IsValidName(text.Text));
            if (ModidBox != null)
            {
                if (string.IsNullOrWhiteSpace(ModidBox.Text) || string.Compare(ModidBox.Text, text.Text, true) < 0)
                {
                    ModidBox.Text = text.Text;
                }
            }
        }
    }
}
