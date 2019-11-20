namespace ForgeModGenerator.CodeGeneration
{
    public sealed class StringGetter
    {
        private readonly string value;

        public StringGetter(string val) => value = val;

        public static implicit operator StringGetter(string val) => new StringGetter(val);

        public override string ToString() => value;

        public static bool operator ==(StringGetter val, StringGetter val2) => val.value == val2.value;
        public static bool operator !=(StringGetter val, StringGetter val2) => val.value != val2.value;

        public static bool operator ==(StringGetter val, string val2) => val.value == val2;
        public static bool operator !=(StringGetter val, string val2) => val.value != val2;

        public override bool Equals(object obj)
        {
            if (obj is StringGetter getter)
            {
                return getter.value == value;
            }
            else if (obj is string label)
            {
                return label == value;
            }
            return false;
        }

        public override int GetHashCode() => value.GetHashCode();
    }
}
