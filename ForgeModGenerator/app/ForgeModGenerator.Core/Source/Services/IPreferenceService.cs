using ForgeModGenerator.Serialization;

namespace ForgeModGenerator.Services
{
    public interface IPreferenceService
    {
        void Save(PreferenceData data);
        T Load<T>() where T : PreferenceData;
        T GetOrCreate<T>() where T : PreferenceData;
    }
}
