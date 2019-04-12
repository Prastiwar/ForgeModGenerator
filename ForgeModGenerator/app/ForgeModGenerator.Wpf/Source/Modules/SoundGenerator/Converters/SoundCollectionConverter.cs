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
        public SoundCollectionConverter(string modname, string modid) => SetModInfo(modname, modid);

        protected static readonly StringBuilder builder = new StringBuilder(32);
        protected static readonly StringBuilder itemBuilder = new StringBuilder(32);

        protected string ModName { get; set; }
        protected string Modid { get; set; }

        public void SetModInfo(string modname, string modid)
        {
            ModName = modname ?? throw new ArgumentNullException(nameof(modname));
            Modid = modid ?? throw new ArgumentNullException(nameof(modid));
        }

        public override bool CanConvert(Type objectType) => objectType.IsAssignableFrom(typeof(ICollection<SoundEvent>))
                                                        || objectType.IsAssignableFrom(typeof(IEnumerable<SoundEvent>))
                                                        || objectType.IsAssignableFrom(typeof(ObservableCollection<SoundEvent>))
                                                        || objectType.IsAssignableFrom(typeof(WpfObservableFolder<SoundEvent>))
                                                        || objectType.IsAssignableFrom(typeof(WpfObservableRangeCollection<SoundEvent>))
                                                        || objectType.IsAssignableFrom(typeof(ObservableRangeCollection<SoundEvent>))
                                                        || objectType.IsAssignableFrom(typeof(List<SoundEvent>));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string soundsPath = ModPaths.SoundsFolder(ModName, Modid);
            ICollection<SoundEvent> folders = new Collection<SoundEvent>();
            JObject item = JObject.Load(reader);
            serializer.Converters.Add(new SoundEventConverter());
            foreach (KeyValuePair<string, JToken> property in item)
            {
                SoundEvent soundEvent = item.GetValue(property.Key).ToObject<SoundEvent>(serializer);
                soundEvent.EventName = property.Key;
                soundEvent.SetInfo(soundsPath);

                for (int i = soundEvent.Files.Count - 1; i >= 0; i--)
                {
                    Sound sound = soundEvent.Files[i];
                    string soundName = sound.Name;
                    int modidLength = sound.Name.IndexOf(":") + 1;
                    if (modidLength != -1)
                    {
                        soundName = sound.Name.Remove(0, modidLength);
                    }
                    try
                    {
                        sound.SetInfo(Path.Combine(soundsPath, $"{soundName}.ogg"));
                    }
                    catch (Exception)
                    {
                        soundEvent.Files.RemoveAt(i); // if file doesn't exist, sound can't be loaded
                    }
                    sound.IsDirty = false;
                }
                soundEvent.IsDirty = false;
                folders.Add(soundEvent);
            }
            if (objectType.IsAssignableFrom(typeof(WpfObservableFolder<SoundEvent>)))
            {
                return new WpfObservableFolder<SoundEvent>(soundsPath, folders);
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
            string newLine = Environment.NewLine;
            builder.Append("{" + newLine);

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
                string json = JsonConvert.SerializeObject(folder, Formatting.Indented, new SoundEventConverter());
                itemBuilder.Append(json);

                bool isLastElement = i < folders.Count() - 1;
                if (isLastElement)
                {
                    itemBuilder.Append(',');
                }
                builder.Append(itemBuilder);
                i++;
            }

            builder.Append(newLine + "}");
            string serializedJson = builder.ToString();
            writer.WriteRawValue(serializedJson.FormatJson(serializer.Formatting));
        }
    }
}
