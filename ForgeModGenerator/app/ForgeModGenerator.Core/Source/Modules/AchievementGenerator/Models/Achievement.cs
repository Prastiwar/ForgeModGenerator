using ForgeModGenerator.Models;

namespace ForgeModGenerator.AchievementGenerator.Models
{
    public class Achievement : ObservableDirtyObject, ICopiable
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

        public object Clone() => MemberwiseClone();
        public object DeepClone() => new Achievement() {
            Name = Name,
            Description = Description,
            IconPath = iconPath
        };

        public bool CopyValues(object fromCopy)
        {
            if (fromCopy is Achievement achievement)
            {
                Name = achievement.Name;
                Description = achievement.Description;
                IconPath = achievement.IconPath;
                return true;
            }
            return false;
        }
    }
}
