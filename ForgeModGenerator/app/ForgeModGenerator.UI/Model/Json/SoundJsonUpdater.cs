using Newtonsoft.Json;
using System.Collections.Generic;

namespace ForgeModGenerator.Model
{
    public class SoundJsonUpdater : JsonUpdater<SoundEvent>
    {
        public SoundJsonUpdater(IEnumerable<SoundEvent> target, string jsonPath) : base(target, jsonPath) { }
        public SoundJsonUpdater(IEnumerable<SoundEvent> target, string jsonPath, JsonSerializerSettings settings) : base(target, jsonPath, settings) { }
        public SoundJsonUpdater(IEnumerable<SoundEvent> target, string jsonPath, JsonConverter converter) : base(target, jsonPath, converter) { }
        public SoundJsonUpdater(IEnumerable<SoundEvent> target, string jsonPath, Formatting formatting, JsonSerializerSettings settings) : base(target, jsonPath, formatting, settings) { }
        public SoundJsonUpdater(IEnumerable<SoundEvent> target, string jsonPath, Formatting formatting, JsonConverter converter) : base(target, jsonPath, formatting, converter) { }

        public override bool IsValidToSerialize()
        {
            foreach (SoundEvent item in Target)
            {
                System.Windows.Controls.ValidationResult result = item.IsValid(Target);
                if (!result.IsValid)
                {
                    Log.Warning($"Cannot serialize json. {item.EventName} is not valid. Reason: {result.ErrorContent}", true);
                    return false;
                }
            }
            return true;
        }

        public void AddToJson(SoundEvent soundEvent, Sound sound)
        {
            if (!JsonContains(soundEvent, sound))
            {
                // TODO: Add
            }
            ForceJsonUpdate(); // temporary solution
        }

        public void RemoveFromJson(SoundEvent soundEvent, Sound sound)
        {
            if (JsonContains(soundEvent, sound))
            {
                // TODO: Remove
            }
            ForceJsonUpdate(); // temporary solution
        }

        public bool JsonContains(SoundEvent soundEvent, Sound sound)
        {
            string json = GetJsonFromFile();
            string itemJson = Serialize();
            if (json.Contains(itemJson))
            {
                itemJson = JsonConvert.SerializeObject(Target, Formatting == Formatting.Indented ? Formatting.None : Formatting.Indented);
            }
            return json.Contains(itemJson);
        }
    }
}
