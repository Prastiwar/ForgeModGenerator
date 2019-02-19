using ForgeModGenerator.Converters;
using ForgeModGenerator.SoundGenerator.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.Tests
{
    [TestClass]
    public class SoundGeneratorTests
    {
        [TestMethod]
        public void SoundJsonConvert()
        {
            string json = "{\"name\":\"craftpolis:testCopy2\",\"volume\":0.5,\"pitch\":2.0,\"weight\":2,\"stream\":true,\"attenuation_distance\":1,\"preload\":true,\"type\":\"event\"}";
            SoundConverter converter = new SoundConverter();
            Sound sound = JsonConvert.DeserializeObject<Sound>(json, converter);
            Assert.IsNotNull(sound);
            Assert.AreEqual("craftpolis:testCopy2", sound.Name);
            Assert.AreEqual(0.5, sound.Volume);
            Assert.AreEqual(2.0, sound.Pitch);
            Assert.AreEqual(2, sound.Weight);
            Assert.AreEqual(true, sound.Stream);
            Assert.AreEqual(1, sound.AttenuationDistance);
            Assert.AreEqual(true, sound.Preload);
            Assert.AreEqual(Sound.SoundType.@event, sound.Type);
        }

        [TestMethod]
        public void SoundEventJsonConvert()
        {
            string json = "{\"replace\":false,\"subtitle\":\"Some subtitle\",\"sounds\":[{\"name\":\"craftpolis:test\",\"volume\":1.0,\"pitch\":1.0,\"weight\":1,\"stream\":false,\"attenuation_distance\":0,\"preload\":false,\"type\":\"file\"},{\"name\":\"craftpolis:testCopy2\",\"volume\":0.5,\"pitch\":2.0,\"weight\":2,\"stream\":true,\"attenuation_distance\":1,\"preload\":true,\"type\":\"event\"}]}";
            SoundEventConverter converter = new SoundEventConverter();
            SoundEvent soundEvent = JsonConvert.DeserializeObject<SoundEvent>(json, converter);
            Assert.IsNotNull(soundEvent);
            Assert.AreEqual(false, soundEvent.Replace);
            Assert.AreEqual("Some subtitle", soundEvent.Subtitle);
            Assert.IsNotNull(soundEvent.Files);
            Assert.AreEqual(2, soundEvent.Files.Count);
            Assert.IsNotNull(soundEvent.Files[0]);
            Assert.IsNotNull(soundEvent.Files[1]);
            Assert.AreEqual("craftpolis:test", soundEvent.Files[0].Name);
            Assert.AreEqual(1.0, soundEvent.Files[0].Volume);
            Assert.AreEqual(1.0, soundEvent.Files[0].Pitch);
            Assert.AreEqual(1, soundEvent.Files[0].Weight);
            Assert.AreEqual(false, soundEvent.Files[0].Stream);
            Assert.AreEqual(0, soundEvent.Files[0].AttenuationDistance);
            Assert.AreEqual(false, soundEvent.Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.file, soundEvent.Files[0].Type);
            Assert.AreEqual("craftpolis:testCopy2", soundEvent.Files[1].Name);
            Assert.AreEqual(0.5, soundEvent.Files[1].Volume);
            Assert.AreEqual(2.0, soundEvent.Files[1].Pitch);
            Assert.AreEqual(2, soundEvent.Files[1].Weight);
            Assert.AreEqual(true, soundEvent.Files[1].Stream);
            Assert.AreEqual(1, soundEvent.Files[1].AttenuationDistance);
            Assert.AreEqual(true, soundEvent.Files[1].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvent.Files[1].Type);
        }

        [TestMethod]
        public void SoundEventsJsonConvertList()
        {
            string json = "{\"sounds\":{\"replace\":false,\"subtitle\":\"\",\"sounds\":[{\"name\":\"craftpolis:test\",\"volume\":1.0,\"pitch\":1.0,\"weight\":1,\"stream\":false,\"attenuation_distance\":0,\"preload\":false,\"type\":\"file\"},{\"name\":\"craftpolis:testCopy2\",\"volume\":0.5,\"pitch\":2.0,\"weight\":2,\"stream\":true,\"attenuation_distance\":1,\"preload\":true,\"type\":\"file\"}]},\"entity.vatras\":{\"replace\":true,\"subtitle\":\"Some subtitle2\",\"sounds\":[{\"name\":\"craftpolis:entity/vatras/greet\",\"volume\":1.0,\"pitch\":1.0,\"weight\":1,\"stream\":true,\"attenuation_distance\":0,\"preload\":true,\"type\":\"event\"}]}}";
            SoundCollectionConverter converter = new SoundCollectionConverter("Craftpolis", "craftpolis");
            List<SoundEvent> soundEvents = JsonConvert.DeserializeObject<List<SoundEvent>>(json, converter);
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

            Assert.AreEqual("craftpolis:test", soundEvents[0].Files[0].Name);
            Assert.AreEqual(1.0, soundEvents[0].Files[0].Volume);
            Assert.AreEqual(1.0, soundEvents[0].Files[0].Pitch);
            Assert.AreEqual(1, soundEvents[0].Files[0].Weight);
            Assert.AreEqual(false, soundEvents[0].Files[0].Stream);
            Assert.AreEqual(0, soundEvents[0].Files[0].AttenuationDistance);
            Assert.AreEqual(false, soundEvents[0].Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.file, soundEvents[0].Files[0].Type);

            Assert.AreEqual("craftpolis:testCopy2", soundEvents[0].Files[1].Name);
            Assert.AreEqual(0.5, soundEvents[0].Files[1].Volume);
            Assert.AreEqual(2.0, soundEvents[0].Files[1].Pitch);
            Assert.AreEqual(2, soundEvents[0].Files[1].Weight);
            Assert.AreEqual(true, soundEvents[0].Files[1].Stream);
            Assert.AreEqual(1, soundEvents[0].Files[1].AttenuationDistance);
            Assert.AreEqual(true, soundEvents[0].Files[1].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvents[0].Files[1].Type);

            Assert.AreEqual("craftpolis:entity/vatras/greet", soundEvents[1].Files[0].Name);
            Assert.AreEqual(1.0, soundEvents[1].Files[0].Volume);
            Assert.AreEqual(1.0, soundEvents[1].Files[0].Pitch);
            Assert.AreEqual(1, soundEvents[1].Files[0].Weight);
            Assert.AreEqual(true, soundEvents[1].Files[0].Stream);
            Assert.AreEqual(0, soundEvents[1].Files[0].AttenuationDistance);
            Assert.AreEqual(true, soundEvents[1].Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvents[1].Files[0].Type);
        }

        [TestMethod]
        public void SoundEventsJsonConvertCollection()
        {
            string json = "{\"sounds\":{\"replace\":false,\"subtitle\":\"\",\"sounds\":[{\"name\":\"craftpolis:test\",\"volume\":1.0,\"pitch\":1.0,\"weight\":1,\"stream\":false,\"attenuation_distance\":0,\"preload\":false,\"type\":\"file\"},{\"name\":\"craftpolis:testCopy2\",\"volume\":0.5,\"pitch\":2.0,\"weight\":2,\"stream\":true,\"attenuation_distance\":1,\"preload\":true,\"type\":\"file\"}]},\"entity.vatras\":{\"replace\":true,\"subtitle\":\"Some subtitle2\",\"sounds\":[{\"name\":\"craftpolis:entity/vatras/greet\",\"volume\":1.0,\"pitch\":1.0,\"weight\":1,\"stream\":true,\"attenuation_distance\":0,\"preload\":true,\"type\":\"event\"}]}}";
            SoundCollectionConverter converter = new SoundCollectionConverter("Craftpolis", "craftpolis");
            Collection<SoundEvent> soundEvents = JsonConvert.DeserializeObject<Collection<SoundEvent>>(json, converter);
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

            Assert.AreEqual("craftpolis:test", soundEvents[0].Files[0].Name);
            Assert.AreEqual(1.0, soundEvents[0].Files[0].Volume);
            Assert.AreEqual(1.0, soundEvents[0].Files[0].Pitch);
            Assert.AreEqual(1, soundEvents[0].Files[0].Weight);
            Assert.AreEqual(false, soundEvents[0].Files[0].Stream);
            Assert.AreEqual(0, soundEvents[0].Files[0].AttenuationDistance);
            Assert.AreEqual(false, soundEvents[0].Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.file, soundEvents[0].Files[0].Type);

            Assert.AreEqual("craftpolis:testCopy2", soundEvents[0].Files[1].Name);
            Assert.AreEqual(0.5, soundEvents[0].Files[1].Volume);
            Assert.AreEqual(2.0, soundEvents[0].Files[1].Pitch);
            Assert.AreEqual(2, soundEvents[0].Files[1].Weight);
            Assert.AreEqual(true, soundEvents[0].Files[1].Stream);
            Assert.AreEqual(1, soundEvents[0].Files[1].AttenuationDistance);
            Assert.AreEqual(true, soundEvents[0].Files[1].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvents[0].Files[1].Type);

            Assert.AreEqual("craftpolis:entity/vatras/greet", soundEvents[1].Files[0].Name);
            Assert.AreEqual(1.0, soundEvents[1].Files[0].Volume);
            Assert.AreEqual(1.0, soundEvents[1].Files[0].Pitch);
            Assert.AreEqual(1, soundEvents[1].Files[0].Weight);
            Assert.AreEqual(true, soundEvents[1].Files[0].Stream);
            Assert.AreEqual(0, soundEvents[1].Files[0].AttenuationDistance);
            Assert.AreEqual(true, soundEvents[1].Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvents[1].Files[0].Type);
        }

        [TestMethod]
        public void SoundEventsJsonConvertObservableCollection()
        {
            string json = "{\"sounds\":{\"replace\":false,\"subtitle\":\"\",\"sounds\":[{\"name\":\"craftpolis:test\",\"volume\":1.0,\"pitch\":1.0,\"weight\":1,\"stream\":false,\"attenuation_distance\":0,\"preload\":false,\"type\":\"file\"},{\"name\":\"craftpolis:testCopy2\",\"volume\":0.5,\"pitch\":2.0,\"weight\":2,\"stream\":true,\"attenuation_distance\":1,\"preload\":true,\"type\":\"file\"}]},\"entity.vatras\":{\"replace\":true,\"subtitle\":\"Some subtitle2\",\"sounds\":[{\"name\":\"craftpolis:entity/vatras/greet\",\"volume\":1.0,\"pitch\":1.0,\"weight\":1,\"stream\":true,\"attenuation_distance\":0,\"preload\":true,\"type\":\"event\"}]}}";
            SoundCollectionConverter converter = new SoundCollectionConverter("Craftpolis", "craftpolis");
            ObservableCollection<SoundEvent> soundEvents = JsonConvert.DeserializeObject<ObservableCollection<SoundEvent>>(json, converter);
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

            Assert.AreEqual("craftpolis:test", soundEvents[0].Files[0].Name);
            Assert.AreEqual(1.0, soundEvents[0].Files[0].Volume);
            Assert.AreEqual(1.0, soundEvents[0].Files[0].Pitch);
            Assert.AreEqual(1, soundEvents[0].Files[0].Weight);
            Assert.AreEqual(false, soundEvents[0].Files[0].Stream);
            Assert.AreEqual(0, soundEvents[0].Files[0].AttenuationDistance);
            Assert.AreEqual(false, soundEvents[0].Files[0].Preload);
            Assert.AreEqual(Sound.SoundType.file, soundEvents[0].Files[0].Type);

            Assert.AreEqual("craftpolis:testCopy2", soundEvents[0].Files[1].Name);
            Assert.AreEqual(0.5, soundEvents[0].Files[1].Volume);
            Assert.AreEqual(2.0, soundEvents[0].Files[1].Pitch);
            Assert.AreEqual(2, soundEvents[0].Files[1].Weight);
            Assert.AreEqual(true, soundEvents[0].Files[1].Stream);
            Assert.AreEqual(1, soundEvents[0].Files[1].AttenuationDistance);
            Assert.AreEqual(true, soundEvents[0].Files[1].Preload);
            Assert.AreEqual(Sound.SoundType.@event, soundEvents[0].Files[1].Type);

            Assert.AreEqual("craftpolis:entity/vatras/greet", soundEvents[1].Files[0].Name);
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
