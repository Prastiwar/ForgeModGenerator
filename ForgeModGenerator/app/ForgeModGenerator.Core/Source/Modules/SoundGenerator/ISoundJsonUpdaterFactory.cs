using ForgeModGenerator.Serialization;
using ForgeModGenerator.SoundGenerator.Models;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator.Serialization
{
    public interface ISoundJsonUpdaterFactory : IJsonUpdaterFactory<IEnumerable<SoundEvent>, SoundEvent>
    {
        void SetModInfo(string modname, string modid);
        IJsonUpdater<IEnumerable<SoundEvent>, SoundEvent> Create(string modname, string modid);
    }
}
