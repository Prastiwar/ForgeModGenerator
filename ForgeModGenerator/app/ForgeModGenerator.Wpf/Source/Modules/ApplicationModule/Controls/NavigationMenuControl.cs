using ForgeModGenerator.Components;
using Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.ApplicationModule.Controls
{
    public class NavigationMenuControl : ContentControl
    {
        static NavigationMenuControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationMenuControl), new FrameworkPropertyMetadata(typeof(NavigationMenuControl)));

        public static readonly DependencyProperty OpenDashboardCommandProperty =
            DependencyProperty.Register("OpenDashboardCommand", typeof(ICommand), typeof(NavigationMenuControl), new PropertyMetadata(null));
        public ICommand OpenDashboardCommand {
            get => (ICommand)GetValue(OpenDashboardCommandProperty);
            set => SetValue(OpenDashboardCommandProperty, value);
        }

        public static readonly DependencyProperty OpenModGeneratorCommandProperty =
            DependencyProperty.Register("OpenModGeneratorCommand", typeof(ICommand), typeof(NavigationMenuControl), new PropertyMetadata(null));
        public ICommand OpenModGeneratorCommand {
            get => (ICommand)GetValue(OpenModGeneratorCommandProperty);
            set => SetValue(OpenModGeneratorCommandProperty, value);
        }

        public static readonly DependencyProperty OpenBuildConfigurationCommandProperty =
            DependencyProperty.Register("OpenBuildConfigurationCommand", typeof(ICommand), typeof(NavigationMenuControl), new PropertyMetadata(null));
        public ICommand OpenBuildConfigurationCommand {
            get => (ICommand)GetValue(OpenBuildConfigurationCommandProperty);
            set => SetValue(OpenBuildConfigurationCommandProperty, value);
        }

        public static readonly DependencyProperty OpenTextureGeneratorCommandProperty =
            DependencyProperty.Register("OpenTextureGeneratorCommand", typeof(ICommand), typeof(NavigationMenuControl), new PropertyMetadata(null));
        public ICommand OpenTextureGeneratorCommand {
            get => (ICommand)GetValue(OpenTextureGeneratorCommandProperty);
            set => SetValue(OpenTextureGeneratorCommandProperty, value);
        }

        public static readonly DependencyProperty OpenBlockGeneratorCommandProperty =
            DependencyProperty.Register("OpenBlockGeneratorCommand", typeof(ICommand), typeof(NavigationMenuControl), new PropertyMetadata(null));
        public ICommand OpenBlockGeneratorCommand {
            get => (ICommand)GetValue(OpenBlockGeneratorCommandProperty);
            set => SetValue(OpenBlockGeneratorCommandProperty, value);
        }

        public static readonly DependencyProperty OpenItemGeneratorCommandProperty =
            DependencyProperty.Register("OpenItemGeneratorCommand", typeof(ICommand), typeof(NavigationMenuControl), new PropertyMetadata(null));
        public ICommand OpenItemGeneratorCommand {
            get => (ICommand)GetValue(OpenItemGeneratorCommandProperty);
            set => SetValue(OpenItemGeneratorCommandProperty, value);
        }

        public static readonly DependencyProperty OpenSoundGeneratorCommandProperty =
            DependencyProperty.Register("OpenSoundGeneratorCommand", typeof(ICommand), typeof(NavigationMenuControl), new PropertyMetadata(null));
        public ICommand OpenSoundGeneratorCommand {
            get => (ICommand)GetValue(OpenSoundGeneratorCommandProperty);
            set => SetValue(OpenSoundGeneratorCommandProperty, value);
        }

        public static readonly DependencyProperty OpenCommandGeneratorCommandProperty =
            DependencyProperty.Register("OpenCommandGeneratorCommand", typeof(ICommand), typeof(NavigationMenuControl), new PropertyMetadata(null));
        public ICommand OpenCommandGeneratorCommand {
            get => (ICommand)GetValue(OpenCommandGeneratorCommandProperty);
            set => SetValue(OpenCommandGeneratorCommandProperty, value);
        }

        public static readonly DependencyProperty OpenAchievementGeneratorCommandProperty =
            DependencyProperty.Register("OpenAchievementGeneratorCommand", typeof(ICommand), typeof(NavigationMenuControl), new PropertyMetadata(null));
        public ICommand OpenAchievementGeneratorCommand {
            get => (ICommand)GetValue(OpenAchievementGeneratorCommandProperty);
            set => SetValue(OpenAchievementGeneratorCommandProperty, value);
        }

        public static readonly DependencyProperty OpenRecipeGeneratorCommandProperty =
            DependencyProperty.Register("OpenRecipeGeneratorCommand", typeof(ICommand), typeof(NavigationMenuControl), new PropertyMetadata(null));
        public ICommand OpenRecipeGeneratorCommand {
            get => (ICommand)GetValue(OpenRecipeGeneratorCommandProperty);
            set => SetValue(OpenRecipeGeneratorCommandProperty, value);
        }

        public static readonly DependencyProperty OpenSettingsCommandProperty =
            DependencyProperty.Register("OpenSettingsCommandCommand", typeof(ICommand), typeof(NavigationMenuControl), new PropertyMetadata(null));
        public ICommand OpenSettingsCommandCommand {
            get => (ICommand)GetValue(OpenSettingsCommandProperty);
            set => SetValue(OpenSettingsCommandProperty, value);
        }

        public static readonly DependencyProperty SlideSpeedProperty =
            DependencyProperty.Register("SlideSpeed", typeof(double), typeof(NavigationMenuControl), new PropertyMetadata(0.25));
        public double SlideSpeed {
            get => (double)GetValue(SlideSpeedProperty);
            set => SetValue(SlideSpeedProperty, value);
        }

        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(Vector), typeof(NavigationMenuControl), new PropertyMetadata(new Vector(170, 0)));
        public Vector Offset {
            get => (Vector)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        public MenuComponent MenuComponent { get; protected set; }

        private ICommand toggleMenu;
        public ICommand ToggleMenu => toggleMenu ?? (toggleMenu = new DelegateCommand(() => { MenuComponent.Toggle(); }));

        public void InitializeMenu(Grid menuGrid, int row, int column) => MenuComponent = new MenuComponent(menuGrid, new MenuSettings(menuGrid, column, row, Offset, SlideSpeed));
    }
}
