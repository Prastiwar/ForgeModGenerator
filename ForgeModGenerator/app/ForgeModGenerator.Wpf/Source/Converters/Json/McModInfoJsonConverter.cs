using ForgeModGenerator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ForgeModGenerator.Converters
{
    public class McModInfoJsonConverter : JsonConverter<McModInfo>
    {
        public override McModInfo ReadJson(JsonReader reader, Type objectType, McModInfo existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            JObject.Load(reader).ToObject<McModInfo>();

        public override void WriteJson(JsonWriter writer, McModInfo value, JsonSerializer serializer)
        {
            JObject jo = new JObject {
                { nameof(McModInfo.Modid).ToLower(), value.Modid ?? "" },
                { nameof(McModInfo.Name).ToLower(), value.Name ?? "" },
                { nameof(McModInfo.Description).ToLower(), value.Description ?? "" },
                { nameof(McModInfo.Version).ToLower(), value.Version ?? "" },
                { "mcVersion", value.McVersion ?? "" },
                { nameof(McModInfo.Url).ToLower(), value.Url ?? "" },
                { "updateUrl", value.UpdateUrl ?? "" },
                { nameof(McModInfo.Credits).ToLower(), value.Credits ?? "" },
                { "logoFile", value.LogoFile  ?? "" },
                { "authorList", value.AuthorList != null ? JToken.FromObject(value.AuthorList, serializer) : "" },
                { nameof(McModInfo.Screenshots).ToLower(), value.Screenshots != null ? JToken.FromObject(value.Screenshots) : "" },
                { nameof(McModInfo.Dependencies).ToLower(), value.Dependencies != null ? JToken.FromObject(value.Dependencies) : "" }
            };
            jo.WriteTo(writer);
        }
    }
}
