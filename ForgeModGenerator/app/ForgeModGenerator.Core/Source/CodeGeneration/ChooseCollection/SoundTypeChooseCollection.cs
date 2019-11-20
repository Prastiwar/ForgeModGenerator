namespace ForgeModGenerator.CodeGeneration
{
    public class SoundTypeChooseCollection : ChooseCollection
    {
        protected override StringGetter[] BuiltInGetters => new StringGetter[] {
            "SoundType.Wood",
            "SoundType.Ground",
            "SoundType.Plant",
            "SoundType.Stone",
            "SoundType.Metal",
            "SoundType.Glass",
            "SoundType.Cloth",
            "SoundType.Sand",
            "SoundType.Snow",
            "SoundType.Ladder",
            "SoundType.Anvil",
            "SoundType.Slime"
        };
    }
}
