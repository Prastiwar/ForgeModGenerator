using ForgeModGenerator.Models;
using ForgeModGenerator.Serialization;
using System;

namespace ForgeModGenerator.Services
{
    public sealed class WpfSessionContextService : SessionContextService
    {
        public WpfSessionContextService(ISerializer<Mod> modSerializer)
        {
            Log.Info("Session loading..");
            this.modSerializer = modSerializer;
            startPage = new Uri("../ApplicationModule/DashboardPage.xaml", UriKind.Relative);
            Refresh();
            Log.Info("Session loaded");
        }

        private readonly ISerializer<Mod> modSerializer;

        private readonly Uri startPage;
        public override Uri StartPage => startPage;

        protected override bool TryGetModFromPath(string path, out Mod mod)
        {
            mod = ModHelper.ImportMod(modSerializer, path);
            return mod != null;
        }
    }
}
