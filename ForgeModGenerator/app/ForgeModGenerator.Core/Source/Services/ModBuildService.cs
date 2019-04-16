using ForgeModGenerator.Models;
using System.Diagnostics;

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
        public void RunClient(Mod mod)
        {
            string path = ModPaths.ModRootFolder(mod.ModInfo.Name);
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = "CMD.EXE",
                Arguments = $"/K cd \"{path}\" & gradlew runClient"
            };
            Process.Start(psi);
        }

        /// <summary> Ignore LanuchSetup and run server for this mod </summary>
        public void RunServer(Mod mod)
        {
            string path = ModPaths.ModRootFolder(mod.ModInfo.Name);
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = "CMD.EXE",
                Arguments = $"/K cd \"{path}\" & gradlew runServer"
            };
            Process.Start(psi);
        }
    }
}
