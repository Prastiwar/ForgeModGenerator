using ForgeModGenerator.Models;
using ForgeModGenerator.Persistence;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ForgeModGenerator.Services
{
    public sealed class WpfSessionContextService : SessionContextService
    {
        public static WpfSessionContextService Instance { get; }
        static WpfSessionContextService() => Instance = new WpfSessionContextService();

        public WpfSessionContextService()
        {
            Log.Info("Session loading..");
            startPage = new Uri("../ApplicationModule/DashboardPage.xaml", UriKind.Relative);
            Refresh();
            Log.Info("Session loaded");
        }

        private readonly Uri startPage;
        public override Uri StartPage => startPage;

        protected override bool TryGetModFromPath(string path, out Mod mod)
        {
            mod = ModHelper.ImportMod(path);
            Log.Info($"Mod {mod.ModInfo.Name} detected");
            return mod != null;
        }

        protected override bool TryGetPreferencesFromFilePath(string filePath, out PreferenceData preferences)
        {
            string jsonText = File.ReadAllText(filePath);
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings() {
                    TypeNameHandling = TypeNameHandling.All
                };
                preferences = (PreferenceData)JsonConvert.DeserializeObject(jsonText, settings);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to load preferences {filePath}");
                preferences = null;
            }
            return false;
        }
    }
}
