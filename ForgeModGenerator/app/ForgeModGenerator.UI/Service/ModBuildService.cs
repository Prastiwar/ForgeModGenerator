using ForgeModGenerator.Model;

namespace ForgeModGenerator.Service
{
    public interface IModBuildService
    {
        void Run(Mod mod);
        void Compile(Mod mod);
    }

    public class ModBuildService : IModBuildService
    {
        public void Compile(Mod mod)
        {
            throw new System.NotImplementedException();
        }

        public void Run(Mod mod)
        {
            throw new System.NotImplementedException();
        }
    }
}
