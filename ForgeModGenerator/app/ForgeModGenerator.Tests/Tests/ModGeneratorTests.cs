using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using ForgeModGenerator.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace ForgeModGenerator.Tests
{
    [TestClass]
    public class ModGeneratorTests
    {
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
            Mod mod = new Mod(
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
                new LaunchSetup(true, true),
                WorkspaceSetup.NONE
            );
            JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            settings.Converters.Add(new ModJsonConverter());
            settings.Converters.Add(new McModInfoJsonConverter());
            settings.Converters.Add(new ForgeVersionJsonConverter());
            string json = JsonConvert.SerializeObject(mod, settings);
            Assert.IsTrue(json.Contains("\"Organization\":\"testorg\""), json);
            Assert.IsTrue(json.Contains("\"Name\":\"forge - 1.12.2 - 14.23.5.2772\""), json);
            Assert.IsTrue(json.Contains("\"ZipPath\":\"C:/Dev/ForgeModGenerator/ForgeModGenerator/forgeversions/forge - 1.12.2 - 14.23.5.2772 - mdk.zip\""), json);
            Assert.IsTrue(json.Contains("\"RunClient\":true"), json);
            Assert.IsTrue(json.Contains("\"RunServer\":true"), json);
            Assert.IsTrue(json.Contains("\"Name\":\"Empty\""), json);
            Assert.IsTrue(json.Contains("\"Side\":0"), json);
        }

        [TestMethod]
        public void ImportMod()
        {
            string json = "{\"Organization\":\"testorg\",\"ModInfo\":{\"modid\":\"testmod\",\"name\":\"TestMod\",\"description\":\"This is example mod\",\"version\":\"1.0\",\"mcVersion\":\"12.2\",\"url\":\"some url\",\"updateUrl\":\"some updateurl\",\"credits\":\"For contributors of ForgeModGenerator\",\"logoFile\":\"some/logofile\",\"authorList\":{\"$type\":\"System.Collections.ObjectModel.ObservableCollection`1[[System.String, mscorlib]], System\",\"$values\":[\"I'm Author\"]},\"screenshots\":[\"some/screenshot\"],\"dependencies\":[\"OtherMod\"]},\"ForgeVersion\":{\"Name\":\"forge - 1.12.2 - 14.23.5.2772\",\"ZipPath\":\"C:/Dev/ForgeModGenerator/ForgeModGenerator/forgeversions/forge - 1.12.2 - 14.23.5.2772 - mdk.zip\"},\"LaunchSetup\":{\"$type\":\"ForgeModGenerator.Models.LaunchSetup, ForgeModGenerator.Core\",\"RunClient\":true,\"RunServer\":true},\"Side\":0,\"WorkspaceSetup\":{\"$type\":\"ForgeModGenerator.Models.EmptyWorkspace, ForgeModGenerator.Core\",\"Name\":\"Empty\"},\"CachedName\":\"NewExampleMod\"}";
            JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            settings.Converters.Add(new ModJsonConverter());
            settings.Converters.Add(new McModInfoJsonConverter());
            settings.Converters.Add(new ForgeVersionJsonConverter());
            Mod mod = JsonConvert.DeserializeObject<Mod>(json, settings);

            Assert.IsNotNull(mod.ForgeVersion);
            Assert.IsNotNull(mod.LaunchSetup);
            Assert.IsNotNull(mod.WorkspaceSetup);
            Assert.IsNotNull(mod.ModInfo.AuthorList);
            Assert.IsNotNull(mod.ModInfo.Screenshots);
            Assert.IsNotNull(mod.ModInfo.Dependencies);

            Assert.AreEqual(IntegratedUnitTests.TestModModid, mod.Modid);
            Assert.AreEqual(IntegratedUnitTests.TestModName, mod.Name);
            Assert.AreEqual("testorg", mod.Organization);
            Assert.AreEqual(ModSide.ClientServer, mod.Side);
            Assert.AreEqual("This is example mod", mod.ModInfo.Description);
            Assert.AreEqual("1.0", mod.ModInfo.Version);
            Assert.AreEqual("12.2", mod.ModInfo.McVersion);
            Assert.AreEqual("some url", mod.ModInfo.Url);
            Assert.AreEqual("some updateurl", mod.ModInfo.UpdateUrl);
            Assert.AreEqual("For contributors of ForgeModGenerator", mod.ModInfo.Credits);
            Assert.AreEqual("some/logofile", mod.ModInfo.LogoFile);

            Assert.AreEqual(1, mod.ModInfo.AuthorList.Count);
            Assert.AreEqual(1, mod.ModInfo.Screenshots.Count);
            Assert.AreEqual(1, mod.ModInfo.Dependencies.Count);
            Assert.AreEqual("I'm Author", mod.ModInfo.AuthorList[0]);
            Assert.AreEqual("some/screenshot", mod.ModInfo.Screenshots[0]);
            Assert.AreEqual("OtherMod", mod.ModInfo.Dependencies[0]);
            Assert.AreEqual("forge - 1.12.2 - 14.23.5.2772", mod.ForgeVersion.Name);
            Assert.AreEqual("C:/Dev/ForgeModGenerator/ForgeModGenerator/forgeversions/forge - 1.12.2 - 14.23.5.2772 - mdk.zip", mod.ForgeVersion.ZipPath);
            Assert.IsTrue(mod.LaunchSetup.RunClient);
            Assert.IsTrue(mod.LaunchSetup.RunServer);
            Assert.AreEqual("Empty", mod.WorkspaceSetup.Name);

            mod = JsonConvert.DeserializeObject<Mod>("{}", settings);
            Assert.IsNull(mod);
        }
    }
}
