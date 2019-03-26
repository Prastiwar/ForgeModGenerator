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
            // TODO: Test exporting
        }

        [TestMethod]
        public void ImportMcModInfo()
        {
            string infoTextFormat = "json";
            string fixedJson = infoTextFormat.Remove(0, 2).Remove(infoTextFormat.Length - 4, 2); // remove [\n and \n]
            McModInfo modInfo = JsonConvert.DeserializeObject<McModInfo>(fixedJson, new McModInfoJsonConverter());
            // TODO: Test importing
        }

        [TestMethod]
        public void ExportMod()
        {
            Mod mod = new Mod( new McModInfo { },
                "testorg", 
                new ForgeVersion("testForge", "forge.zip"), 
                ModSide.Server,
                new LaunchSetup(false, true), WorkspaceSetup.NONE);
            JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            settings.Converters.Add(new ModJsonConverter());
            settings.Converters.Add(new McModInfoJsonConverter());
            settings.Converters.Add(new ForgeVersionJsonConverter());
            string json = JsonConvert.SerializeObject(mod, settings);
            // TODO: Test exporting
        }

        [TestMethod]
        public void ImportMod()
        {
            string json = "{}";
            JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            settings.Converters.Add(new ModJsonConverter());
            settings.Converters.Add(new McModInfoJsonConverter());
            settings.Converters.Add(new ForgeVersionJsonConverter());
            Mod mod = JsonConvert.DeserializeObject<Mod>(json, settings);
            // TODO: Test importing
        }
    }
}
