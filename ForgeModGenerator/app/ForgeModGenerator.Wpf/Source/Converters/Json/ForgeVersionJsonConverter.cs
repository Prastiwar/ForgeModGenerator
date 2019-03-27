using ForgeModGenerator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ForgeModGenerator.Converters
{
    public class ForgeVersionJsonConverter : JsonConverter<ForgeVersion>
    {
        public override ForgeVersion ReadJson(JsonReader reader, Type objectType, ForgeVersion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            if (!item.HasValues)
            {
                return null;
            }
            string zipPath = item.GetValue(nameof(ForgeVersion.ZipPath), StringComparison.OrdinalIgnoreCase).ToObject<string>();
            string name = null;
            if (item.TryGetValue(nameof(ForgeVersion.Name), out JToken nameValue))
            {
                name = nameValue.ToObject<string>();
            }
            return new ForgeVersion(name, zipPath);
        }

        public override void WriteJson(JsonWriter writer, ForgeVersion value, JsonSerializer serializer)
        {
            JObject jo = new JObject {
                { nameof(ForgeVersion.Name), value.Name },
                { nameof(ForgeVersion.ZipPath), value.ZipPath }
            };
            jo.WriteTo(writer);
        }
    }
}
