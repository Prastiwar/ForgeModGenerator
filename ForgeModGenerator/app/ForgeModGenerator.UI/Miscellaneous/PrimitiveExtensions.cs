using Newtonsoft.Json;
using System;
using System.Text;

namespace ForgeModGenerator.Miscellaneous
{
    public static class PrimitiveExtensions
    {
        private static readonly StringBuilder textBuilder = new StringBuilder(32);

        public static string FormatJson(this string json, Formatting format) => JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), format);

        public static bool HasProperty(this Type obj, string propertyName) => obj.GetProperty(propertyName) != null;

        public static string Replace(this string text, string oldString, string newString, int count)
        {
            textBuilder.Clear();
            textBuilder.Append(text);
            int index = 0;
            for (int i = 0; (i < count && index >= 0); i++)
            {
                index = textBuilder.IndexOf(oldString);
                textBuilder.Remove(index, oldString.Length).Insert(index, newString);
            }
            return textBuilder.ToString();
        }

        public static StringBuilder ReplaceN(this StringBuilder sb, string oldString, string newString, int count)
        {
            textBuilder.Clear();
            textBuilder.Append(sb.ToString());
            int index = 0;
            for (int i = 0; (i < count && index >= 0); i++)
            {
                index = textBuilder.IndexOf(oldString);
                textBuilder.Remove(index, oldString.Length).Insert(index, newString);
            }
            sb.Clear();
            sb.Append(textBuilder);
            return sb;
        }

        public static int IndexOf(this StringBuilder sb, string value, int startIndex = 0, bool ignoreCase = false)
        {
            // Solution from: https://stackoverflow.com/a/6601226
            int index;
            int length = value.Length;
            int maxSearchLength = (sb.Length - length) + 1;

            if (ignoreCase)
            {
                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (char.ToLower(sb[i]) == char.ToLower(value[0]))
                    {
                        index = 1;
                        while ((index < length) && (char.ToLower(sb[i + index]) == char.ToLower(value[index])))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }
                return -1;
            }

            for (int i = startIndex; i < maxSearchLength; ++i)
            {
                if (sb[i] == value[0])
                {
                    index = 1;
                    while ((index < length) && (sb[i + index] == value[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }
            return -1;
        }
    }
}
