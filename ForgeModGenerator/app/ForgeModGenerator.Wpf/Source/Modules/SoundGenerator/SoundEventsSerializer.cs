using ForgeModGenerator.Serialization;
using ForgeModGenerator.SoundGenerator.Converters;
using ForgeModGenerator.SoundGenerator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator.Serialization
{
    public class SoundEventsSerializer : ISoundEventsSerializer
    {
        public SoundEventsSerializer() { }

        protected string ModName { get; set; }
        protected string Modid { get; set; }
        protected SoundCollectionConverter Converter { get; set; }

        public void SetModInfo(string modname, string modid)
        {
            ModName = modname ?? throw new ArgumentNullException(nameof(modname));
            Modid = modid ?? throw new ArgumentNullException(nameof(modid));
            if (Converter == null)
            {
                Converter = new SoundCollectionConverter(modname, modid);
            }
            else
            {
                Converter.SetModInfo(modname, modid);
            }
        }

        public IEnumerable<SoundEvent> Deserialize(string value) => JsonConvert.DeserializeObject<IEnumerable<SoundEvent>>(value, Converter);

        public SoundEvent DeserializeItem(string value) => JsonConvert.DeserializeObject<SoundEvent>(value);

        public string Serialize(IEnumerable<SoundEvent> value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None, Converter);
        public string Serialize(IEnumerable<SoundEvent> value) => JsonConvert.SerializeObject(value, Converter);

        public string SerializeItem(SoundEvent value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None, Converter);
        public string SerializeItem(SoundEvent value) => JsonConvert.SerializeObject(value, Converter);

        T ISerializer.DeserializeObject<T>(string value) => JsonConvert.DeserializeObject<T>(value, Converter);
        object ISerializer.DeserializeObject(string value) => Deserialize(value);
        object ISerializer.DeserializeObject(string value, Type type) => Deserialize(value);
        string ISerializer.SerializeObject(object value, Type type, bool prettyPrint) => Serialize((IEnumerable<SoundEvent>)value);
        string ISerializer.SerializeObject(object value, Type type) => Serialize((IEnumerable<SoundEvent>)value);
        string ISerializer.SerializeObject(object value, bool prettyPrint) => Serialize((IEnumerable<SoundEvent>)value, prettyPrint);
        string ISerializer.SerializeObject(object value) => Serialize((IEnumerable<SoundEvent>)value);
    }
}
