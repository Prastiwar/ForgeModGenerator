using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ForgeModGenerator
{
    public class Cache : MemoryCache
    {
        public Cache(IOptions<MemoryCacheOptions> optionsAccessor) : base(optionsAccessor) { }

        static Cache() => Default = new Cache(new MemoryCacheOptions());

        public static IMemoryCache Default { get; }
    }
}
