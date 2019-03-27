using System.IO;

namespace ForgeModGenerator.Tests
{
    /// <summary> Base class for testing actual behavior on TestMod environment that's located in /mods/TestMod folder </summary>
    public class IntegratedUnitTests
    {
        public IntegratedUnitTests()
        {
            if (!Directory.Exists(ModPaths.ModRootFolder("TestMod")))
            {
                throw new System.InvalidOperationException("You cannot run these tests without TestMod environment");
            }
        }
    }
}
