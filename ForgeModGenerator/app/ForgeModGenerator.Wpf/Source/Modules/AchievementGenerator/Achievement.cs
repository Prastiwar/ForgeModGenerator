using Prism.Mvvm;
using System.Drawing;

namespace ForgeModGenerator.AchievementGenerator.Models
{
    public class Achievement : BindableBase
    {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string description;
        public string Description {
            get => description;
            set => SetProperty(ref description, value);
        }

        private Image icon;
        public Image Icon {
            get => icon;
            set => SetProperty(ref icon, value);
        }
    }
}
