using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ForgeModGenerator.Core;

namespace ForgeModGenerator.Model
{
    public abstract class PreferenceData : ObservableObject
    {
        public string PreferenceLocation => Path.Combine(AppPaths.Preferences, $"{GetType().Name}.json");

        public virtual void SavePreferences()
        {

        }

        public virtual void LoadPreferences()
        {

        }
    }
}
