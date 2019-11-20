namespace ForgeModGenerator.CodeGeneration
{
    public class SoundTypeChooseCollection : ChooseCollection
    {
        protected override StringGetter[] BuiltInGetters => new StringGetter[] {
            "Wood",
            "Ground",
            "Plant",
            "Stone",
            "Metal",
            "Glass",
            "Cloth",
            "Sand",
            "Snow",
            "Ladder",
            "Anvil",
            "Slime"
        };
    }
}
