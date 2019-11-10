using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ItemGenerator.Models;
using ForgeModGenerator.Services;
using ForgeModGenerator.Validation;
using ForgeModGenerator.ViewModels;
using System.IO;

namespace ForgeModGenerator.ItemGenerator.ViewModels
{
    /// <summary> ItemGenerator Business ViewModel </summary>
    public class ItemGeneratorViewModel : SimpleInitViewModelBase<Item>
    {
        public ItemGeneratorViewModel(ISessionContextService sessionContext,
                                      IEditorFormFactory<Item> editorFormFactory,
                                      IUniqueValidator<Item> validator,
                                      ICodeGenerationService codeGenerationService)
            : base(sessionContext, editorFormFactory, validator, codeGenerationService) { }

        protected override string ScriptFilePath => SourceCodeLocator.Items(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization).FullPath;

        protected override Item ParseModelFromJavaField(string line)
        {
            Item item = new Item();

            int startIndex = line.IndexOf("new ") + 4;
            int endIndex = line.IndexOf("(", startIndex);
            string type = line.Substring(startIndex, endIndex - startIndex);
            item.Type = (ItemType)System.Enum.Parse(typeof(ItemType), type.Replace("Base", ""), true);

            startIndex = line.IndexOf("\"", endIndex) + 1;
            endIndex = line.IndexOf("\"", startIndex);
            string name = line.Substring(startIndex, endIndex - startIndex);
            item.Name = name;

            if (item.Type != ItemType.Item)
            {
                startIndex = line.IndexOf(" ", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string material = line.Substring(startIndex, endIndex - startIndex);
                item.Material = material;
            }
            if (item.Type == ItemType.Armor)
            {
                startIndex = line.IndexOf("EntityEquipmentSlot", endIndex) + "EntityEquipmentSlot".Length + 1;
                endIndex = line.IndexOf(")", startIndex);
                string armorType = line.Substring(startIndex, endIndex - startIndex);
                switch (armorType)
                {
                    case "HEAD":
                        item.ArmorType = ArmorType.Helmet;
                        break;
                    case "CHEST":
                        item.ArmorType = ArmorType.Chestplate;
                        break;
                    case "LEGS":
                        item.ArmorType = ArmorType.Leggings;
                        break;
                    case "FEET":
                        item.ArmorType = ArmorType.Boots;
                        break;
                    default:
                        item.ArmorType = ArmorType.None;
                        break;
                }
            }
            if (item.Type == ItemType.Item)
            {
                startIndex = line.IndexOf(" ", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string stackSize = line.Substring(startIndex, endIndex - startIndex);
                item.StackSize = int.Parse(stackSize, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                item.StackSize = 1;
            }

            string modelsItemPath = ModPaths.ModelsItemFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization);
            string itemJsonPath = Path.Combine(modelsItemPath, name.ToLower() + ".json");
            if (File.Exists(itemJsonPath))
            {
                string jsonContent = File.ReadAllText(itemJsonPath);
                startIndex = jsonContent.IndexOf(SessionContext.SelectedMod.ModInfo.Modid, 1);
                endIndex = jsonContent.IndexOf("\"", startIndex);
                string textureName = jsonContent.Substring(startIndex, endIndex - startIndex);
                item.TextureName = textureName;
            }

            item.IsDirty = false;
            return item;
        }
    }
}
