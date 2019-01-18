using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.Controls
{
    public partial class ModForm : UserControl
    {
        public ModForm()
        {
            InitializeComponent();
        }

        private TextBoxValidVisual modidValidVisual;
        protected TextBoxValidVisual ModidValidVisual => modidValidVisual ?? (modidValidVisual = new TextBoxValidVisual(ModidBox));

        private TextBoxValidVisual organizationdValidVisual;
        protected TextBoxValidVisual OrganizationdValidVisual => organizationdValidVisual ?? (organizationdValidVisual = new TextBoxValidVisual(OrganizationBox));

        private TextBoxValidVisual nameValidVisual;
        protected TextBoxValidVisual NameValidVisual => nameValidVisual ?? (nameValidVisual = new TextBoxValidVisual(NameBox));

        public static readonly DependencyProperty ValidatorProperty =
            DependencyProperty.Register("Validator", typeof(ModValidationService), typeof(ModForm), new PropertyMetadata(null));
        public ModValidationService Validator {
            get { return (ModValidationService)GetValue(ValidatorProperty); }
            set { SetValue(ValidatorProperty, value); }
        }

        public static readonly DependencyProperty SidesProperty =
            DependencyProperty.Register("Sides", typeof(IEnumerable<ModSide>), typeof(ModForm), new PropertyMetadata(null));
        public IEnumerable<ModSide> Sides {
            get { return (IEnumerable<ModSide>)GetValue(SidesProperty); }
            set { SetValue(SidesProperty, value); }
        }

        public static readonly DependencyProperty SetupsProperty =
            DependencyProperty.Register("Setups", typeof(ObservableCollection<WorkspaceSetup>), typeof(ModForm), new PropertyMetadata(null));
        public ObservableCollection<WorkspaceSetup> Setups {
            get { return (ObservableCollection<WorkspaceSetup>)GetValue(SetupsProperty); }
            set { SetValue(SetupsProperty, value); }
        }

        public static readonly DependencyProperty ForgeVersionsProperty =
            DependencyProperty.Register("ForgeVersions", typeof(ObservableCollection<ForgeVersion>), typeof(ModForm), new PropertyMetadata(null));
        public ObservableCollection<ForgeVersion> ForgeVersions {
            get { return (ObservableCollection<ForgeVersion>)GetValue(ForgeVersionsProperty); }
            set { SetValue(ForgeVersionsProperty, value); }
        }

        public static readonly DependencyProperty AddForgeVersionCommandProperty =
            DependencyProperty.Register("AddForgeVersionCommand", typeof(ICommand), typeof(ModForm), new PropertyMetadata(null));
        public ICommand AddForgeVersionCommand {
            get => (ICommand)GetValue(AddForgeVersionCommandProperty);
            set => SetValue(AddForgeVersionCommandProperty, value);
        }

        public static readonly DependencyProperty AddStringItemCommandProperty =
            DependencyProperty.Register("AddStringItemCommand", typeof(ICommand), typeof(ModForm), new PropertyMetadata(null));
        public ICommand AddStringItemCommand {
            get => (ICommand)GetValue(AddStringItemCommandProperty);
            set => SetValue(AddStringItemCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveStringItemCommandProperty =
            DependencyProperty.Register("RemoveStringItemCommand", typeof(ICommand), typeof(ModForm), new PropertyMetadata(null));
        public ICommand RemoveStringItemCommand {
            get => (ICommand)GetValue(RemoveStringItemCommandProperty);
            set => SetValue(RemoveStringItemCommandProperty, value);
        }

        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(ModForm), new PropertyMetadata(null));
        public ICommand SaveCommand {
            get => (ICommand)GetValue(SaveCommandProperty);
            set => SetValue(SaveCommandProperty, value);
        }

        public static readonly DependencyProperty SaveButtonTextProperty =
            DependencyProperty.Register("SaveButtonText", typeof(string), typeof(ModForm), new PropertyMetadata("Save Changes"));
        public string SaveButtonText {
            get => (string)GetValue(SaveButtonTextProperty);
            set => SetValue(SaveButtonTextProperty, value);
        }

        private void ValidateModid(object sender, TextChangedEventArgs e)
        {
            TextBox text = sender as TextBox;
            text.Text = text.Text.ToLower();
            ModidValidVisual.SetValid(Validator.IsValidModid(text.Text));
        }

        private void ValidateOrganization(object sender, TextChangedEventArgs e)
        {
            TextBox text = sender as TextBox;
            text.Text = text.Text.ToLower();
            OrganizationdValidVisual.SetValid(Validator.IsValidOrganization(text.Text));
        }

        private void ValidateNameAndTryFillModif(object sender, TextChangedEventArgs e)
        {
            TextBox text = sender as TextBox;
            NameValidVisual.SetValid(Validator.IsValidName(text.Text));
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
