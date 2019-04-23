using ForgeModGenerator.Models;
using ForgeModGenerator.Serialization;
using System;

namespace ForgeModGenerator.Services
{
    public sealed class WpfSessionContextService : SessionContextService
    {
        public WpfSessionContextService(ISerializer<McMod> modSerializer)
        {
            Log.Info("Session loading..");
            this.modSerializer = modSerializer;
            startPage = new Uri("../ApplicationModule/DashboardPage.xaml", UriKind.Relative);
            Refresh();
            Log.Info("Session loaded");
        }

        private readonly ISerializer<McMod> modSerializer;

        private readonly Uri startPage;
        public override Uri StartPage => startPage;

        protected override bool TryGetModFromPath(string path, out McMod mcMod)
        {
            mcMod = ModHelper.ImportMod(modSerializer, path);
            return mcMod != null;
        }
    }
}
