using Newtonsoft.Json;
using System;

namespace ForgeModGenerator.Model
{
    public abstract class WorkspaceSetup
    {
        public static WorkspaceSetup NONE = new EmptyWorkspace();

        [JsonProperty(Required = Required.Always)]
        public string Name { get; protected set; }

        public WorkspaceSetup(string name)
        {
            Name = name;
        }

        public abstract void Setup(Mod mod);
    }

    public class EmptyWorkspace : WorkspaceSetup
    {
        public EmptyWorkspace() : base("None") { }

        public override void Setup(Mod mod) { }
    }

    public class EclipseWorkspace : WorkspaceSetup
    {
        public EclipseWorkspace() : base("Eclipse") { }

        public override void Setup(Mod mod)
        {
            throw new NotImplementedException();
        }
    }

    public class IntelliJIDEAWorkspace : WorkspaceSetup
    {
        public IntelliJIDEAWorkspace() : base("IntelliJIDEA") { }

        public override void Setup(Mod mod)
        {
            throw new NotImplementedException();
        }
    }

    public class VSCodeWorkspace : WorkspaceSetup
    {
        public VSCodeWorkspace() : base("VSCode") { }

        public override void Setup(Mod mod)
        {
            throw new NotImplementedException();
        }
    }
}
