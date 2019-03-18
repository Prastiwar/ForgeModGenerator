using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace ForgeModGenerator.Models
{
    public class ObservableDirtyObject : ObservableObject, IDirty
    {
        [JsonIgnore]
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
