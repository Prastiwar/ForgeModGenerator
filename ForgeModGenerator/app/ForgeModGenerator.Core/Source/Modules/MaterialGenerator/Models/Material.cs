using ForgeModGenerator.Models;

namespace ForgeModGenerator.MaterialGenerator.Models
{
    public class Material : ObservableDirtyObject, ICopiable
    {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }
        public virtual object Clone() => MemberwiseClone();
        public virtual object DeepClone() => new Material() {
            Name = Name,
        };

        public virtual bool CopyValues(object fromCopy)
        {
            if (fromCopy is Material material)
            {
                Name = material.Name;
                return true;
            }
            return false;
        }
    }
}
