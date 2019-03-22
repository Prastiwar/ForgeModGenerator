using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.IO;

namespace ForgeModGenerator.Tests
{
    [TestClass]
    public class ModGeneratorTests
    {
        [TestMethod]
        public void TestClassLocator()
        {
            ClassLocator locator = new ClassLocator("someFolder.SomeClassName");
            Assert.AreEqual("SomeClassName", locator.ClassName);
            Assert.AreEqual("someFolder.SomeClassName", locator.ImportFullName);
            Assert.AreEqual("someFolder/SomeClassName.java", locator.RelativePath);

            string sourcePath = ModPaths.SourceCodeRootFolder("TestMod", "testorg");
            string armorBasePath = Path.Combine(sourcePath, SourceCodeLocator.ArmorBase.RelativePath);
            string armorBaseFileName = Path.GetFileNameWithoutExtension(armorBasePath);
            Assert.AreEqual(armorBaseFileName, SourceCodeLocator.ArmorBase.ClassName);
        }

        [TestMethod]
        public void ExportMcModInfo()
        {
            McModInfo modInfo = new McModInfo();
            string serializedModInfo = JsonConvert.SerializeObject(modInfo, Formatting.Indented, new McModInfoJsonConverter());
            serializedModInfo = "[\n" + serializedModInfo + "\n]";
            Assert.AreEqual("", serializedModInfo);
        }
    }
}
