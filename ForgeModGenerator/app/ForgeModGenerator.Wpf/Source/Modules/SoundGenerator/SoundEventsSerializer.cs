using ForgeModGenerator.Serialization;
using ForgeModGenerator.SoundGenerator.Converters;
using ForgeModGenerator.SoundGenerator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator.Serialization
{
    public sealed class SoundEventsSerializer : ISoundEventsSerializer
    {
        public SoundEventsSerializer() { }

        private string modname;
        private string modid;
        private SoundCollectionConverter converter;

        public void SetModInfo(string modname, string modid)
        {
            this.modname = modname ?? throw new ArgumentNullException(nameof(modname));
            this.modid = modid ?? throw new ArgumentNullException(nameof(modid));
            if (converter == null)
            {
                converter = new SoundCollectionConverter(modname, modid);
            }
            else
            {
                converter.SetModInfo(modname, modid);
            }
        }

        public IEnumerable<SoundEvent> Deserialize(string value) => JsonConvert.DeserializeObject<IEnumerable<SoundEvent>>(value, converter);

        public SoundEvent DeserializeItem(string value) => JsonConvert.DeserializeObject<SoundEvent>(value);

        public string Serialize(IEnumerable<SoundEvent> value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None, converter);
        public string Serialize(IEnumerable<SoundEvent> value) => JsonConvert.SerializeObject(value, converter);

        public string SerializeItem(SoundEvent value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None, converter);
        public string SerializeItem(SoundEvent value) => JsonConvert.SerializeObject(value, converter);

        T ISerializer.DeserializeObject<T>(string value) => JsonConvert.DeserializeObject<T>(value, converter);
        object ISerializer.DeserializeObject(string value) => Deserialize(value);
        object ISerializer.DeserializeObject(string value, Type type) => Deserialize(value);
        string ISerializer.SerializeObject(object value, Type type, bool prettyPrint) => Serialize((IEnumerable<SoundEvent>)value);
        string ISerializer.SerializeObject(object value, Type type) => Serialize((IEnumerable<SoundEvent>)value);
        string ISerializer.SerializeObject(object value, bool prettyPrint) => Serialize((IEnumerable<SoundEvent>)value, prettyPrint);
        string ISerializer.SerializeObject(object value) => Serialize((IEnumerable<SoundEvent>)value);
    }
}
