using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeModGenerator.Model
{
    public abstract class PreferenceData
    {
        public abstract string PreferenceLocation { get; }

        public virtual void SavePreferences()
        {

        }

        public virtual void LoadPreferences()
        {

        }
    }
}
