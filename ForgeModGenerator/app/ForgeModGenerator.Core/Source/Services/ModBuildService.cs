using ForgeModGenerator.Models;

namespace ForgeModGenerator.Services
{
    public class ModBuildService : IModBuildService
    {
        public void Compile(Mod mod) => throw new System.NotImplementedException();

        /// <summary> Run mod depends on mod.LanuchSetup </summary>
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

        /// <summary> Ignore LanuchSetup and run client for this mod </summary>
        public void RunClient(Mod mod) => throw new System.NotImplementedException();

        /// <summary> Ignore LanuchSetup and run server for this mod </summary>
        public void RunServer(Mod mod) => throw new System.NotImplementedException();
    }
}
