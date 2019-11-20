namespace ForgeModGenerator.CodeGeneration
{
    public sealed class StringGetter
    {
        private readonly string value;

        public StringGetter(string val) => value = val;

        public static implicit operator StringGetter(string val) => new StringGetter(val);

        public override string ToString() => value;
    }
}
