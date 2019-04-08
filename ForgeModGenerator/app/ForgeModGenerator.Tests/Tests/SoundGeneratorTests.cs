using ForgeModGenerator.SoundGenerator.Converters;
using ForgeModGenerator.SoundGenerator.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace ForgeModGenerator.Tests
{
    [TestClass]
    public class SoundGeneratorTests : IntegratedUnitTests
    {
        private readonly string soundsPath = ModPaths.SoundsFolder(TestModName, TestModModid);
        private readonly string deserializeJson = "{\"sounds\":{\"replace\":false,\"subtitle\":\"\",\"sounds\":[{\"name\":\"" + TestModModid + ":test\",\"volume\":1.0,\"pitch\":1.0,\"weight\":1,\"stream\":false,\"attenuation_distance\":0,\"preload\":false,\"type\":\"file\"},{\"name\":\"" + TestModModid + ":testCopy2\",\"volume\":0.5,\"pitch\":2.0,\"weight\":2,\"stream\":true,\"attenuation_distance\":1,\"preload\":true,\"type\":\"event\"}]},\"entity.vatras\":{\"replace\":true,\"subtitle\":\"Some subtitle2\",\"sounds\":[{\"name\":\"" + TestModModid + ":entity/vatras/greet\",\"volume\":1.0,\"pitch\":1.0,\"weight\":1,\"stream\":true,\"attenuation_distance\":0,\"preload\":true,\"type\":\"event\"}]}}";
        private readonly SoundEventConverter soundEventConverter = new SoundEventConverter();
        private readonly SoundConverter soundConverter = new SoundConverter();
        private readonly SoundCollectionConverter soundEventCollectionConverter = new SoundCollectionConverter(TestModName, TestModModid);

        private Sound CreateSoundTest() =>
            new Sound(TestModModid, Path.Combine(soundsPath, "test.ogg")) {
                Volume = 1.0f,
                Pitch = 1.0f,
                Weight = 1,
                Stream = false,
                AttenuationDistance = 0,
                Preload = false,
                Type = Sound.SoundType.file
            };

        private Sound CreateSoundTestCopy2() =>
            new Sound(TestModModid, Path.Combine(soundsPath, "testCopy2.ogg")) {
                Volume = 0.5f,
                Pitch = 2.0f,
                Weight = 2,
                Stream = true,
                AttenuationDistance = 1,
                Preload = true,
                Type = Sound.SoundType.@event
            };

        private Sound CreateSoundGreet() =>
            new Sound(TestModModid, Path.Combine(soundsPath, "entity", "vatras", "greet.ogg")) {
                Volume = 1.0f,
                Pitch = 1.0f,
                Weight = 1,
                Stream = true,
                AttenuationDistance = 0,
                Preload = true,
                Type = Sound.SoundType.@event
            };

        // ----------------------------------------------------------- Serialization

        [TestMethod]
        public void SoundSerialize()
        {
            Sound sound = CreateSoundTestCopy2();
            sound.FormatName();
            string json = JsonConvert.SerializeObject(sound, soundConverter);
            Assert.IsTrue(json.Contains($"\"name\":\"{TestModModid}:testCopy2\""), json);
            Assert.IsTrue(json.Contains("\"volume\":0.5"), json);
            Assert.IsTrue(json.Contains("\"pitch\":2.0"), json);
            Assert.IsTrue(json.Contains("\"weight\":2"), json);
            Assert.IsTrue(json.Contains("\"stream\":true"), json);
            Assert.IsTrue(json.Contains("\"attenuation_distance\":1"), json);
            Assert.IsTrue(json.Contains("\"preload\":true"), json);
            Assert.IsTrue(json.Contains("\"type\":\"event\""), json);
        }

        [TestMethod]
        public void SoundEventSerialize()
        {
            Collection<Sound> sounds = new Collection<Sound>() {
                CreateSoundTest(), CreateSoundTestCopy2()
            };
            SoundEvent soundEvent = new SoundEvent(soundsPath, sounds) {
                Subtitle = "Some subtitle",
                Replace = false
            };
            string json = JsonConvert.SerializeObject(soundEvent, soundEventConverter);
            Assert.IsTrue(json.Contains("\"sounds\":{"), json);
            Assert.IsTrue(json.Contains("\"replace\":false"), json);
            Assert.IsTrue(json.Contains("\"subtitle\":\"Some subtitle\""), json);
            Assert.IsTrue(json.Contains("\"sounds\":[{"), json);

            Assert.IsTrue(json.Contains($"\"name\":\"{TestModModid}:test\""), json);
            Assert.IsTrue(json.Contains("\"volume\":1.0"), json);
            Assert.IsTrue(json.Contains("\"pitch\":1.0"), json);
            Assert.IsTrue(json.Contains("\"weight\":1"), json);
            Assert.IsTrue(json.Contains("\"stream\":false"), json);
            Assert.IsTrue(json.Contains("\"attenuation_distance\":0"), json);
            Assert.IsTrue(json.Contains("\"preload\":false"), json);
            Assert.IsTrue(json.Contains("\"type\":\"file\""), json);

            Assert.IsTrue(json.Contains($"\"name\":\"{TestModModid}:testCopy2\""), json);
            Assert.IsTrue(json.Contains("\"volume\":0.5"), json);
            Assert.IsTrue(json.Contains("\"pitch\":2.0"), json);
            Assert.IsTrue(json.Contains("\"weight\":2"), json);
            Assert.IsTrue(json.Contains("\"stream\":true"), json);
            Assert.IsTrue(json.Contains("\"attenuation_distance\":1"), json);
            Assert.IsTrue(json.Contains("\"preload\":true"), json);
            Assert.IsTrue(json.Contains("\"type\":\"event\""), json);
        }

        [TestMethod]
        public void SoundEventListSerialize()
        {
            Collection<Sound> sounds1 = new Collection<Sound>() {
                CreateSoundTest(), CreateSoundTestCopy2()
            };
            Collection<Sound> sounds2 = new Collection<Sound>() {
                CreateSoundGreet()
            };
            List<SoundEvent> soundEvent = new List<SoundEvent>(){
                new SoundEvent(soundsPath, sounds1) {
                    Subtitle = "",
                    Replace = false
                },
                new SoundEvent(Path.Combine(soundsPath, "entity", "vatras"), sounds2) {
                    Subtitle = "Some subtitle2",
                    Replace = true
                }
            };
            string json = JsonConvert.SerializeObject(soundEvent, soundEventCollectionConverter);
            Assert.IsTrue(json.Contains("\"sounds\":{"), json);
            Assert.IsTrue(json.Contains("\"replace\":false"), json);
            Assert.IsTrue(json.Contains("\"subtitle\":\"\""), json);
            Assert.IsTrue(json.Contains("\"sounds\":[{"), json);

            Assert.IsTrue(json.Contains($"\"name\":\"{TestModModid}:test\""), json);
            Assert.IsTrue(json.Contains("\"volume\":1.0"), json);
            Assert.IsTrue(json.Contains("\"pitch\":1.0"), json);
            Assert.IsTrue(json.Contains("\"weight\":1"), json);
            Assert.IsTrue(json.Contains("\"stream\":false"), json);
            Assert.IsTrue(json.Contains("\"attenuation_distance\":0"), json);
            Assert.IsTrue(json.Contains("\"preload\":false"), json);
            Assert.IsTrue(json.Contains("\"type\":\"file\""), json);

            Assert.IsTrue(json.Contains($"\"name\":\"{TestModModid}:testCopy2\""), json);
            Assert.IsTrue(json.Contains("\"volume\":0.5"), json);
            Assert.IsTrue(json.Contains("\"pitch\":2.0"), json);
            Assert.IsTrue(json.Contains("\"weight\":2"), json);
            Assert.IsTrue(json.Contains("\"stream\":true"), json);
            Assert.IsTrue(json.Contains("\"attenuation_distance\":1"), json);
            Assert.IsTrue(json.Contains("\"preload\":true"), json);
            Assert.IsTrue(json.Contains("\"type\":\"event\""), json);
            
            Assert.IsTrue(json.Contains("\"entity.vatras.greet\":{"), json);
            Assert.IsTrue(json.Contains("\"replace\":true"), json);
            Assert.IsTrue(json.Contains("\"subtitle\":\"Some subtitle2\""), json);
            Assert.IsTrue(json.Contains($"\"name\":\"{TestModModid}:entity/vatras/greet\""), json);
        }


        // ----------------------------------------------------------- Deserialization

        [TestMethod]
        public void SoundDeserialize()
        {
            string json = "{\"name\":\"" + TestModModid + ":testCopy2\",\"volume\":0.5,\"pitch\":2.0,\"weight\":2,\"stream\":true,\"attenuation_distance\":1,\"preload\":true,\"type\":\"event\"}";
            Sound sound = JsonConvert.DeserializeObject<Sound>(json, soundConverter);
            Assert.IsNotNull(sound);
            Assert.AreEqual($"{TestModModid}:testCopy2", sound.Name);
            Assert.AreEqual(0.5, sound.Volume);
            Assert.AreEqual(2.0, sound.Pitch);
            Assert.AreEqual(2, sound.Weight);
            Assert.AreEqual(true, sound.Stream);
            Assert.AreEqual(1, sound.AttenuationDistance);
            Assert.AreEqual(true, sound.Preload);
            Assert.AreEqual(Sound.SoundType.@event, sound.Type);
        }

        [TestMethod]
        public void SoundEventDeserialize()
        {
            string json = "{\"replace\":false,\"subtitle\":\"Some subtitle\",\"sounds\":[{\"name\":\"" + TestModModid + ":test\",\"volume\":1.0,\"pitch\":1.0,\"weight\":1,\"stream\":false,\"attenuation_distance\":0,\"preload\":false,\"type\":\"file\"},{\"name\":\"" + TestModModid + ":testCopy2\",\"volume\":0.5,\"pitch\":2.0,\"weight\":2,\"stream\":true,\"attenuation_distance\":1,\"preload\":true,\"type\":\"event\"}]}";
            SoundEvent soundEvent = JsonConvert.DeserializeObject<SoundEvent>(json, soundEventConverter);
            Assert.IsNotNull(soundEvent);
            Assert.AreEqual(false, soundEvent.Replace);
            Assert.AreEqual("Some subtitle", soundEvent.Subtitle);
            Assert.IsNotNull(soundEvent.Files);
            Assert.AreEqual(2, soundEvent.Files.Count);
            Assert.IsNotNull(soundEvent.Files[0]);
            Assert.IsNotNull(soundEvent.Files[1]);
            Assert.AreEqual($"{TestModModid}:test", soundEvent.Files[0].Name);
            Assert.AreEqual(1.0, soundEvent.Files[0].Volume);
            Assert.AreEqual(1.0, soundEvent.Files[0].Pitch);
            Assert.AreEqual(1, soundEvent.Files[0].Weight);
            Assert.AreEqual(false, soundEvent.Files[0].Stream);
            Assert.AreEqual(0, soundEvent.Files[0].AttenuationDistance);
            Assert.AreEqual(false, soundEvent.Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.file, soundEvent.Files[0].Type);
            Assert.AreEqual($"{TestModModid}:testCopy2", soundEvent.Files[1].Name);
            Assert.AreEqual(0.5, soundEvent.Files[1].Volume);
            Assert.AreEqual(2.0, soundEvent.Files[1].Pitch);
            Assert.AreEqual(2, soundEvent.Files[1].Weight);
            Assert.AreEqual(true, soundEvent.Files[1].Stream);
            Assert.AreEqual(1, soundEvent.Files[1].AttenuationDistance);
            Assert.AreEqual(true, soundEvent.Files[1].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvent.Files[1].Type);
        }

        [TestMethod]
        public void SoundEventListDeserialize()
        {
            List<SoundEvent> soundEvents = JsonConvert.DeserializeObject<List<SoundEvent>>(deserializeJson, soundEventCollectionConverter);
            Assert.IsNotNull(soundEvents);
            Assert.AreEqual(2, soundEvents.Count);
            Assert.AreEqual("sounds", soundEvents[0].EventName);
            Assert.AreEqual("entity.vatras", soundEvents[1].EventName);
            Assert.IsNotNull(soundEvents[0].Files);
            Assert.IsNotNull(soundEvents[1].Files);
            Assert.AreEqual(2, soundEvents[0].Files.Count);
            Assert.AreEqual(1, soundEvents[1].Files.Count);
            Assert.AreEqual(false, soundEvents[0].Replace);
            Assert.AreEqual(true, soundEvents[1].Replace);
            Assert.AreEqual("", soundEvents[0].Subtitle);
            Assert.AreEqual("Some subtitle2", soundEvents[1].Subtitle);
            Assert.IsNotNull(soundEvents[0].Files[0]);
            Assert.IsNotNull(soundEvents[0].Files[1]);
            Assert.IsNotNull(soundEvents[1].Files[0]);

            Assert.AreEqual($"{TestModModid}:test", soundEvents[0].Files[0].Name);
            Assert.AreEqual(1.0, soundEvents[0].Files[0].Volume);
            Assert.AreEqual(1.0, soundEvents[0].Files[0].Pitch);
            Assert.AreEqual(1, soundEvents[0].Files[0].Weight);
            Assert.AreEqual(false, soundEvents[0].Files[0].Stream);
            Assert.AreEqual(0, soundEvents[0].Files[0].AttenuationDistance);
            Assert.AreEqual(false, soundEvents[0].Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.file, soundEvents[0].Files[0].Type);

            Assert.AreEqual($"{TestModModid}:testCopy2", soundEvents[0].Files[1].Name);
            Assert.AreEqual(0.5, soundEvents[0].Files[1].Volume);
            Assert.AreEqual(2.0, soundEvents[0].Files[1].Pitch);
            Assert.AreEqual(2, soundEvents[0].Files[1].Weight);
            Assert.AreEqual(true, soundEvents[0].Files[1].Stream);
            Assert.AreEqual(1, soundEvents[0].Files[1].AttenuationDistance);
            Assert.AreEqual(true, soundEvents[0].Files[1].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvents[0].Files[1].Type);

            Assert.AreEqual($"{TestModModid}:entity/vatras/greet", soundEvents[1].Files[0].Name);
            Assert.AreEqual(1.0, soundEvents[1].Files[0].Volume);
            Assert.AreEqual(1.0, soundEvents[1].Files[0].Pitch);
            Assert.AreEqual(1, soundEvents[1].Files[0].Weight);
            Assert.AreEqual(true, soundEvents[1].Files[0].Stream);
            Assert.AreEqual(0, soundEvents[1].Files[0].AttenuationDistance);
            Assert.AreEqual(true, soundEvents[1].Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvents[1].Files[0].Type);
        }

        [TestMethod]
        public void SoundEventCollectionDeserialize()
        {
            Collection<SoundEvent> soundEvents = JsonConvert.DeserializeObject<Collection<SoundEvent>>(deserializeJson, soundEventCollectionConverter);
            Assert.IsNotNull(soundEvents);
            Assert.AreEqual(2, soundEvents.Count);
            Assert.AreEqual("sounds", soundEvents[0].EventName);
            Assert.AreEqual("entity.vatras", soundEvents[1].EventName);
            Assert.IsNotNull(soundEvents[0].Files);
            Assert.IsNotNull(soundEvents[1].Files);
            Assert.AreEqual(2, soundEvents[0].Files.Count);
            Assert.AreEqual(1, soundEvents[1].Files.Count);
            Assert.AreEqual(false, soundEvents[0].Replace);
            Assert.AreEqual(true, soundEvents[1].Replace);
            Assert.AreEqual("", soundEvents[0].Subtitle);
            Assert.AreEqual("Some subtitle2", soundEvents[1].Subtitle);
            Assert.IsNotNull(soundEvents[0].Files[0]);
            Assert.IsNotNull(soundEvents[0].Files[1]);
            Assert.IsNotNull(soundEvents[1].Files[0]);

            Assert.AreEqual($"{TestModModid}:test", soundEvents[0].Files[0].Name);
            Assert.AreEqual(1.0, soundEvents[0].Files[0].Volume);
            Assert.AreEqual(1.0, soundEvents[0].Files[0].Pitch);
            Assert.AreEqual(1, soundEvents[0].Files[0].Weight);
            Assert.AreEqual(false, soundEvents[0].Files[0].Stream);
            Assert.AreEqual(0, soundEvents[0].Files[0].AttenuationDistance);
            Assert.AreEqual(false, soundEvents[0].Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.file, soundEvents[0].Files[0].Type);

            Assert.AreEqual($"{TestModModid}:testCopy2", soundEvents[0].Files[1].Name);
            Assert.AreEqual(0.5, soundEvents[0].Files[1].Volume);
            Assert.AreEqual(2.0, soundEvents[0].Files[1].Pitch);
            Assert.AreEqual(2, soundEvents[0].Files[1].Weight);
            Assert.AreEqual(true, soundEvents[0].Files[1].Stream);
            Assert.AreEqual(1, soundEvents[0].Files[1].AttenuationDistance);
            Assert.AreEqual(true, soundEvents[0].Files[1].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvents[0].Files[1].Type);

            Assert.AreEqual($"{TestModModid}:entity/vatras/greet", soundEvents[1].Files[0].Name);
            Assert.AreEqual(1.0, soundEvents[1].Files[0].Volume);
            Assert.AreEqual(1.0, soundEvents[1].Files[0].Pitch);
            Assert.AreEqual(1, soundEvents[1].Files[0].Weight);
            Assert.AreEqual(true, soundEvents[1].Files[0].Stream);
            Assert.AreEqual(0, soundEvents[1].Files[0].AttenuationDistance);
            Assert.AreEqual(true, soundEvents[1].Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvents[1].Files[0].Type);
        }

        [TestMethod]
        public void SoundEventObservableCollectionDeserialize()
        {
            ObservableCollection<SoundEvent> soundEvents = JsonConvert.DeserializeObject<ObservableCollection<SoundEvent>>(deserializeJson, soundEventCollectionConverter);
            Assert.IsNotNull(soundEvents);
            Assert.AreEqual(2, soundEvents.Count);
            Assert.AreEqual("sounds", soundEvents[0].EventName);
            Assert.AreEqual("entity.vatras", soundEvents[1].EventName);
            Assert.IsNotNull(soundEvents[0].Files);
            Assert.IsNotNull(soundEvents[1].Files);
            Assert.AreEqual(2, soundEvents[0].Files.Count);
            Assert.AreEqual(1, soundEvents[1].Files.Count);
            Assert.AreEqual(false, soundEvents[0].Replace);
            Assert.AreEqual(true, soundEvents[1].Replace);
            Assert.AreEqual("", soundEvents[0].Subtitle);
            Assert.AreEqual("Some subtitle2", soundEvents[1].Subtitle);
            Assert.IsNotNull(soundEvents[0].Files[0]);
            Assert.IsNotNull(soundEvents[0].Files[1]);
            Assert.IsNotNull(soundEvents[1].Files[0]);

            Assert.AreEqual($"{TestModModid}:test", soundEvents[0].Files[0].Name);
            Assert.AreEqual(1.0, soundEvents[0].Files[0].Volume);
            Assert.AreEqual(1.0, soundEvents[0].Files[0].Pitch);
            Assert.AreEqual(1, soundEvents[0].Files[0].Weight);
            Assert.AreEqual(false, soundEvents[0].Files[0].Stream);
            Assert.AreEqual(0, soundEvents[0].Files[0].AttenuationDistance);
            Assert.AreEqual(false, soundEvents[0].Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.file, soundEvents[0].Files[0].Type);

            Assert.AreEqual($"{TestModModid}:testCopy2", soundEvents[0].Files[1].Name);
            Assert.AreEqual(0.5, soundEvents[0].Files[1].Volume);
            Assert.AreEqual(2.0, soundEvents[0].Files[1].Pitch);
            Assert.AreEqual(2, soundEvents[0].Files[1].Weight);
            Assert.AreEqual(true, soundEvents[0].Files[1].Stream);
            Assert.AreEqual(1, soundEvents[0].Files[1].AttenuationDistance);
            Assert.AreEqual(true, soundEvents[0].Files[1].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvents[0].Files[1].Type);

            Assert.AreEqual($"{TestModModid}:entity/vatras/greet", soundEvents[1].Files[0].Name);
            Assert.AreEqual(1.0, soundEvents[1].Files[0].Volume);
            Assert.AreEqual(1.0, soundEvents[1].Files[0].Pitch);
            Assert.AreEqual(1, soundEvents[1].Files[0].Weight);
            Assert.AreEqual(true, soundEvents[1].Files[0].Stream);
            Assert.AreEqual(0, soundEvents[1].Files[0].AttenuationDistance);
            Assert.AreEqual(true, soundEvents[1].Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvents[1].Files[0].Type);
        }
    }
}
