using ForgeModGenerator.Persistence;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Validation;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator.Persistence
{
    public class SoundJsonUpdater : CollectionJsonUpdater<SoundEvent>
    {
        public SoundJsonUpdater(IEnumerable<SoundEvent> target, string jsonPath) : base(target, jsonPath) { }
        public SoundJsonUpdater(IEnumerable<SoundEvent> target, string jsonPath, JsonSerializerSettings settings) : base(target, jsonPath, settings) { }
        public SoundJsonUpdater(IEnumerable<SoundEvent> target, string jsonPath, JsonConverter converter) : base(target, jsonPath, converter) { }
        public SoundJsonUpdater(IEnumerable<SoundEvent> target, string jsonPath, Formatting formatting, JsonSerializerSettings settings) : base(target, jsonPath, formatting, settings) { }
        public SoundJsonUpdater(IEnumerable<SoundEvent> target, string jsonPath, Formatting formatting, JsonConverter converter) : base(target, jsonPath, formatting, converter) { }

        public override bool IsValidToSerialize()
        {
            foreach (SoundEvent soundEvent in Target)
            {
                ValidateResult result = soundEvent.Validate();
                if (!result.IsValid)
                {
                    Log.Warning($"Cannot serialize json. {soundEvent.EventName} is not valid. Reason: {result.Error}", true);
                    return false;
                }
            }
            return true;
        }
    }
}
