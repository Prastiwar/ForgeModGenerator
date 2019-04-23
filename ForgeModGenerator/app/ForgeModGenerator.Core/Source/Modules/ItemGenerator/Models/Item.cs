using Prism.Mvvm;
using System.Collections.Generic;

namespace ForgeModGenerator.ItemGenerator.Models
{
    public class Item : BindableBase
    {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private ItemType type;
        public ItemType Type {
            get => type;
            set => SetProperty(ref type, value);
        }

        private string texturePath;
        public string TexturePath {
            get => texturePath;
            set => SetProperty(ref texturePath, value);
        }

        private float maxDamage;
        public float MaxDamage {
            get => maxDamage;
            set => SetProperty(ref maxDamage, value);
        }

        private int stackSize;
        public int StackSize {
            get => stackSize;
            set => SetProperty(ref stackSize, value);
        }

        private KeyValuePair<string, int> harvestLevel;
        public KeyValuePair<string, int> HarvestLevel {
            get => harvestLevel;
            set => SetProperty(ref harvestLevel, value);
        }
    }
}
