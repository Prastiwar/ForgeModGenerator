using ForgeModGenerator.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ForgeModGenerator.Converter
{
    public class SoundEventConverter : JsonConverter<SoundEvent>
    {
        public override SoundEvent ReadJson(JsonReader reader, Type objectType, SoundEvent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            string soundsJson = item.GetValue("sounds").ToString();
            List<Sound> sounds = JsonConvert.DeserializeObject<List<Sound>>(soundsJson, new SoundConverter());
            string name = item.TryGetValue("EventName", out JToken eventName) ? eventName.ToObject<string>() : "";
            SoundEvent soundEvent = new SoundEvent(name, sounds);
            if (item.TryGetValue("replace", out JToken replace))
            {
                soundEvent.Replace = replace.ToObject<bool>();
            }
            if (item.TryGetValue("subtitle", out JToken subtitle))
            {
                soundEvent.Subtitle = subtitle.ToObject<string>();
            }
            return soundEvent;
        }

        public override void WriteJson(JsonWriter writer, SoundEvent value, JsonSerializer serializer)
        {
            writer.WriteRawValue(JsonConvert.SerializeObject(value));
        }
    }
}
