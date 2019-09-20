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
}
