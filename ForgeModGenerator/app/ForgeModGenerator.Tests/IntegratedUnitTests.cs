using System.IO;

namespace ForgeModGenerator.Tests
{
    /// <summary> Base class for testing actual behavior on TestMod environment that's located in /mods/TestMod folder </summary>
    public class IntegratedUnitTests
    {
        public const string TestModName = "TestMod";
        public const string TestModModid = "testmod";

        protected string TestModFolder => ModPaths.ModRootFolder(TestModName);

        public IntegratedUnitTests()
        {
            if (!Directory.Exists(TestModFolder))
            {
                throw new System.InvalidOperationException("You cannot run these tests without TestMod environment");
            }
        }
    }
}
