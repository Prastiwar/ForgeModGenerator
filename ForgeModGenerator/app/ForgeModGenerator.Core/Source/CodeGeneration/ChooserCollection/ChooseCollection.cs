using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ForgeModGenerator.CodeGeneration
{
    public class ChooseCollection : IEnumerable<string>
    {
        protected List<string> Getters { get; } = new List<string>();
        protected virtual string[] BuiltInGetters { get; } = System.Array.Empty<string>();

        public void AddGetter(string getter) => Getters.Add(getter);

        public void RemoveGetter(string getter) => Getters.Remove(getter);

        public string[] GetGettersCollection()
        {
            string[] getters = new string[BuiltInGetters.Length + Getters.Count];
            int i = 0;
            foreach (string sound in Getters.Concat(BuiltInGetters))
            {
                getters[i] = sound;
                i++;
            }
            return getters;
        }

        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)GetGettersCollection()).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetGettersCollection().GetEnumerator();
    }
}
