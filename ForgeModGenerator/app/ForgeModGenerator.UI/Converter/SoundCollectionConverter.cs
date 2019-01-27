using ForgeModGenerator.Core;
using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace ForgeModGenerator.Converter
{
    public class SoundCollectionConverter : JsonConverter<FileList<SoundEvent>>
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

        public override FileList<SoundEvent> ReadJson(JsonReader reader, Type objectType, FileList<SoundEvent> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string soundsPath = ModPaths.SoundsFolder(ModName, Modid);
            JObject item = JObject.Load(reader);
            string json = item.ToString();

            FileList<SoundEvent> fileList = new FileList<SoundEvent>(soundsPath);
            foreach (KeyValuePair<string, JToken> property in item)
            {
                SoundEvent soundEvent = item.GetValue(property.Key).ToObject<SoundEvent>();
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
            return fileList;
        }

        public override void WriteJson(JsonWriter writer, FileList<SoundEvent> value, JsonSerializer serializer)
        {
            builder.Clear();
            builder.Append("{\n");

            for (int i = 0; i < value.Count; i++)
            {
                SoundEvent item = value[i];
                itemBuilder.Clear();
                string json = JsonConvert.SerializeObject(item, Formatting.Indented);
                itemBuilder.Append(json).Replace($"\"{nameof(item.EventName)}\":", "").ReplaceN(",", ": {", 1).Remove(0, 1);
                if (i < value.Count - 1)
                {
                    itemBuilder.Append(',');
                }
                builder.Append(itemBuilder);
            }

            builder.Append("\n}");
            string serializedJson = builder.ToString();
            writer.WriteRawValue(serializedJson.FormatJson(serializer.Formatting));
        }
    }
}
