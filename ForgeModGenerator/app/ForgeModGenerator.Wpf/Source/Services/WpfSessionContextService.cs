using ForgeModGenerator.Models;
using System;

namespace ForgeModGenerator.Services
{
    public sealed class WpfSessionContextService : SessionContextService
    {
        public static WpfSessionContextService Instance { get; }
        static WpfSessionContextService() => Instance = new WpfSessionContextService();

        private WpfSessionContextService()
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
            return mod != null;
        }
    }
}
