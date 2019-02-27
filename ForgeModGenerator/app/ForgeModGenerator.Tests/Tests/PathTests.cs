using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ForgeModGenerator.Tests
{
    [TestClass]
    public class PathTests
    {
        [TestMethod]
        public void GetUniqueName()
        {
            Assert.AreEqual("smth(5)", IOHelper.GetUniqueName("smth", (name) => name == "smth(5)"));
            HashSet<string> strings = new HashSet<string> { "test", "test(1)", "test(2)" };
            Assert.AreEqual("test(3)", IOHelper.GetUniqueName("test", (name) => !strings.Contains(name)));
        }

        [TestMethod]
        public void Subpaths()
        {
            Assert.IsTrue(IOHelper.IsSubPathOf(@"c:\foo", @"c:"));
            Assert.IsTrue(IOHelper.IsSubPathOf(@"c:\foo", @"c:\"));
            Assert.IsTrue(IOHelper.IsSubPathOf(@"c:\foo", @"c:\foo"));
            Assert.IsTrue(IOHelper.IsSubPathOf(@"c:\foo", @"c:\foo\"));
            Assert.IsTrue(IOHelper.IsSubPathOf(@"c:\foo\", @"c:\foo"));
            Assert.IsTrue(IOHelper.IsSubPathOf(@"c:\foo\bar\", @"c:\foo\"));
            Assert.IsTrue(IOHelper.IsSubPathOf(@"c:\foo\bar", @"c:\foo\"));
            Assert.IsTrue(IOHelper.IsSubPathOf(@"c:\foo\a.txt", @"c:\foo"));
            Assert.IsTrue(IOHelper.IsSubPathOf(@"c:\FOO\a.txt", @"c:\foo"));
            Assert.IsTrue(IOHelper.IsSubPathOf(@"c:/foo/a.txt", @"c:\foo"));
            Assert.IsTrue(IOHelper.IsSubPathOf(@"c:\foo\..\bar\baz", @"c:\bar"));
            Assert.IsFalse(IOHelper.IsSubPathOf(@"c:\foobar", @"c:\foo"));
            Assert.IsFalse(IOHelper.IsSubPathOf(@"c:\foobar\a.txt", @"c:\foo"));
            Assert.IsFalse(IOHelper.IsSubPathOf(@"c:\foobar\a.txt", @"c:\foo\"));
            Assert.IsFalse(IOHelper.IsSubPathOf(@"c:\foo\a.txt", @"c:\foobar"));
            Assert.IsFalse(IOHelper.IsSubPathOf(@"c:\foo\a.txt", @"c:\foobar\"));
            Assert.IsFalse(IOHelper.IsSubPathOf(@"c:\foo\..\bar\baz", @"c:\foo"));
            Assert.IsFalse(IOHelper.IsSubPathOf(@"c:\foo\..\bar\baz", @"c:\barr"));
        }

        [TestMethod]
        public void SoundPath()
        {
            Assert.AreEqual("testmod", Sound.GetModidFromSoundName("testmod:entity/jump"));
            Assert.AreEqual("entity/jump", Sound.GetRelativePathFromSoundName("testmod:entity/jump"));
        }

        [TestMethod]
        public void PathValidation()
        {
            Assert.IsTrue(IOHelper.IsDirectoryPath(@"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod\src\main\resources"));
            Assert.IsTrue(IOHelper.IsDirectoryPath(@"\ForgeModGenerator\ForgeModGenerator\mods\TestMod"));
            Assert.IsTrue(IOHelper.IsDirectoryPath(@"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod\src\main\resources"));
            Assert.IsTrue(IOHelper.IsDirectoryPath(@"TestMod"));

            Assert.IsFalse(IOHelper.IsDirectoryPath(@"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod\src\main\resources.smth"));
            Assert.IsFalse(IOHelper.IsDirectoryPath(@"TestMod.lol"));

            Assert.IsFalse(IOHelper.IsDirectoryPath(@"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod\src\main\resou:;<>rcessmth"));
            Assert.IsFalse(IOHelper.IsDirectoryPath(@"<>?:32"));

            Assert.IsTrue(IOHelper.IsFilePath(@"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod\src\main\resources\smth.png"));
            Assert.IsTrue(IOHelper.IsFilePath(@"\ForgeModGenerator\ForgeModGenerator\mods\TestMod.png"));
            Assert.IsTrue(IOHelper.IsFilePath(@"TestMod.png"));

            Assert.IsFalse(IOHelper.IsFilePath(@"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod\src\main\resou:;<>rces.smth"));
            Assert.IsFalse(IOHelper.IsFilePath(@"<>?:32.png"));

            Assert.IsTrue(IOHelper.IsPathValid(@"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod\src\main\resources"));
            Assert.IsTrue(IOHelper.IsPathValid(@"\ForgeModGenerator\ForgeModGenerator\mods\TestMod"));
        }

        [TestMethod]
        public void GetModidFromPath()
        {
            string path = @"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod\src\main\resources";
            string path1 = @"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod\src\main\resources\assets\testmod";
            string path2 = @"testmod:entity/something/either";
            string path3 = @"testmod:entity.something.either";
            string path4 = @"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod";
            string path5 = @"\ForgeModGenerator\ForgeModGenerator\mods\TestMod";

            string resultModid = Models.Mod.GetModidFromPath(path);
            string resultModid1 = Models.Mod.GetModidFromPath(path1);
            string resultModid2 = Models.Mod.GetModidFromPath(path2);
            string resultModid3 = Models.Mod.GetModidFromPath(path3);
            string resultModid4 = Models.Mod.GetModidFromPath(path4);
            string resultModid5 = Models.Mod.GetModidFromPath(path5);

            Assert.AreEqual("testmod", resultModid);
            Assert.AreEqual("testmod", resultModid1);
            Assert.AreEqual("testmod", resultModid2);
            Assert.AreEqual("testmod", resultModid3);
            Assert.AreEqual("testmod", resultModid4);
            Assert.AreEqual(null, resultModid5);
        }

        [TestMethod]
        public void GetModnameFromPath()
        {
            string path = @"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod\src\main\resources";
            string path1 = @"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod\src\main\resources\assets\testmod";
            string path2 = @"C:\Dev\ForgeModGenerator\ForgeModGenerator\mods\TestMod";
            string path3 = @"\ForgeModGenerator\ForgeModGenerator\mods\TestMod";
            string path4 = "C:\\Dev\\ForgeModGenerator\\ForgeModGenerator\\mods\\TestMod\\src\\main\\resources\\assets\\testmod\\sounds\\test.ogg";

            string resultModid = Models.Mod.GetModnameFromPath(path);
            string resultModid1 = Models.Mod.GetModnameFromPath(path1);
            string resultModid2 = Models.Mod.GetModnameFromPath(path2);
            string resultModid3 = Models.Mod.GetModnameFromPath(path3);
            string resultModid4 = Models.Mod.GetModnameFromPath(path4);

            Assert.AreEqual("TestMod", resultModid);
            Assert.AreEqual("TestMod", resultModid1);
            Assert.AreEqual("TestMod", resultModid2);
            Assert.AreEqual(null, resultModid3);
            Assert.AreEqual("TestMod", resultModid4);
        }
    }
}
