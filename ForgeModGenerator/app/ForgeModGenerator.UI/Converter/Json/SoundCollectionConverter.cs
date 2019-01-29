using ForgeModGenerator.Core;
using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace ForgeModGenerator.Converter
{
    public class SoundCollectionConverter : JsonConverter
    {
        public string ModName { get; set; }
        public string Modid { get; set; }

        protected static readonly StringBuilder builder = new StringBuilder(32);
        protected static readonly StringBuilder itemBuilder = new StringBuilder(32);

        public SoundCollectionConverter(string modname, string modid)
        {
            ModName = modname;
            Modid = modid;
        }

        public override bool CanConvert(Type objectType) => typeof(ObservableCollection<FileList<SoundEvent>>).IsAssignableFrom(objectType) || typeof(FileList<SoundEvent>).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string soundsPath = ModPaths.SoundsFolder(ModName, Modid);
            FileList<SoundEvent> fileList = new FileList<SoundEvent>(soundsPath);
            JObject item = JObject.Load(reader);

            foreach (KeyValuePair<string, JToken> property in item)
            {
                JsonSerializer eventSerializer = new JsonSerializer();
                eventSerializer.Converters.Add(new SoundEventConverter());
                SoundEvent soundEvent = item.GetValue(property.Key).ToObject<SoundEvent>(eventSerializer);
                soundEvent.EventName = property.Key;
                soundEvent.SetFileItem(soundsPath);
                foreach (Sound sound in soundEvent.Sounds)
                {
                    string soundName = sound.Name;
                    int modidLength = sound.Name.IndexOf(":") + 1;
                    if (modidLength != -1)
                    {
                        soundName = sound.Name.Remove(0, modidLength);
                    }
                    sound.SetFileItem(Path.Combine(soundsPath, $"{soundName}.ogg"));
                }
                fileList.Add(soundEvent);
            }
            if (typeof(ObservableCollection<FileList<SoundEvent>>).IsAssignableFrom(objectType))
            {
                return new ObservableCollection<FileList<SoundEvent>>() { fileList };
            }
            else if (typeof(FileList<SoundEvent>).IsAssignableFrom(objectType))
            {
                return fileList;
            }
            throw new JsonReaderException($"Object type is not neither assignable from {typeof(ObservableCollection<FileList<SoundEvent>>)} or {typeof(FileList<SoundEvent>)}");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            builder.Clear();
            builder.Append("{\n");

            if (value is FileList<SoundEvent> fileList)
            {
                AppendFileListSoundEvent(fileList, true);
            }
            else if (value is ObservableCollection<FileList<SoundEvent>> observable)
            {
                for (int i = 0; i < observable.Count; i++)
                {
                    bool isLastElement = i < observable.Count - 1;
                    AppendFileListSoundEvent(observable[i], !isLastElement);
                }
            }
            else
            {
                throw new JsonWriterException($"Object type was null, or neither type, {typeof(ObservableCollection<FileList<SoundEvent>>)} or {typeof(FileList<SoundEvent>)}");
            }

            void AppendFileListSoundEvent(FileList<SoundEvent> val, bool removeCommaFromEnd)
            {
                for (int i = 0; i < val.Count; i++)
                {
                    SoundEvent item = val[i];
                    itemBuilder.Clear();
                    string json = JsonConvert.SerializeObject(item, Formatting.Indented);
                    itemBuilder.Append(json).Replace($"\"{nameof(item.EventName)}\":", "")
                        .ReplaceN(",", ": {", 1)
                        .Remove(0, 1)
                        .Replace("null", "\"\"");

                    bool isLastElement = i < val.Count - 1;
                    if (removeCommaFromEnd && isLastElement)
                    {
                        itemBuilder.Append(',');
                    }
                    builder.Append(itemBuilder);
                }
            }

            builder.Append("\n}");
            string serializedJson = builder.ToString();
            writer.WriteRawValue(serializedJson.FormatJson(serializer.Formatting));
        }
    }
}
