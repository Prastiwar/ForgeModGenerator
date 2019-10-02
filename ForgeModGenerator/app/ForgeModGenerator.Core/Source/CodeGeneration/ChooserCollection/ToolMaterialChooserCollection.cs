namespace ForgeModGenerator.CodeGeneration
{
    public class ToolMaterialChooserCollection : ChooseCollection
    {
        protected override string[] BuiltInGetters => new string[] {
            "ToolMaterial.WOOD",
            "ToolMaterial.STONE",
            "ToolMaterial.IRON",
            "ToolMaterial.DIAMOND",
            "ToolMaterial.GOLD",
        };
    }
}
