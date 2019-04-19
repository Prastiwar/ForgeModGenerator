using ForgeModGenerator.Serialization;
using System.Diagnostics;
using System.IO;

namespace ForgeModGenerator.Models
{
    /// <summary> Base class for workspace to setup for manual changes in mod </summary>
    public abstract class WorkspaceSetup
    {
        public static WorkspaceSetup NONE = new EmptyWorkspace();

        public string Name => GetType().Name.Replace("Workspace", "");

        public abstract void Setup(Mod mod);

        public override bool Equals(object obj) => obj is WorkspaceSetup objSetup && objSetup.Name == Name;
        public override int GetHashCode() => base.GetHashCode();
    }

    public class EmptyWorkspace : WorkspaceSetup
    {
        public override void Setup(Mod mod) { }
    }

    public class EclipseWorkspace : WorkspaceSetup
    {
        public override void Setup(Mod mod)
        {
            string modPath = ModPaths.ModRootFolder(mod.ModInfo.Name);
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = "CMD.EXE",
                Arguments = $"/K cd \"{modPath}\" & gradlew setupDecompWorkspace"
            };
            Process.Start(psi);
            psi.Arguments = $"/K cd \"{modPath}\" & gradlew eclipse";
            Process.Start(psi);
        }
    }

    public class IntelliJIDEAWorkspace : WorkspaceSetup
    {
        public override void Setup(Mod mod)
        {
            string modPath = ModPaths.ModRootFolder(mod.ModInfo.Name);
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = "CMD.EXE",
                Arguments = $"/K cd \"{modPath}\" & gradlew setupDecompWorkspace"
            };
            Process.Start(psi);
            psi.Arguments = $"/K cd \"{modPath}\" & gradlew idea";
            Process.Start(psi);
        }
    }

    public class VSCodeWorkspace : WorkspaceSetup
    {
        public VSCodeWorkspace(ISerializer<VSCLaunch> serializer) => this.serializer = serializer;

        private readonly ISerializer<VSCLaunch> serializer;

        public override void Setup(Mod mod)
        {
            string modPath = ModPaths.ModRootFolder(mod.ModInfo.Name);
            string vscPath = Path.Combine(modPath, ".vscode");
            string workspacePath = Path.Combine(modPath, "workspace.code-workspace");
            string launchPath = Path.Combine(vscPath, "launch.json");

            string buildProgramPath = "${workspaceFolder}/.vscode/build.py";

            VSCConfiguration build = new VSCConfiguration {
                Name = "Build",
                Program = buildProgramPath,
                Console = "externalTerminal",
                Type = "python",
                Request = "launch",
                Args = new string[] { "build", mod.ModInfo.Name }
            };

            VSCConfiguration runClient = new VSCConfiguration {
                Name = "Run Client",
                Program = buildProgramPath,
                Console = "externalTerminal",
                Type = "python",
                Request = "launch",
                Args = new string[] { "run", mod.ModInfo.Name, "client" }
            };

            VSCConfiguration runServer = new VSCConfiguration {
                Name = "Run Server",
                Program = buildProgramPath,
                Console = "externalTerminal",
                Type = "python",
                Request = "launch",
                Args = new string[] { "run", mod.ModInfo.Name, "server" }
            };

            VSCLaunch launcher = new VSCLaunch {
                Version = "0.2.0",
                Configurations = new VSCConfiguration[] {
                    build, runClient, runServer
                }
            };

            string json = serializer.Serialize(launcher, true);
            new FileInfo(launchPath).Directory.Create();
            File.WriteAllText(launchPath, json);
            File.WriteAllText(workspacePath, "{ \"folders\": [ { \"path\": \".\" } ] }");
        }
    }
}
