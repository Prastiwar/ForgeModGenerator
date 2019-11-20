using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ForgeModGenerator.CodeGeneration
{
    public class ChooseCollection : IEnumerable<StringGetter>
    {
        protected List<StringGetter> Getters { get; } = new List<StringGetter>();
        protected virtual StringGetter[] BuiltInGetters { get; } = System.Array.Empty<StringGetter>();

        public void AddGetter(string getter) => Getters.Add(getter);

        public bool RemoveGetter(string getter) => Getters.Remove(getter);

        public StringGetter[] GetGettersCollection()
        {
            StringGetter[] getters = new StringGetter[BuiltInGetters.Length + Getters.Count];
            int i = 0;
            foreach (StringGetter get in Getters.Concat(BuiltInGetters))
            {
                getters[i] = get;
                i++;
            }
            return getters;
        }

        public IEnumerator<StringGetter> GetEnumerator() => ((IEnumerable<StringGetter>)GetGettersCollection()).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetGettersCollection().GetEnumerator();
    }
}
