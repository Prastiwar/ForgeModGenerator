using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using ForgeModGenerator.Utility;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace ForgeModGenerator.Tests
{
    [TestClass]
    public class ModGeneratorTests
    {
        public ModGeneratorTests() => SourceCodeLocator.Initialize(new MemoryCache(new MemoryCacheOptions()));

        [TestMethod]
        public void TestClassLocator()
        {
            ClassLocator locator = new ClassLocator("com.testorg.testmod.someFolder.SomeClassName");
            Assert.AreEqual("SomeClassName", locator.ClassName);
            Assert.AreEqual("someFolder.SomeClassName", locator.ImportRelativeName);
            Assert.AreEqual("someFolder/SomeClassName.java", locator.RelativePath);

            string sourcePath = ModPaths.SourceCodeRootFolder("TestMod", "testorg");
            string armorBasePath = Path.Combine(sourcePath, SourceCodeLocator.ArmorBase("TestMod", "testorg").RelativePath);
            string armorBaseFileName = Path.GetFileNameWithoutExtension(armorBasePath);
            Assert.AreEqual(armorBaseFileName, SourceCodeLocator.ArmorBase("TestMod", "testorg").ClassName);
            Assert.IsTrue(IOHelper.IsSubPathOf(locator.FullPath, sourcePath));
        }

        [TestMethod]
        public void ExportMcModInfo()
        {
            McModInfo modInfo = new McModInfo() {
                Name = IntegratedUnitTests.TestModName,
                Modid = IntegratedUnitTests.TestModModid,
                Description = "Some description",
                Version = "1.0",
                McVersion = "12.2",
                Url = "smth",
                UpdateUrl = "smth",
                Credits = "me",
                LogoFile = "there",
                AuthorList = new ObservableCollection<string>() { "I'm Author" },
                Screenshots = new ObservableCollection<string>() { "Some/screenshot" },
                Dependencies = new ObservableCollection<string>() { "OtherMod" },
            };
            string json = JsonConvert.SerializeObject(modInfo, Formatting.Indented, new McModInfoJsonConverter());
            Assert.IsTrue(json.Contains("[\n"), json);
            Assert.IsTrue(json.Contains("\n]"), json);
            Assert.IsTrue(json.Contains($"\"modid\": \"{IntegratedUnitTests.TestModModid}\""), json);
            Assert.IsTrue(json.Contains($"\"name\": \"{IntegratedUnitTests.TestModName}\""), json);
            Assert.IsTrue(json.Contains("\"description\": \"Some description\""), json);
            Assert.IsTrue(json.Contains("\"version\": \"1.0\""), json);
            Assert.IsTrue(json.Contains("\"mcVersion\": \"12.2\""), json);
            Assert.IsTrue(json.Contains("\"url\": \"smth\""), json);
            Assert.IsTrue(json.Contains("\"updateUrl\": \"smth\""), json);
            Assert.IsTrue(json.Contains("\"credits\": \"me\""), json);
            Assert.IsTrue(json.Contains("\"logoFile\": \"there\""), json);
            Assert.IsTrue(json.Contains("\"authorList\":"), json);
            Assert.IsTrue(json.Contains("\"I'm Author\""), json);
            Assert.IsTrue(json.Contains("\"screenshots\":"), json);
            Assert.IsTrue(json.Contains("\"Some/screenshot\""), json);
            Assert.IsTrue(json.Contains("\"dependencies\":"), json);
            Assert.IsTrue(json.Contains("\"OtherMod\""), json);

            modInfo.AuthorList = null;
            modInfo.Screenshots = null;
            modInfo.Dependencies = null;
            json = JsonConvert.SerializeObject(modInfo, Formatting.Indented, new McModInfoJsonConverter());

            Assert.IsTrue(json.Contains("\"authorList\": []"), json);
            Assert.IsTrue(json.Contains("\"screenshots\": []"), json);
            Assert.IsTrue(json.Contains("\"dependencies\": []"), json);
        }

        [TestMethod]
        public void ImportMcModInfo()
        {
            string json = "[{\"modid\":\"" + IntegratedUnitTests.TestModModid + "\",\"name\":\"" + IntegratedUnitTests.TestModName + "\",\"description\":\"Some description\",\"version\":\"1.0\",\"mcversion\":\"12.2\",\"url\":\"some url\",\"updateUrl\":\"some updateurl\",\"credits\":\"me\",\"logoFile\":\"some/logofile\",\"authorList\":[\"I'm Author\"],\"screenshots\":[\"some/screenshot\"],\"dependencies\":[\"OtherMod\"]}]";
            McModInfo modInfo = JsonConvert.DeserializeObject<McModInfo>(json, new McModInfoJsonConverter());
            Assert.AreEqual(IntegratedUnitTests.TestModName, modInfo.Name);
            Assert.AreEqual(IntegratedUnitTests.TestModModid, modInfo.Modid);
            Assert.AreEqual("Some description", modInfo.Description);
            Assert.AreEqual("1.0", modInfo.Version);
            Assert.AreEqual("12.2", modInfo.McVersion);
            Assert.AreEqual("some url", modInfo.Url);
            Assert.AreEqual("some updateurl", modInfo.UpdateUrl);
            Assert.AreEqual("me", modInfo.Credits);
            Assert.AreEqual("some/logofile", modInfo.LogoFile);
            Assert.IsNotNull(modInfo.AuthorList);
            Assert.IsNotNull(modInfo.Screenshots);
            Assert.IsNotNull(modInfo.Dependencies);
            Assert.AreEqual(1, modInfo.AuthorList.Count);
            Assert.AreEqual(1, modInfo.Screenshots.Count);
            Assert.AreEqual(1, modInfo.Dependencies.Count);
            Assert.AreEqual("I'm Author", modInfo.AuthorList[0]);
            Assert.AreEqual("some/screenshot", modInfo.Screenshots[0]);
            Assert.AreEqual("OtherMod", modInfo.Dependencies[0]);

            modInfo = JsonConvert.DeserializeObject<McModInfo>("{}", new McModInfoJsonConverter());
            Assert.IsNull(modInfo);
        }

        [TestMethod]
        public void ExportMod()
        {
            McMod mcMod = new McMod(
                new McModInfo {
                    Modid = IntegratedUnitTests.TestModModid,
                    Name = IntegratedUnitTests.TestModName,
                    Description = "This is example mod",
                    Version = "1.0",
                    McVersion = "12.2",
                    Url = "some url",
                    UpdateUrl = "some updateurl",
                    Credits = "For contributors of ForgeModGenerator",
                    LogoFile = "some/logofile",
                    AuthorList = new ObservableCollection<string>() { "I'm Author" },
                    Screenshots = new ObservableCollection<string>() { "some/screenshot" },
                    Dependencies = new ObservableCollection<string>() { "OtherMod" },
                },
                "testorg",
                new ForgeVersion("forge - 1.12.2 - 14.23.5.2772", "C:/Dev/ForgeModGenerator/ForgeModGenerator/forgeversions/forge - 1.12.2 - 14.23.5.2772 - mdk.zip"),
                ModSide.ClientServer,
                LaunchSetup.Client,
                WorkspaceSetup.NONE
            );
            JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            settings.Converters.Add(new ModJsonConverter());
            settings.Converters.Add(new McModInfoJsonConverter());
            settings.Converters.Add(new ForgeVersionJsonConverter());
            string json = JsonConvert.SerializeObject(mcMod, settings);
            Assert.IsTrue(json.Contains("\"Organization\":\"testorg\""), json);
            Assert.IsTrue(json.Contains("\"Name\":\"forge - 1.12.2 - 14.23.5.2772\""), json);
            Assert.IsTrue(json.Contains("\"ZipPath\":\"C:/Dev/ForgeModGenerator/ForgeModGenerator/forgeversions/forge - 1.12.2 - 14.23.5.2772 - mdk.zip\""), json);
            Assert.IsTrue(json.Contains("\"LaunchSetup\":0"), json);
            Assert.IsTrue(json.Contains("\"Name\":\"Empty\""), json);
            Assert.IsTrue(json.Contains("\"Side\":0"), json);
        }

        [TestMethod]
        public void ImportMod()
        {
            string json = "{\"Organization\":\"testorg\",\"ForgeVersion\":{\"Name\":\"forge - 1.12.2 - 14.23.5.2772\",\"ZipPath\":\"C:/Dev/ForgeModGenerator/ForgeModGenerator/forgeversions/forge - 1.12.2 - 14.23.5.2772 - mdk.zip\"},\"LaunchSetup\":0,\"Side\":0,\"WorkspaceSetup\":{\"$type\":\"ForgeModGenerator.Models.EmptyWorkspace, ForgeModGenerator.Core\",\"Name\":\"Empty\"},\"CachedName\":\"TestMod\"}";
            JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            settings.Converters.Add(new ModJsonConverter());
            settings.Converters.Add(new McModInfoJsonConverter());
            settings.Converters.Add(new ForgeVersionJsonConverter());
            McMod mcMod = JsonConvert.DeserializeObject<McMod>(json, settings);

            Assert.IsNotNull(mcMod.ForgeVersion);
            Assert.IsNotNull(mcMod.LaunchSetup);
            Assert.IsNotNull(mcMod.WorkspaceSetup);
            Assert.IsNotNull(mcMod.ModInfo.AuthorList);
            Assert.IsNotNull(mcMod.ModInfo.Screenshots);
            Assert.IsNotNull(mcMod.ModInfo.Dependencies);

            Assert.AreEqual(IntegratedUnitTests.TestModModid, mcMod.Modid);
            Assert.AreEqual(IntegratedUnitTests.TestModName, mcMod.Name);
            Assert.AreEqual("testorg", mcMod.Organization);
            Assert.AreEqual(ModSide.ClientServer, mcMod.Side);
            Assert.AreEqual("This is example mod", mcMod.ModInfo.Description);
            Assert.AreEqual("1.0", mcMod.ModInfo.Version);
            Assert.AreEqual("12.2", mcMod.ModInfo.McVersion);
            Assert.AreEqual("some url", mcMod.ModInfo.Url);
            Assert.AreEqual("some updateurl", mcMod.ModInfo.UpdateUrl);
            Assert.AreEqual("For contributors of ForgeModGenerator", mcMod.ModInfo.Credits);
            Assert.AreEqual("some/logofile", mcMod.ModInfo.LogoFile);

            Assert.AreEqual(1, mcMod.ModInfo.AuthorList.Count);
            Assert.AreEqual(1, mcMod.ModInfo.Screenshots.Count);
            Assert.AreEqual(1, mcMod.ModInfo.Dependencies.Count);
            Assert.AreEqual("I'm Author", mcMod.ModInfo.AuthorList[0]);
            Assert.AreEqual("some/screenshot", mcMod.ModInfo.Screenshots[0]);
            Assert.AreEqual("OtherMod", mcMod.ModInfo.Dependencies[0]);
            Assert.AreEqual("forge - 1.12.2 - 14.23.5.2772", mcMod.ForgeVersion.Name);
            Assert.AreEqual("C:/Dev/ForgeModGenerator/ForgeModGenerator/forgeversions/forge - 1.12.2 - 14.23.5.2772 - mdk.zip", mcMod.ForgeVersion.ZipPath);
            Assert.IsTrue(mcMod.LaunchSetup == LaunchSetup.Client);
            Assert.AreEqual("Empty", mcMod.WorkspaceSetup.Name);

            mcMod = JsonConvert.DeserializeObject<McMod>("{}", settings);
            Assert.IsNull(mcMod);
        }
    }
}
