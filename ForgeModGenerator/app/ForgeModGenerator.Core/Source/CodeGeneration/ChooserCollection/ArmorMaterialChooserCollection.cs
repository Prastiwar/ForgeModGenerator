namespace ForgeModGenerator.CodeGeneration
{
    public class ArmorMaterialChooserCollection : ChooseCollection
    {
        protected override string[] BuiltInGetters => new string[] {
            "ArmorMaterial.LEATHER",
            "ArmorMaterial.CHAIN",
            "ArmorMaterial.IRON",
            "ArmorMaterial.GOLD",
            "ArmorMaterial.DIAMOND"
        };
    }
}
