using Prism.Mvvm;
using System.Runtime.CompilerServices;

namespace ForgeModGenerator.Models
{
    public class ObservableDirtyObject : BindableBase, IDirty
    {
        public bool IsDirty { get; set; }

        protected bool DirtSetProperty<T>(ref T variable, T newValue, [CallerMemberName] string propertyName = null)
        {
            bool set = SetProperty(ref variable, newValue, propertyName);
            if (set)
            {
                IsDirty = true;
            }
            return set;
        }
    }
}
