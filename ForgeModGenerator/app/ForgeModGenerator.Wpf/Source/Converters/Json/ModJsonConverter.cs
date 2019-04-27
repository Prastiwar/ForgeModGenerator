using ForgeModGenerator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace ForgeModGenerator.Converters
{
    public class ModJsonConverter : JsonConverter<McMod>
    {
        public override McMod ReadJson(JsonReader reader, Type objectType, McMod existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            if (!item.HasValues)
            {
                return null;
            }
            string organization = item.GetValue(nameof(McMod.Organization), StringComparison.OrdinalIgnoreCase).ToObject<string>();
            string modname = item.GetValue(nameof(McMod.CachedName), StringComparison.OrdinalIgnoreCase).ToObject<string>();
            string modInfoPath = ModPaths.McModInfoFile(modname);
            string modInfoContent = File.ReadAllText(modInfoPath);
            McModInfo modInfo = JsonConvert.DeserializeObject<McModInfo>(modInfoContent, new JsonSerializerSettings {
                Converters = serializer.Converters
            });
            ForgeVersion forgeVersion = item.GetValue(nameof(McMod.ForgeVersion), StringComparison.OrdinalIgnoreCase).ToObject<ForgeVersion>(serializer);

            ModSide side = ModSide.ClientServer;
            if (item.TryGetValue(nameof(McMod.Side), StringComparison.OrdinalIgnoreCase, out JToken sideValue))
            {
                side = sideValue.ToObject<ModSide>();
            }

            LaunchSetup launchSetup = LaunchSetup.Client;
            if (item.TryGetValue(nameof(McMod.LaunchSetup), StringComparison.OrdinalIgnoreCase, out JToken launchValue))
            {
                launchSetup = launchValue.ToObject<LaunchSetup>();
            }

            WorkspaceSetup workspaceSetup = null;
            if (item.TryGetValue(nameof(McMod.WorkspaceSetup), StringComparison.OrdinalIgnoreCase, out JToken workspaceValue))
            {
                workspaceSetup = workspaceValue.ToObject<WorkspaceSetup>(serializer);
            }
            return new McMod(modInfo, organization, forgeVersion, side, launchSetup, workspaceSetup);
        }

        public override void WriteJson(JsonWriter writer, McMod value, JsonSerializer serializer)
        {
            JObject jo = new JObject {
                { nameof(McMod.Organization), value.Organization },
                { nameof(McMod.ModInfo), JToken.FromObject(value.ModInfo, serializer) },
                { nameof(McMod.ForgeVersion), JToken.FromObject(value.ForgeVersion, serializer) },
                { nameof(McMod.LaunchSetup), JToken.FromObject(value.LaunchSetup, serializer) },
                { nameof(McMod.Side), JToken.FromObject(value.Side, serializer) },
                { nameof(McMod.WorkspaceSetup), JToken.FromObject(value.WorkspaceSetup, serializer) },
                { nameof(McMod.CachedName), value.CachedName }
            };
            jo.WriteTo(writer);
        }
    }
}
