using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ForgeModGenerator.Utility
{
    public static class CollectionExtensions
    {
        public static bool HasOnlyOne<T>(this IEnumerable<T> items, Func<T, bool> match) => items.Where(match).Take(2).Count() == 1;

        public static bool Exists<T>(this Collection<T> items, Predicate<T> match) => items.FindIndex(match) != -1;

        public static T Find<T>(this Collection<T> items, Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (match(items[i]))
                {
                    return items[i];
                }
            }
            return default;
        }

        public static Collection<T> FindAll<T>(this Collection<T> items, Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            Collection<T> collection = new Collection<T>();
            for (int i = 0; i < items.Count; i++)
            {
                if (match(items[i]))
                {
                    collection.Add(items[i]);
                }
            }
            return collection;
        }

        public static int FindIndex<T>(this Collection<T> items, Predicate<T> match) => items.FindIndex(0, items.Count, match);

        public static int FindIndex<T>(this Collection<T> items, int startIndex, Predicate<T> match) => items.FindIndex(startIndex, items.Count - startIndex, match);

        public static int FindIndex<T>(this Collection<T> items, int startIndex, int count, Predicate<T> match)
        {
            if (startIndex > items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            else if (count < 0 || startIndex > items.Count - count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            else if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            int endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (match(items[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static T FindLast<T>(this Collection<T> items, Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (match(items[i]))
                {
                    return items[i];
                }
            }
            return default;
        }

        public static int FindLastIndex<T>(this Collection<T> items, Predicate<T> match) => items.FindLastIndex(items.Count - 1, items.Count, match);

        public static int FindLastIndex<T>(this Collection<T> items, int startIndex, Predicate<T> match) => items.FindLastIndex(startIndex, startIndex + 1, match);

        public static int FindLastIndex<T>(this Collection<T> items, int startIndex, int count, Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            if (items.Count == 0)
            {
                // Special case for 0 length List
                if (startIndex != -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex));
                }
            }
            else
            {
                // Make sure we're not out of range            
                if (startIndex >= items.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex));
                }
            }

            // 2nd have of this also catches when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
            if (count < 0 || startIndex - count + 1 < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            int endIndex = startIndex - count;
            for (int i = startIndex; i > endIndex; i--)
            {
                if (match(items[i]))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
