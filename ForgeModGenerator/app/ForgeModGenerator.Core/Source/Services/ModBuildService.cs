using ForgeModGenerator.Models;
using System;
using System.Diagnostics;
using System.IO;

namespace ForgeModGenerator.Services
{
    public class ModBuildService : IModBuildService
    {
        public void Compile(Mod mod) => throw new NotImplementedException();

        /// <summary> Run mod depends on mod.LanuchSetup </summary>
        public void Run(Mod mod)
        {
            switch (mod.LaunchSetup)
            {
                case LaunchSetup.Client:
                    RunClient(mod);
                    break;
                case LaunchSetup.Server:
                    RunServer(mod);
                    break;
                default:
                    RunClient(mod);
                    break;
            }
        }

        /// <summary> Ignore LanuchSetup and run client for this mod </summary>
        public void RunClient(Mod mod)
        {
            string modPath = ModPaths.ModRootFolder(mod.ModInfo.Name);
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = "CMD.EXE",
                Arguments = $"/K cd \"{modPath}\" & gradlew runClient"
            };
            Process.Start(psi);
        }

        /// <summary> Ignore LanuchSetup and run server for this mod </summary>
        public void RunServer(Mod mod)
        {
            string modPath = ModPaths.ModRootFolder(mod.ModInfo.Name);
            string eulaPath = Path.Combine(modPath, "run", "eula.txt");
            FileInfo eulaFile = new FileInfo(eulaPath);
            if (!eulaFile.Exists)
            {
                eulaFile.Directory.Create();
                string eulaMessage =
$@"#By changing the setting below to TRUE you are indicating your agreement to our EULA (https://account.mojang.com/documents/minecraft_eula).
#{DateTime.UtcNow}
eula = true";
                File.WriteAllText(eulaPath, eulaMessage);
            }

            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = "CMD.EXE",
                Arguments = $"/K cd \"{modPath}\" & gradlew runServer"
            };
            Process.Start(psi);
        }
    }
}
