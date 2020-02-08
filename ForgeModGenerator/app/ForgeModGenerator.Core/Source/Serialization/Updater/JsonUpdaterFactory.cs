namespace ForgeModGenerator.Serialization
{
    public class JsonUpdaterFactory<T> : IJsonUpdaterFactory<T>
    {
        public JsonUpdaterFactory(ISerializer<T> serializer) => this.serializer = serializer;

        private readonly ISerializer<T> serializer;

        public IJsonUpdater<T> Create() => new JsonUpdater<T>(serializer);
    }
}
