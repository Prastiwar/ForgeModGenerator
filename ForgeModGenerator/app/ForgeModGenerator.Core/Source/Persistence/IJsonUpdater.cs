namespace ForgeModGenerator.Persistence
{
    public interface IJsonUpdater
    {
        string Path { get; set; }

        string Serialize(bool prettyPrint);

        void ForceJsonUpdate();
        void ForceJsonUpdateAsync();

        bool IsValidToSerialize();
        bool IsUpdateAvailable();
    }

    public interface IJsonUpdater<T> : IJsonUpdater
    {
        T Target { get; set; }
    }
}
