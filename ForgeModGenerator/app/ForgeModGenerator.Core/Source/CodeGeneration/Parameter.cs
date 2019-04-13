namespace ForgeModGenerator.CodeGeneration
{
    public struct Parameter
    {
        public string TypeName { get; }
        public string Name { get; }

        public Parameter(string type, string name)
        {
            TypeName = type;
            Name = name;
        }
    }
}
