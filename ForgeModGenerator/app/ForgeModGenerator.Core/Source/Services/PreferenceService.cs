using ForgeModGenerator.Serialization;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator.Services
{
    public sealed class PreferenceService : IPreferenceService, IDisposable
    {
        public PreferenceService(ISerializer<PreferenceData> serializer, IMemoryCache cache, ISynchronizeInvoke synchronizingObject)
        {
            this.serializer = serializer;
            this.cache = cache;
            fileSystemWatcher = new FileSystemWatcherExtended(AppPaths.Preferences, "*.json") {
                NotifyFilter = NotifyFilters.LastWrite,
                SynchronizingObject = synchronizingObject,
                EnableRaisingEvents = true
            };
            fileSystemWatcher.FileChanged += FileSystemWatcher_FileChanged;
            CachePreferences();
        }

        private readonly ISerializer<PreferenceData> serializer;
        private readonly IMemoryCache cache;
        private readonly FileSystemWatcherExtended fileSystemWatcher;

        public void Save(PreferenceData data)
        {
            string json = serializer.Serialize(data, true);
            File.WriteAllText(data.PreferenceLocation, json);
            data.IsDirty = false;
        }

        public T GetOrCreate<T>() where T : PreferenceData
        {
            T preferences = Load<T>();
            if (preferences == null)
            {
                preferences = Activator.CreateInstance<T>();
                Save(preferences);
            }
            return preferences;
        }

        public T Load<T>() where T : PreferenceData
        {
            Type type = typeof(T);
            if (cache.TryGetValue(type, out PreferenceData data))
            {
                return (T)data;
            }
            return LoadFromFileSystem<T>();
        }

        private T LoadFromFileSystem<T>() where T : PreferenceData
        {
            T instance = Activator.CreateInstance<T>();
            if (File.Exists(instance.PreferenceLocation))
            {
                string content = File.ReadAllText(instance.PreferenceLocation);
                object preferences = serializer.Deserialize(content);
                if (preferences is T data)
                {
                    return data;
                }
            }
            return null;
        }

        private void CachePreferences()
        {
            foreach (string filePath in Directory.EnumerateFiles(AppPaths.Preferences, "*.json"))
            {
                CacheFilePreference(filePath);
            }
        }

        private void CacheFilePreference(string filePath)
        {
            string content = File.ReadAllText(filePath);
            PreferenceData preferences = serializer.Deserialize(content);
            if (preferences != null)
            {
                cache.Set(preferences.GetType(), preferences);
            }
        }

        private async void FileSystemWatcher_FileChanged(object sender, FileSystemEventArgs e)
        {
            await System.Threading.Tasks.Task.Delay(500).ConfigureAwait(false);
            CacheFilePreference(e.FullPath);
        }

        public void Dispose() => fileSystemWatcher.Dispose();
    }
}
