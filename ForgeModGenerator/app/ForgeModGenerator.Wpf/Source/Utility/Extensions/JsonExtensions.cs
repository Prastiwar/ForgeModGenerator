using Newtonsoft.Json;

namespace ForgeModGenerator
{
    public static class JsonExtensions
    {
        public static string FormatJson(this string json, Formatting format) => JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), format);
    }
}
