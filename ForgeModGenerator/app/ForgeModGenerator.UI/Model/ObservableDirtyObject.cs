using GalaSoft.MvvmLight;
using System.Runtime.CompilerServices;

namespace ForgeModGenerator.Model
{
    public interface IDirty
    {
        bool IsDirty { get; set; }
    }

    public class ObservableDirtyObject : ObservableObject, IDirty
    {
        public bool IsDirty { get; set; }

        protected bool DirtSet<T>(ref T variable, T newValue, [CallerMemberName] string propertyName = null)
        {
            bool set = Set(ref variable, newValue, propertyName);
            if (set)
            {
                IsDirty = true;
            }
            return set;
        }
    }
}
