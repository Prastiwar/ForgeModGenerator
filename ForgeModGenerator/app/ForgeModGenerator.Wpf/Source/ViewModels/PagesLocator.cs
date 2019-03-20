using Prism.Mvvm;
using System;

// Holds information about page key and page type(class)
public class PageInfo
{
    public readonly string key;
    public readonly Type type;

    public PageInfo(string key, Type type)
    {
        this.key = key;
        this.type = type;
    }
}

// Automatically registered page
public class PageInfo<T> : PageInfo where T : BindableBase
{
    public Type ViewModelType;
    public PageInfo(string key, Type pageType) : base(key, pageType) => ViewModelType = typeof(T);
}

// Holds info about every page in application
public struct Pages
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