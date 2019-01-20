using ForgeModGenerator.Model;

namespace ForgeModGenerator.Service
{
    public interface IModBuildService
    {
        void Run(Mod mod);
        void RunClient(Mod mod);
        void RunServer(Mod mod);
        void Compile(Mod mod);
    }

    public class ModBuildService : IModBuildService
    {
        public void Compile(Mod mod)
        {
            throw new System.NotImplementedException();
        }

        // Run mod depends on mod.LanuchSetup
        public void Run(Mod mod)
        {
            if (mod.LaunchSetup.RunClient)
            {
                RunClient(mod);
            }
            if (mod.LaunchSetup.RunServer)
            {
                RunServer(mod);
            }
        }

        // Ignore LanuchSetup and run client for this mod
        public void RunClient(Mod mod)
        {
            throw new System.NotImplementedException();
        }

        // Ignore LanuchSetup and run server for this mod
        public void RunServer(Mod mod)
        {
            throw new System.NotImplementedException();
        }
    }
}
