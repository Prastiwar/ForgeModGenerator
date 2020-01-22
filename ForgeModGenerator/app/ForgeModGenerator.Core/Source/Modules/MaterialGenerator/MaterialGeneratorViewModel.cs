using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.MaterialGenerator.Models;
using ForgeModGenerator.ViewModels;

namespace ForgeModGenerator.MaterialGenerator.ViewModels
{
    /// <summary> MaterialGenerator Business ViewModel </summary>
    public class MaterialGeneratorViewModel : JavaInitViewModelBase<Material>
    {
        public MaterialGeneratorViewModel(GeneratorContext<Material> context) : base(context) { }

        protected override string InitFilePath => SourceCodeLocator.Materials(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization).FullPath;

        protected override Material ParseModelFromJavaField(string line)
        {
            System.Globalization.CultureInfo invariancy = System.Globalization.CultureInfo.InvariantCulture;

            int startIndex = 0;
            int endIndex = line.IndexOf(" ", startIndex);
            string type = line.Substring(startIndex, endIndex - startIndex);

            startIndex = endIndex + 1;
            endIndex = line.IndexOf(" ", startIndex);
            string name = line.Substring(startIndex, endIndex - startIndex);

            Material material = null;
            if (type == "ToolMaterial")
            {
                material = new ToolMaterial() { Name = name };
                ToolMaterial toolMaterial = (ToolMaterial)material;

                startIndex = line.IndexOf(",", endIndex) + 1;
                endIndex = line.IndexOf(",", startIndex);
                string harvestLevel = line.Substring(startIndex, endIndex - startIndex);
                toolMaterial.HarvestLevel = int.Parse(harvestLevel, invariancy);

                startIndex = line.IndexOf(",", endIndex) + 1;
                endIndex = line.IndexOf(",", startIndex);
                string maxUses = line.Substring(startIndex, endIndex - startIndex);
                toolMaterial.MaxUses = int.Parse(maxUses, invariancy);

                startIndex = line.IndexOf(",", endIndex) + 1;
                endIndex = line.IndexOf(",", startIndex);
                string efficiency = line.Substring(startIndex, endIndex - startIndex);
                toolMaterial.Efficiency = float.Parse(efficiency, invariancy);

                startIndex = line.IndexOf(",", endIndex) + 1;
                endIndex = line.IndexOf(",", startIndex);
                string damage = line.Substring(startIndex, endIndex - startIndex);
                toolMaterial.AttackDamage = float.Parse(damage, invariancy);

                startIndex = line.IndexOf(",", endIndex) + 1;
                endIndex = line.IndexOf(",", startIndex);
                string enchantability = line.Substring(startIndex, endIndex - startIndex);
                toolMaterial.Enchantability = int.Parse(enchantability, invariancy);
            }
            else if (type == "ArmorMaterial")
            {
                material = new ArmorMaterial() { Name = name };
                ArmorMaterial armorMaterial = (ArmorMaterial)material;

                startIndex = line.IndexOf(",", endIndex) + 1;
                endIndex = line.IndexOf(",", startIndex);
                string textureName = line.Substring(startIndex, endIndex - startIndex);
                armorMaterial.TextureName = textureName;

                startIndex = line.IndexOf(",", endIndex) + 1;
                endIndex = line.IndexOf(",", startIndex);
                string durability = line.Substring(startIndex, endIndex - startIndex);
                armorMaterial.Durability = int.Parse(durability, invariancy);

                startIndex = line.IndexOf("{", endIndex) + 1;
                endIndex = line.IndexOf("}", startIndex);
                string reductionAmounts = line.Substring(startIndex, endIndex - startIndex).Trim();
                armorMaterial.HelmetDamageReduction = (int)char.GetNumericValue(reductionAmounts, 0);
                armorMaterial.PlateDamageReduction = (int)char.GetNumericValue(reductionAmounts, 2);
                armorMaterial.LegsDamageReduction = (int)char.GetNumericValue(reductionAmounts, 4);
                armorMaterial.BootsDamageReduction = (int)char.GetNumericValue(reductionAmounts, 6);

                startIndex = line.IndexOf(",", endIndex) + 1;
                endIndex = line.IndexOf(",", startIndex);
                string enchantability = line.Substring(startIndex, endIndex - startIndex);
                armorMaterial.Enchantability = int.Parse(enchantability, invariancy);

                startIndex = line.IndexOf(",", endIndex) + 1;
                endIndex = line.IndexOf(",", startIndex);
                string soundOnEquip = line.Substring(startIndex, endIndex - startIndex);
                armorMaterial.SoundEvent = soundOnEquip;

                startIndex = line.IndexOf(",", endIndex) + 1;
                endIndex = line.IndexOf(",", startIndex);
                string toughness = line.Substring(startIndex, endIndex - startIndex);
                armorMaterial.Toughness = float.Parse(toughness, invariancy);
            }
            else if (type == "Material")
            {
                material = new BlockMaterial() { Name = name };
                BlockMaterial blockMaterial = (BlockMaterial)material;

                startIndex = line.IndexOf("(", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string solid = line.Substring(startIndex, endIndex - startIndex);
                blockMaterial.IsSolid = bool.Parse(solid);

                startIndex = line.IndexOf("(", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string liquid = line.Substring(startIndex, endIndex - startIndex);
                blockMaterial.IsLiquid = bool.Parse(liquid);

                startIndex = line.IndexOf("(", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string blockLight = line.Substring(startIndex, endIndex - startIndex);
                blockMaterial.BlocksLight = bool.Parse(blockLight);

                startIndex = line.IndexOf("(", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string blockMovement = line.Substring(startIndex, endIndex - startIndex);
                blockMaterial.BlocksMovement = bool.Parse(blockMovement);

                startIndex = line.IndexOf("(", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string translucent = line.Substring(startIndex, endIndex - startIndex);
                blockMaterial.IsTranslucent = bool.Parse(translucent);

                startIndex = line.IndexOf("(", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string requiresNoTool = line.Substring(startIndex, endIndex - startIndex);
                blockMaterial.RequiresNoTool = bool.Parse(requiresNoTool);

                startIndex = line.IndexOf("(", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string burning = line.Substring(startIndex, endIndex - startIndex);
                blockMaterial.CanBurn = bool.Parse(burning);

                startIndex = line.IndexOf("(", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string replaceable = line.Substring(startIndex, endIndex - startIndex);
                blockMaterial.IsReplaceable = bool.Parse(replaceable);

                startIndex = line.IndexOf("(", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string adventureModeExempt = line.Substring(startIndex, endIndex - startIndex);
                blockMaterial.IsAdventureModeExempt = bool.Parse(adventureModeExempt);

                startIndex = line.IndexOf("(", endIndex) + 1;
                endIndex = line.IndexOf(")", startIndex);
                string mobilityFlag = line.Substring(startIndex, endIndex - startIndex);
                blockMaterial.MobilityFlag = (PushReaction)System.Enum.Parse(typeof(PushReaction), mobilityFlag.Remove(0, "EnumPushReaction.".Length), true);
            }
            else
            {
                throw new System.NotImplementedException($"Implementation of {type} was not found");
            }

            material.IsDirty = false;
            return material;
        }
    }
}
