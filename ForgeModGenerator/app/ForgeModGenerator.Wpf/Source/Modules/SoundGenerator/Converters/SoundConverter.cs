using ForgeModGenerator.SoundGenerator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ForgeModGenerator.SoundGenerator.Converters
{
    public class SoundConverter : JsonConverter<Sound>
    {
        public override Sound ReadJson(JsonReader reader, Type objectType, Sound existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            string name = item.GetValue("name").ToObject<string>();
            string modid = Sound.GetModidFromSoundName(name);
            Sound sound = Sound.CreateEmpty(name, modid);
            if (item.TryGetValue("volume", out JToken volume))
            {
                sound.Volume = volume.ToObject<float>();
            }
            if (item.TryGetValue("pitch", out JToken pitch))
            {
                sound.Pitch = pitch.ToObject<float>();
            }
            if (item.TryGetValue("weight", out JToken weight))
            {
                sound.Weight = weight.ToObject<int>();
            }
            if (item.TryGetValue("stream", out JToken stream))
            {
                sound.Stream = stream.ToObject<bool>();
            }
            if (item.TryGetValue("attenuation_distance", out JToken attenuation_distance))
            {
                sound.AttenuationDistance = attenuation_distance.ToObject<int>();
            }
            if (item.TryGetValue("preload", out JToken preload))
            {
                sound.Preload = preload.ToObject<bool>();
            }
            if (item.TryGetValue("type", out JToken type))
            {
                sound.Type = preload.ToObject<Sound.SoundType>();
            }
            sound.IsDirty = false;
            return sound; // NOTE: This is not properly initialized Sound, Info is not initialized
        }

        public override void WriteJson(JsonWriter writer, Sound value, JsonSerializer serializer)
        {
               JObject jo = new JObject {
                { nameof(Sound.Name).ToLower(), value.Name },
                { nameof(Sound.Volume).ToLower(), value.Volume },
                { nameof(Sound.Pitch).ToLower(), value.Pitch },
                { nameof(Sound.Weight).ToLower(), value.Weight },
                { nameof(Sound.Stream).ToLower(), value.Stream },
                { "attenuation_distance", value.AttenuationDistance },
                { nameof(Sound.Preload).ToLower(), value.Preload },
                { nameof(Sound.Type).ToLower(), value.Type.ToString() }
            };
            jo.WriteTo(writer);
        }
    }
}
