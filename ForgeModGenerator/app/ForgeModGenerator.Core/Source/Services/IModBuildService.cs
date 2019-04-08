using ForgeModGenerator.Models;

namespace ForgeModGenerator.Services
{
    public interface IModBuildService
    {
        void Run(Mod mod);
        void RunClient(Mod mod);
        void RunServer(Mod mod);

        void Compile(Mod mod);
    }
}
