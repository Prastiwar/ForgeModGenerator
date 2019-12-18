using System.Collections.Generic;

namespace ForgeModGenerator.Validation
{
    internal class GenericUniqueValidator<T> : AbstractUniqueValidator<T>
    {
        public GenericUniqueValidator(IEnumerable<T> repository) : base(repository) { }
    }
}
