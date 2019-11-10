using ForgeModGenerator.Models;
using ForgeModGenerator.Validation;
using System.ComponentModel;

namespace ForgeModGenerator.MaterialGenerator.Models
{
    public class Material : ObservableDirtyObject, ICopiable, IDataErrorInfo, IValidable<Material>
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

        public ValidateResult Validate() => throw new System.NotImplementedException();

        public event PropertyValidationEventHandler<Material> ValidateProperty;
        string IDataErrorInfo.Error => null;
        string IDataErrorInfo.this[string propertyName] => OnValidate(propertyName);
        private string OnValidate(string propertyName) => ValidateHelper.OnValidateError(ValidateProperty, this, propertyName);
    }
}
