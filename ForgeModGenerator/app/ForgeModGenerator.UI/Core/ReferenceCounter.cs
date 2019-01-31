using System.Collections.Generic;

namespace ForgeModGenerator
{
    public static class ReferenceCounter
    {
        private static Dictionary<object, HashSet<object>> referenceValues = new Dictionary<object, HashSet<object>>();

        public static void AddReference(object referencedObject, object reference)
        {
            if (referencedObject == null)
            {
                return;
            }
            if (referenceValues.TryGetValue(referencedObject, out HashSet<object> hashSet))
            {
                hashSet.Add(reference);
            }
            else
            {
                referenceValues[referencedObject] = new HashSet<object>() { reference };
            }
        }

        public static void RemoveReference(object referencedObject, object reference)
        {
            if (referencedObject == null)
            {
                return;
            }
            if (referenceValues.TryGetValue(referencedObject, out HashSet<object> hashSet))
            {
                bool removed = hashSet.Remove(reference);
                if (hashSet.Count <= 0)
                {
                    referenceValues.Remove(referencedObject);
                }
            }
        }

        public static int GetReferenceCount(object referencedObject) => referencedObject != null && referenceValues.TryGetValue(referencedObject, out HashSet<object> hashSet) ? hashSet.Count : 0;
        public static IEnumerable<object> GetReferences(object referencedObject) => referencedObject != null && referenceValues.TryGetValue(referencedObject, out HashSet<object> hashSet) ? hashSet : null;
    }
}
