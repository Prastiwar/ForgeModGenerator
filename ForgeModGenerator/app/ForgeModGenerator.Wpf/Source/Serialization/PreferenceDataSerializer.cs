using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using ForgeModGenerator.Serialization;
using Newtonsoft.Json;

namespace ForgeModGenerator.Serialization
{
    public class PreferenceDataSerializer : ISerializer<PreferenceData>
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings() {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = GetResolver()
        };

        private static PropertyRenameIgnoreResolver GetResolver()
        {
            PropertyRenameIgnoreResolver resolver = new PropertyRenameIgnoreResolver();
            resolver.IgnoreProperty(typeof(PreferenceData), nameof(PreferenceData.PreferenceLocation));
            resolver.IgnoreProperty(typeof(ObservableDirtyObject), nameof(PreferenceData.IsDirty));
            return resolver;
        }

        public PreferenceData DeserializeObject(string value) => JsonConvert.DeserializeObject(value, settings) as PreferenceData;

        public string SerializeObject(PreferenceData value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None, settings);

        public string SerializeObject(PreferenceData value) => JsonConvert.SerializeObject(value, settings);
    }
}
