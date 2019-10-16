using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.MaterialGenerator.Models;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace ForgeModGenerator.MaterialGenerator.CodeGeneration
{
    public class MaterialCodeGenerator : InitVariablesCodeGenerator<Material>
    {
        public MaterialCodeGenerator(McMod mcMod) : this(mcMod, Enumerable.Empty<Material>()) { }
        public MaterialCodeGenerator(McMod mcMod, IEnumerable<Material> elements) : base(mcMod, elements) => ScriptLocator = SourceCodeLocator.Materials(Modname, Organization);

        public override ClassLocator ScriptLocator { get; }

        protected override string GetElementName(Material element) => element.Name;

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeCompileUnit unit = CreateDefaultTargetCodeUnit(ScriptLocator.ClassName, "Material");
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.block.material.Material"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.Item.ToolMaterial"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.item.ItemArmor.ArmorMaterial"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.block.material.Material"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraftforge.common.util.EnumHelper"));

            foreach (Material element in Elements)
            {
                CodeMemberField materialField = null;
                if (element is ArmorMaterial armorMaterial)
                {
                    materialField = NewFieldGlobal("ArmorMaterial", armorMaterial.Name.ToUpper(), NewMethodInvokeType("EnumHelper", "addArmorMaterial",
                        NewPrimitive(armorMaterial.Name.ToLower()),
                        NewPrimitive(armorMaterial.TextureName),
                        NewPrimitive(armorMaterial.Durability),
                        NewPrimitveArray(armorMaterial.DamageReductionAmountArray),
                        NewPrimitive(armorMaterial.Enchantability),
                        NewPrimitive(armorMaterial.SoundEvent),
                        NewPrimitive(armorMaterial.Toughness)
                        ));
                }
                else if (element is ToolMaterial toolMaterial)
                {
                    materialField = NewFieldGlobal("ToolMaterial", toolMaterial.Name.ToUpper(), NewMethodInvokeType("EnumHelper", "addToolMaterial",
                        NewPrimitive(toolMaterial.Name.ToLower()),
                        NewPrimitive(toolMaterial.HarvestLevel),
                        NewPrimitive(toolMaterial.MaxUses),
                        NewPrimitive(toolMaterial.Efficiency),
                        NewPrimitive(toolMaterial.AttackDamage),
                        NewPrimitive(toolMaterial.Enchantability)
                        ));
                }
                else if (element is BlockMaterial blockMaterial)
                {
                    CodeObjectCreateExpression assignObject = NewObject("Material");

                    CodeMethodInvokeExpression setSolid = NewMethodInvoke(assignObject, "setSolid", NewPrimitive(blockMaterial.IsTranslucent));
                    CodeMethodInvokeExpression setLiquid = NewMethodInvoke(setSolid, "setLiquid", NewPrimitive(blockMaterial.IsTranslucent));
                    CodeMethodInvokeExpression setBlockLight = NewMethodInvoke(setLiquid, "setBlockLight", NewPrimitive(blockMaterial.IsTranslucent));
                    CodeMethodInvokeExpression setBlockMovement = NewMethodInvoke(setBlockLight, "setBlockMovement", NewPrimitive(blockMaterial.IsTranslucent));
                    CodeMethodInvokeExpression setTranslucent = NewMethodInvoke(assignObject, "setTranslucent", NewPrimitive(blockMaterial.IsTranslucent));
                    CodeMethodInvokeExpression setRequiresNoTool = NewMethodInvoke(setTranslucent, "setRequiresNoTool", NewPrimitive(blockMaterial.RequiresNoTool));
                    CodeMethodInvokeExpression setBurning = NewMethodInvoke(setRequiresNoTool, "setBurning", NewPrimitive(blockMaterial.CanBurn));
                    CodeMethodInvokeExpression setReplaceable = NewMethodInvoke(setBurning, "setReplaceable", NewPrimitive(blockMaterial.IsReplaceable));
                    CodeMethodInvokeExpression setAdventureModeExempt = NewMethodInvoke(setReplaceable, "setAdventureModeExempt", NewPrimitive(blockMaterial.IsAdventureModeExempt));
                    CodeMethodInvokeExpression setMobilityFlag = NewMethodInvoke(setAdventureModeExempt, "setMobilityFlag", NewPrimitive(blockMaterial.MobilityFlag));

                    materialField = NewFieldGlobal("Material", blockMaterial.Name.ToUpper(), setMobilityFlag);
                }
                else
                {
                    throw new System.NotImplementedException($"{element.GetType()} was not implemented");
                }
                unit.Namespaces[0].Types[0].Members.Add(materialField);
            }
            return unit;
        }
    }
}
