using ForgeModGenerator.View;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ForgeModGenerator
{
    public partial class App : Application
    {
        public struct Pages
        {
            public const string Dashboard = "Dashboard";
            public const string ModGenerator = "ModGenerator";
            public const string BlockGenerator = "BlockGenerator";
            public const string ItemGenerator = "ItemGenerator";
            public const string SoundGenerator = "SoundGenerator";
            public const string CommandGenerator = "CommandGenerator";
            public const string AchievementGenerator = "AchievementGenerator";
            public const string RecipeGenerator = "RecipeGenerator";
            public const string Settings = "Settings";

            public static KeyValuePair<string, Type>[] GetAllPagesInfo() => new KeyValuePair<string, Type>[] {
                new KeyValuePair<string, Type>(Dashboard, typeof(DashboardPage))
                //new KeyValuePair<string, Type>(ModGenerator, typeof(ModGeneratorPage)),
                //new KeyValuePair<string, Type>(BlockGenerator, typeof(BlockGeneratorPage)),
                //new KeyValuePair<string, Type>(ItemGenerator, typeof(ItemGeneratorPage)),
                //new KeyValuePair<string, Type>(SoundGenerator, typeof(SoundGeneratorPage)),
                //new KeyValuePair<string, Type>(CommandGenerator, typeof(CommandGeneratorPage)),
                //new KeyValuePair<string, Type>(AchievementGenerator, typeof(AchievementGeneratorPage)),
                //new KeyValuePair<string, Type>(RecipeGenerator, typeof(RecipeGeneratorPage))
                //new KeyValuePair<string, Type>(Settings, typeof(SettingsPage))
            };
        }
    }
}
