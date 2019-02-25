using ForgeModGenerator.SoundGenerator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator.Converters
{
    public class SoundEventConverter : JsonConverter<SoundEvent>
    {
        public override SoundEvent ReadJson(JsonReader reader, Type objectType, SoundEvent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            string soundsJson = item.GetValue("sounds", StringComparison.OrdinalIgnoreCase).ToString();
            List<Sound> sounds = JsonConvert.DeserializeObject<List<Sound>>(soundsJson);

            SoundEvent soundEvent = SoundEvent.CreateEmpty(sounds);
            soundEvent.EventName = SoundEvent.FormatDottedSoundNameFromSoundName(sounds[0].Name);

            if (item.TryGetValue(nameof(SoundEvent.Replace), StringComparison.OrdinalIgnoreCase, out JToken replace))
            {
                soundEvent.Replace = replace.ToObject<bool>();
            }
            if (item.TryGetValue(nameof(SoundEvent.Subtitle), StringComparison.OrdinalIgnoreCase, out JToken subtitle))
            {
                soundEvent.Subtitle = subtitle.ToObject<string>();
            }
            soundEvent.IsDirty = false;
            return soundEvent; // NOTE: This is not properly initialized SoundEvent, Info is not initialized
        }

        public override void WriteJson(JsonWriter writer, SoundEvent value, JsonSerializer serializer)
        {
            if (value.Files.Count <= 0)
            {
                return;
            }
            writer.WriteRawValue($"\"{value.EventName}\":");
            if (serializer.Formatting == Formatting.Indented)
            {
                writer.WriteRawValue(" ");
            }

            JObject jo = new JObject {
                { nameof(SoundEvent.Replace).ToLower(), value.Replace },
                { nameof(SoundEvent.Subtitle).ToLower(), value.Subtitle ?? "" },
                { "sounds", JArray.FromObject(value.Files, serializer) }
            };
            jo.WriteTo(writer);
        }
    }
}
