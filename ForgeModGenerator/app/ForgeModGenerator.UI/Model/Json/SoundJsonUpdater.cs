using Newtonsoft.Json;

namespace ForgeModGenerator.Model
{
    public class SoundJsonUpdater : JsonUpdater<SoundEvent>
    {
        public SoundJsonUpdater(object target, string jsonPath) : base(target, jsonPath) { }
        public SoundJsonUpdater(object target, string jsonPath, JsonSerializerSettings settings) : base(target, jsonPath, settings) { }
        public SoundJsonUpdater(object target, string jsonPath, JsonConverter converter) : base(target, jsonPath, converter) { }
        public SoundJsonUpdater(object target, string jsonPath, Formatting formatting, JsonSerializerSettings settings) : base(target, jsonPath, formatting, settings) { }
        public SoundJsonUpdater(object target, string jsonPath, Formatting formatting, JsonConverter converter) : base(target, jsonPath, formatting, converter) { }

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
