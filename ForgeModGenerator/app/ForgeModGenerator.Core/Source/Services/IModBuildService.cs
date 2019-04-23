using ForgeModGenerator.Models;

namespace ForgeModGenerator.Services
{
    public interface IModBuildService
    {
        void Run(McMod mcMod);
        void RunClient(McMod mcMod);
        void RunServer(McMod mcMod);

        void Compile(McMod mcMod);
    }
}
