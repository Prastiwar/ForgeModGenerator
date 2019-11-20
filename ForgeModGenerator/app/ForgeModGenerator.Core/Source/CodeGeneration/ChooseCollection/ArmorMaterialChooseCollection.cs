namespace ForgeModGenerator.CodeGeneration
{
    public class ArmorMaterialChooseCollection : ChooseCollection
    {
        protected override StringGetter[] BuiltInGetters => new StringGetter[] {
            "ArmorMaterial.LEATHER",
            "ArmorMaterial.CHAIN",
            "ArmorMaterial.IRON",
            "ArmorMaterial.GOLD",
            "ArmorMaterial.DIAMOND"
        };
    }
}
