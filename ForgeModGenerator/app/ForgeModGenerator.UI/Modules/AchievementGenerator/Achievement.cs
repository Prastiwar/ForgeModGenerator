using GalaSoft.MvvmLight;
using System.Drawing;

namespace ForgeModGenerator.AchievementGenerator.Models
{
    public class Achievement : ObservableObject
    {
        private string name;
        public string Name {
            get => name;
            set => Set(ref name, value);
        }

        private string description;
        public string Description {
            get => description;
            set => Set(ref description, value);
        }

        private Image icon;
        public Image Icon {
            get => icon;
            set => Set(ref icon, value);
        }
    }
}
