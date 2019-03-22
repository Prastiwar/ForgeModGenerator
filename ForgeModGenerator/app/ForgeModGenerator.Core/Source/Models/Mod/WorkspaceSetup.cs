using System;

namespace ForgeModGenerator.Models
{
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
        public override void Setup(Mod mod) => throw new NotImplementedException();
    }

    public class IntelliJIDEAWorkspace : WorkspaceSetup
    {
        public override void Setup(Mod mod) => throw new NotImplementedException();
    }

    public class VSCodeWorkspace : WorkspaceSetup
    {
        public override void Setup(Mod mod) => throw new NotImplementedException();
    }
}
