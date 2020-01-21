using System;
using System.Diagnostics;
using System.IO;

namespace ForgeModGenerator.Services
{
    public class PrismRegionNavigationBridge : INavigationService
    {
        public PrismRegionNavigationBridge(Prism.Regions.IRegionManager regionManager) => this.regionManager = regionManager;

        private readonly Prism.Regions.IRegionManager regionManager;

        public const string RegionName = "PageRegion";

        public bool NavigateTo(string name)
        {
            if (File.Exists(name))
            {
                Process.Start(Path.GetDirectoryName(name));
            }
            else if (Directory.Exists(name))
            {
                Process.Start(name);
            }
            else
            {
                regionManager.RequestNavigate(RegionName, name);
            }
            return true;
        }

        public bool NavigateTo(Uri uri) => NavigateTo(uri.OriginalString);
    }
}
