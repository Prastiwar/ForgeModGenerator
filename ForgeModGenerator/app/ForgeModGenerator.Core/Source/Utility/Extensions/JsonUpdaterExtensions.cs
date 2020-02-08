using ForgeModGenerator.Serialization;

namespace ForgeModGenerator.Utility
{
    public static class JsonUpdaterExtensions
    {
        public static bool IsValidToSerialize(this IJsonUpdater updater, object item)
        {
            object wasTarget = updater.Target;
            bool isValid = updater.SetTarget(item).IsValidToSerialize();
            updater.SetTarget(wasTarget);
            return isValid;
        }
        public static bool IsUpdateAvailable(this IJsonUpdater updater, object item, string path)
        {
            object wasTarget = updater.Target;
            string wasPath = updater.Path;
            bool isUpdateAvailable = updater.SetTarget(item).SetPath(path).IsValidToSerialize();
            updater.SetTarget(wasTarget).SetPath(wasPath);
            return isUpdateAvailable;
        }

        public static string Serialize(this IJsonUpdater updater, object item, bool prettyPrint)
        {
            object wasTarget = updater.Target;
            bool wasPrettyPrint = updater.PrettyPrint;
            string serialized = updater.SetTarget(item).SetPrettyPrint(prettyPrint).Serialize();
            updater.SetTarget(wasTarget).SetPrettyPrint(wasPrettyPrint);
            return serialized;
        }

        public static string Serialize(this IJsonUpdater updater, bool prettyPrint)
        {
            bool wasPrettyPrint = updater.PrettyPrint;
            string serialized = updater.SetPrettyPrint(prettyPrint).Serialize();
            updater.SetPrettyPrint(wasPrettyPrint);
            return serialized;
        }

        public static void ForceJsonUpdate(this IJsonUpdater updater, object target, string path)
        {
            object wasTarget = updater.Target;
            string wasPath = updater.Path;
            updater.SetTarget(target).SetPath(path).ForceJsonUpdate();
            updater.SetTarget(wasTarget).SetPath(wasPath);
        }

        public static void ForceJsonUpdateAsync(this IJsonUpdater updater, object target, string path)
        {
            object wasTarget = updater.Target;
            string wasPath = updater.Path;
            updater.SetTarget(target).SetPath(path).ForceJsonUpdateAsync();
            updater.SetTarget(wasTarget).SetPath(wasPath);
        }

        public static object DeserializeObjectFromPath(this IJsonUpdater updater, string path)
        {
            string wasPath = updater.Path;
            object obj = updater.SetPath(path).DeserializeObject();
            updater.SetPath(wasPath);
            return obj;
        }

        public static T DeserializeFromPath<T>(this IJsonUpdater<T> updater, string path)
        {
            string wasPath = updater.Path;
            updater.SetPath(path);
            T obj = updater.Deserialize();
            updater.SetPath(wasPath);
            return obj;
        }
    }
}
