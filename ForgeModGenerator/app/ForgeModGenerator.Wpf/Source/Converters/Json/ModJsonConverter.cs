using ForgeModGenerator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ForgeModGenerator.Converters
{
    public class ModJsonConverter : JsonConverter<Mod>
    {
        public override Mod ReadJson(JsonReader reader, Type objectType, Mod existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            if (!item.HasValues)
            {
                return null;
            }
            string organization = item.GetValue(nameof(Mod.Organization), StringComparison.OrdinalIgnoreCase).ToObject<string>();
            McModInfo modInfo = item.GetValue(nameof(Mod.ModInfo), StringComparison.OrdinalIgnoreCase).ToObject<McModInfo>(serializer);            
            ForgeVersion forgeVersion = item.GetValue(nameof(Mod.ForgeVersion), StringComparison.OrdinalIgnoreCase).ToObject<ForgeVersion>(serializer);

            ModSide side = ModSide.ClientServer;
            if (item.TryGetValue(nameof(Mod.Side), StringComparison.OrdinalIgnoreCase, out JToken sideValue))
            {
                side = sideValue.ToObject<ModSide>();
            }

            LaunchSetup launchSetup = null;
            if (item.TryGetValue(nameof(Mod.LaunchSetup), StringComparison.OrdinalIgnoreCase, out JToken launchValue))
            {
                launchSetup = launchValue.ToObject<LaunchSetup>();
            }
            
            WorkspaceSetup workspaceSetup = null;
            if (item.TryGetValue(nameof(Mod.WorkspaceSetup), StringComparison.OrdinalIgnoreCase, out JToken workspaceValue))
            {
                workspaceSetup = workspaceValue.ToObject<WorkspaceSetup>(serializer);
            }
            return new Mod(modInfo, organization, forgeVersion, side, launchSetup, workspaceSetup);
        }

        public override void WriteJson(JsonWriter writer, Mod value, JsonSerializer serializer)
        {
            JObject jo = new JObject {
                { nameof(Mod.Organization), value.Organization },
                { nameof(Mod.ModInfo), JToken.FromObject(value.ModInfo, serializer) },
                { nameof(Mod.ForgeVersion), JToken.FromObject(value.ForgeVersion, serializer) },
                { nameof(Mod.LaunchSetup), JToken.FromObject(value.LaunchSetup, serializer) },
                { nameof(Mod.Side), JToken.FromObject(value.Side, serializer) },
                { nameof(Mod.WorkspaceSetup), JToken.FromObject(value.WorkspaceSetup, serializer) },
                { nameof(Mod.CachedName), value.CachedName }
            };
            jo.WriteTo(writer);
        }
    }
}
