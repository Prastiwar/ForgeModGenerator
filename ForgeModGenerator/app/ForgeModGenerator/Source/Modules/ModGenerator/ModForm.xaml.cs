﻿using ForgeModGenerator.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.ModGenerator.Controls
{
    public partial class ModForm : UserControl
    {
        public ModForm() => InitializeComponent();

        public static readonly DependencyProperty SidesProperty =
            DependencyProperty.Register("Sides", typeof(IEnumerable<ModSide>), typeof(ModForm), new PropertyMetadata(null));
        public IEnumerable<ModSide> Sides {
            get => (IEnumerable<ModSide>)GetValue(SidesProperty);
            set => SetValue(SidesProperty, value);
        }

        public static readonly DependencyProperty SetupsProperty =
            DependencyProperty.Register("Setups", typeof(ObservableCollection<WorkspaceSetup>), typeof(ModForm), new PropertyMetadata(null));
        public ObservableCollection<WorkspaceSetup> Setups {
            get => (ObservableCollection<WorkspaceSetup>)GetValue(SetupsProperty);
            set => SetValue(SetupsProperty, value);
        }

        public static readonly DependencyProperty ForgeVersionsProperty =
            DependencyProperty.Register("ForgeVersions", typeof(ObservableCollection<ForgeVersion>), typeof(ModForm), new PropertyMetadata(null));
        public ObservableCollection<ForgeVersion> ForgeVersions {
            get => (ObservableCollection<ForgeVersion>)GetValue(ForgeVersionsProperty);
            set => SetValue(ForgeVersionsProperty, value);
        }

        public static readonly DependencyProperty AddForgeVersionCommandProperty =
            DependencyProperty.Register("AddForgeVersionCommand", typeof(ICommand), typeof(ModForm), new PropertyMetadata(null));
        public ICommand AddForgeVersionCommand {
            get => (ICommand)GetValue(AddForgeVersionCommandProperty);
            set => SetValue(AddForgeVersionCommandProperty, value);
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

        private void ValidateNameAndTryFillModif(object sender, TextChangedEventArgs e)
        {
            TextBox text = sender as TextBox;
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