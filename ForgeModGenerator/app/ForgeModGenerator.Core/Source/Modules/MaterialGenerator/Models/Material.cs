using ForgeModGenerator.Models;

namespace ForgeModGenerator.MaterialGenerator.Models
{
    public class Material : ObservableDirtyObject
    {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }
    }
}
