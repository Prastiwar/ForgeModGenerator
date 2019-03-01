using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ForgeModGenerator.SoundGenerator.Converters
{
    public class SoundCollectionConverter : JsonConverter
    {
        public string ModName { get; set; }
        public string Modid { get; set; }

        protected static readonly StringBuilder builder = new StringBuilder(32);
        protected static readonly StringBuilder itemBuilder = new StringBuilder(32);

        public SoundCollectionConverter(string modname, string modid)
        {
            ModName = modname ?? throw new ArgumentNullException(nameof(modname));
            Modid = modid ?? throw new ArgumentNullException(nameof(modid));
        }

        public override bool CanConvert(Type objectType) => objectType.IsAssignableFrom(typeof(ICollection<SoundEvent>))
                                                        || objectType.IsAssignableFrom(typeof(IEnumerable<SoundEvent>))
                                                        || objectType.IsAssignableFrom(typeof(ObservableCollection<SoundEvent>))
                                                        || objectType.IsAssignableFrom(typeof(ObservableFolder<SoundEvent>))
                                                        || objectType.IsAssignableFrom(typeof(WpfObservableRangeCollection<SoundEvent>))
                                                        || objectType.IsAssignableFrom(typeof(ObservableRangeCollection<SoundEvent>))
                                                        || objectType.IsAssignableFrom(typeof(List<SoundEvent>));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string soundsPath = ModPaths.SoundsFolder(ModName, Modid);
            ICollection<SoundEvent> folders = new Collection<SoundEvent>();
            JObject item = JObject.Load(reader);

            foreach (KeyValuePair<string, JToken> property in item)
            {
                SoundEvent soundEvent = item.GetValue(property.Key).ToObject<SoundEvent>();
                soundEvent.EventName = property.Key;
                soundEvent.SetInfo(soundsPath);
                foreach (Sound sound in soundEvent.Files)
                {
                    string soundName = sound.Name;
                    int modidLength = sound.Name.IndexOf(":") + 1;
                    if (modidLength != -1)
                    {
                        soundName = sound.Name.Remove(0, modidLength);
                    }
                    sound.SetInfo(Path.Combine(soundsPath, $"{soundName}.ogg"));
                    sound.IsDirty = false;
                }
                soundEvent.IsDirty = false;
                folders.Add(soundEvent);
            }
            if (objectType.IsAssignableFrom(typeof(ObservableFolder<SoundEvent>)))
            {
                return new ObservableFolder<SoundEvent>(soundsPath, folders);
            }
            else if (objectType.IsAssignableFrom(typeof(ObservableCollection<SoundEvent>)))
            {
                return new ObservableCollection<SoundEvent>(folders);
            }
            else if (objectType.IsAssignableFrom(typeof(WpfObservableRangeCollection<SoundEvent>)))
            {
                return new WpfObservableRangeCollection<SoundEvent>(folders);
            }
            else if (objectType.IsAssignableFrom(typeof(ObservableRangeCollection<SoundEvent>)))
            {
                return new ObservableRangeCollection<SoundEvent>(folders);
            }
            else if (objectType.IsAssignableFrom(typeof(List<SoundEvent>)))
            {
                return folders.ToList();
            }
            return folders;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            builder.Clear();
            builder.Append("{\n");

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            else if (!(value is IEnumerable<SoundEvent>))
            {
                throw new ArgumentException("Passed object should derive from IEnumerable");
            }
            IEnumerable<SoundEvent> folders = (IEnumerable<SoundEvent>)value;
            int i = 0;
            foreach (SoundEvent folder in folders.Where(folder => folder.Files.Count > 0))
            {
                itemBuilder.Clear();
                string json = JsonConvert.SerializeObject(folder, Formatting.Indented);
                itemBuilder.Append(json);

                bool isLastElement = i < folders.Count() - 1;
                if (isLastElement)
                {
                    itemBuilder.Append(',');
                }
                builder.Append(itemBuilder);
                i++;
            }

            builder.Append("\n}");
            string serializedJson = builder.ToString();
            writer.WriteRawValue(serializedJson.FormatJson(serializer.Formatting));
        }
    }
}
