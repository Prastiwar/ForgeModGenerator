using Prism.Mvvm;

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

        private string iconPath;
        public string IconPath {
            get => iconPath;
            set => SetProperty(ref iconPath, value);
        }
    }
}
