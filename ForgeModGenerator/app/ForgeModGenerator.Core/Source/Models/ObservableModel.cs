using ForgeModGenerator.Validation;
using System;
using System.ComponentModel;
using System.Reflection;

namespace ForgeModGenerator.Models
{
    public abstract class ObservableModel : ObservableDirtyObject, ICopiable, IDataErrorInfo, IValidable
    {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        public abstract bool CopyValues(object fromCopy);

        public virtual object DeepClone()
        {
            ObservableModel model = (ObservableModel)Activator.CreateInstance(GetType(), true);
            model.CopyValues(this);
            return model;
        }

        public virtual object Clone() => MemberwiseClone();

        public virtual ValidateResult Validate()
        {
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                string errorString = OnValidate(property.Name);
                if (!string.IsNullOrEmpty(errorString))
                {
                    return new ValidateResult(false, errorString);
                }
            }
            return ValidateResult.Valid;
        }

        public event PropertyValidationEventHandler ValidateProperty;
        protected virtual string OnValidate(string propertyName) => ValidateHelper.OnValidateError(ValidateProperty, this, propertyName);

        protected void AddValidateProperty(ObservableModel otherModel) => ValidateProperty += otherModel.ValidateProperty;
        protected void RemoveValidateProperty(ObservableModel otherModel) => ValidateProperty -= otherModel.ValidateProperty;
        protected void SetValidateProperty(ObservableModel otherModel) => ValidateProperty = otherModel.ValidateProperty;
        protected void ClearValidateProperty()
        {
            if (ValidateProperty?.GetInvocationList() != null)
            {
                foreach (Delegate del in ValidateProperty.GetInvocationList())
                {
                    ValidateProperty -= (PropertyValidationEventHandler)del;
                }
            }
        }

        string IDataErrorInfo.Error => Validate().Error;
        string IDataErrorInfo.this[string propertyName] => OnValidate(propertyName);
    }
}
