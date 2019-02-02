using ForgeModGenerator.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ForgeModGenerator.Converter
{
    public class SoundEventConverter : JsonConverter<SoundEvent>
    {
        public override SoundEvent ReadJson(JsonReader reader, Type objectType, SoundEvent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);

            string soundsJson = item.GetValue("sounds").ToString();
            PropertyRenameIgnoreResolver soundRenameIgnoreResolver = new PropertyRenameIgnoreResolver();
            soundRenameIgnoreResolver.IgnoreProperty(typeof(Sound), nameof(Sound.Info));

            JsonSerializerSettings soundSerializerSettings = new JsonSerializerSettings() {
                ContractResolver = soundRenameIgnoreResolver,
                Converters = new List<JsonConverter>() { new SoundConverter() }
            };
            List<Sound> sounds = JsonConvert.DeserializeObject<List<Sound>>(soundsJson, soundSerializerSettings);// new SoundConverter());

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
            soundEvent.IsDirty = false;
            return soundEvent;
        }

        public override void WriteJson(JsonWriter writer, SoundEvent value, JsonSerializer serializer)
        {
            MessageBox.Show(value.ToString());
            writer.WriteRawValue(JsonConvert.SerializeObject(value, serializer.Formatting));
        }
    }
}
