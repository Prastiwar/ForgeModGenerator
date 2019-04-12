using ForgeModGenerator.Serialization;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.SoundGenerator.Serialization;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator.ViewModels
{
    public class SoundJsonUpdaterFactory : ISoundJsonUpdaterFactory
    {
        public SoundJsonUpdaterFactory(ISoundEventsSerializer serializer) => this.serializer = serializer;

        private readonly ISoundEventsSerializer serializer;

        public void SetModInfo(string modname, string modid) => serializer.SetModInfo(modname, modid);

        public IJsonUpdater<IEnumerable<SoundEvent>, SoundEvent> Create() => new SoundJsonUpdater(serializer, null, null);
        public IJsonUpdater<IEnumerable<SoundEvent>, SoundEvent> Create(string modname, string modid)
        {
            SetModInfo(modname, modid);
            return new SoundJsonUpdater(serializer, null, null);
        }
    }
}
