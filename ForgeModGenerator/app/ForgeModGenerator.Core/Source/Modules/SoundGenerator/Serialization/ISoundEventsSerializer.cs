using ForgeModGenerator.Serialization;
using ForgeModGenerator.SoundGenerator.Models;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator.Serialization
{
    public interface ISoundEventsSerializer : ISerializer<IEnumerable<SoundEvent>, SoundEvent>
    {
        void SetModInfo(string modname, string modid);
    }
}
