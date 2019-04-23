using System;

namespace ForgeModGenerator
{
    /// <summary> Holds information about page key and page type(class) </summary>
    public class PageInfo
    {
        public string Key { get; }
        public Type Type { get; }

        public PageInfo(string key, Type type)
        {
            Key = key;
            Type = type;
        }
    }

    /// <summary> Holds information about page key, page type(class) and it's viewmodel </summary>
    public class PageInfo<T> : PageInfo
    {
        public Type ViewModelType { get; }
        public PageInfo(string key, Type pageType) : base(key, pageType) => ViewModelType = typeof(T);
    }

    /// <summary> Holds info about every page in application </summary>
    public static class Pages
    {
        public const string RegionName = "PageRegion";

        public const string Dashboard = "Dashboard";
        public const string BuildConfiguration = "BuildConfiguration";
        public const string ModGenerator = "ModGenerator";
        public const string TextureGenerator = "TextureGenerator";
        public const string BlockGenerator = "BlockGenerator";
        public const string ItemGenerator = "ItemGenerator";
        public const string SoundGenerator = "SoundGenerator";
        public const string CommandGenerator = "CommandGenerator";
        public const string AchievementGenerator = "AchievementGenerator";
        public const string RecipeGenerator = "RecipeGenerator";
        public const string Settings = "Settings";
    }
}
