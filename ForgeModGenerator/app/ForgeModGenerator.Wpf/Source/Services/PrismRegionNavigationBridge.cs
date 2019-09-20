using System;

namespace ForgeModGenerator.Services
{
    public class PrismRegionNavigationBridge : INavigationService
    {
        public PrismRegionNavigationBridge(Prism.Regions.IRegionManager regionManager) => this.regionManager = regionManager;

        private readonly Prism.Regions.IRegionManager regionManager;

        public const string RegionName = "PageRegion";

        public bool NavigateTo(string name)
        {
            regionManager.RequestNavigate(RegionName, name);
            return true;
        }

        public bool NavigateTo(Uri uri)
        {
            regionManager.RequestNavigate(RegionName, uri);
            return true;
        }
    }
}
