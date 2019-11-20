namespace ForgeModGenerator.CodeGeneration
{
    public class ToolMaterialChooseCollection : ChooseCollection
    {
        protected override StringGetter[] BuiltInGetters => new StringGetter[] {
            "ToolMaterial.WOOD",
            "ToolMaterial.STONE",
            "ToolMaterial.IRON",
            "ToolMaterial.DIAMOND",
            "ToolMaterial.GOLD",
        };
    }
}
