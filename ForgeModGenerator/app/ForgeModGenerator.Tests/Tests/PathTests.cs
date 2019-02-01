using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.Tests
{
    [TestClass]
    public class PathTests
    {
        [TestMethod]
        public void GetModidFromPath()
        {
            string path = @"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\Craftpolis\src\main\resources";
            string path1 = @"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\Craftpolis\src\main\resources\assets\craftpolis";
            string path2 = @"craftpolis:entity/something/either";
            string path3 = @"craftpolis:entity.something.either";
            string path4 = @"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\Craftpolis";
            string path5 = @"\ForgeModGenerator\ForgeModGenerator\mods\Craftpolis";

            string resultModid = GetModidFromPath(path);
            string resultModid1 = GetModidFromPath(path1);
            string resultModid2 = GetModidFromPath(path2);
            string resultModid3 = GetModidFromPath(path3);
            string resultModid4 = GetModidFromPath(path4);
            string resultModid5 = GetModidFromPath(path5);

            Assert.AreEqual("craftpolis", resultModid);
            Assert.AreEqual("craftpolis", resultModid1);
            Assert.AreEqual("craftpolis", resultModid2);
            Assert.AreEqual("craftpolis", resultModid3);
            Assert.AreEqual("craftpolis", resultModid4);
            Assert.AreEqual(null, resultModid5);
        }

        [TestMethod]
        public void GetModnameFromPath()
        {
            string path = @"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\Craftpolis\src\main\resources";
            string path1 = @"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\Craftpolis\src\main\resources\assets\craftpolis";
            string path2 = @"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\Craftpolis";
            string path3 = @"\ForgeModGenerator\ForgeModGenerator\mods\Craftpolis";

            string resultModid = GetModnameFromPath(path);
            string resultModid1 = GetModnameFromPath(path1);
            string resultModid2 = GetModnameFromPath(path2);
            string resultModid3 = GetModnameFromPath(path3);

            Assert.AreEqual("Craftpolis", resultModid);
            Assert.AreEqual("Craftpolis", resultModid1);
            Assert.AreEqual("Craftpolis", resultModid2);
            Assert.AreEqual(null, resultModid3);
        }

        public static string GetModidFromPath(string path)
        {
            path = path.Replace("\\", "/");
            int index = !path.Contains(":/") ? path.IndexOf(':') : -1;
            if (index >= 1)
            {
                return path.Substring(0, index);
            }
            string modname = GetModnameFromPath(path);
            if (modname != null)
            {
                string assetsPath = $@"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\{modname}\src\main\resources\assets"; // in assets folder there should be always folder with modid
                int assetsPathLength = assetsPath.Length;
                try
                {
                    string directory = Directory.EnumerateDirectories(assetsPath).First();
                    string dir = directory.Replace("\\", "/");
                    return dir.Remove(0, assetsPathLength + 1);
                }
                catch (System.Exception) { }
            }
            return null;
        }

        public static string GetModnameFromPath(string path)
        {
            bool invalidPath = path == null;
            if (invalidPath)
            {
                return null;
            }
            path = path.Replace("\\", "/");
            string modsPath = @"C:/Dev/ForgeModGenerator/ForgeModGenerator/mods";
            int length = modsPath.Length;
            if (!path.StartsWith(modsPath))
            {
                return null;
            }
            try
            {
                string sub = path.Remove(0, length + 1);
                int index = sub.IndexOf("/");
                return index >= 1 ? sub.Substring(0, index) : sub;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}
