using Newtonsoft.Json;

namespace ForgeModGenerator.Persistence
{
    public class DefaultPreferenceData : PreferenceData
    {
        protected override string GetJsonContent()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All
            };
            return JsonConvert.SerializeObject(this, settings);
        }
    }
}
