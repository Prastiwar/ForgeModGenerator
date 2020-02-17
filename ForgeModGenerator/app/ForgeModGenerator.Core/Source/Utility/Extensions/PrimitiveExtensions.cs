using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ForgeModGenerator.Utility
{
    public static class PrimitiveExtensions
    {
        private static readonly StringBuilder output = new StringBuilder(32);
        private static readonly Regex spaceRemover = new Regex(@"\s+", RegexOptions.Compiled);

        public static string SetCharAt(this string text, int index, char character)
        {
            char[] chars = text.ToCharArray();
            chars[index] = character;
            return new string(chars);
        }

        /// <summary> Removes character on the end of text. Useful in decimals string </summary>
        public static string RemoveEnding(this string text) => char.IsDigit(text[text.Length - 1]) ? text : text.Remove(text.Length - 1, 1);

        public static string RemoveAllSpaces(this string text) => spaceRemover.Replace(text, string.Empty);

        public static string Replace(this string text, string oldString, string newString, int count)
        {
            output.Clear();
            output.Append(text);
            int index = 0;
            for (int i = 0; (i < count && index >= 0); i++)
            {
                index = output.IndexOf(oldString);
                output.Remove(index, oldString.Length).Insert(index, newString);
            }
            return output.ToString();
        }

        public static StringBuilder ReplaceN(this StringBuilder sb, string oldString, string newString, int count)
        {
            output.Clear();
            output.Append(sb.ToString());
            int index = 0;
            for (int i = 0; (i < count && index >= 0); i++)
            {
                index = output.IndexOf(oldString);
                output.Remove(index, oldString.Length).Insert(index, newString);
            }
            sb.Clear();
            sb.Append(output);
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
                        {
                            ++index;
                        }

                        if (index == length)
                        {
                            return i;
                        }
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
                    {
                        ++index;
                    }

                    if (index == length)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary> Returns string value between { and } </summary>
        public static string GetBracesContent(this string expression) => expression.GetSeparatedContent('{', '}');

        /// <summary> Returns string value between [ and ] </summary>
        public static string GetBracketsContent(this string expression) => expression.GetSeparatedContent('[', ']');

        /// <summary> Returns string value between ( and ) </summary>
        public static string GetParenthesesContent(this string expression) => expression.GetSeparatedContent('(', ')');

        public static IEnumerable<string> SplitSeparatedContent(this string expression, char startSeparationChar, char endSeperationChar)
        {
            string content = GetSeparatedContent(expression, startSeparationChar, endSeperationChar);
            foreach (string item in content.Split(','))
            {
                yield return item;
            }
        }

        /// <summary> Returns string value between startSeparationChar and endSeperationChar </summary>
        public static string GetSeparatedContent(this string expression, char startSeparationChar, char endSeperationChar)
        {
            int startIndex = expression.IndexOf(startSeparationChar);
            if (startIndex < 0)
            {
                return "";
            }
            int openingCount = 0;
            int endingCount = 0;
            for (int endIndex = startIndex; endIndex < expression.Length; endIndex++)
            {
                if (expression[endIndex] == startSeparationChar)
                {
                    openingCount++;
                }
                else if (expression[endIndex] == endSeperationChar)
                {
                    endingCount++;
                    if (endingCount >= openingCount)
                    {
                        return expression.Substring(startIndex + 1, endIndex - startIndex - 1);
                    }
                }
            }
            return "";
        }

        public static string[] SplitTrim(this string value, char separator)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Array.Empty<string>();
            }
            string[] split = value.Split(separator);
            for (int i = 0; i < split.Length; i++)
            {
                split[i] = split[i].Trim();
            }
            return split;
        }

        public static IEnumerable<string> LazySplit(this string value, char separator)
        {
            int length = value.Length;
            int separatorIndex = value.IndexOf(separator, 0, length);
            if (separatorIndex == -1)
            {
                yield return value;
                yield break;
            }

            int i = 0;
            while (separatorIndex != -1)
            {
                bool isNotEmpty = (separatorIndex - i > 0);
                if (isNotEmpty)
                {
                    yield return value.Substring(i, separatorIndex - i); // Return non-empty match
                }
                i = separatorIndex + 1;
                separatorIndex = value.IndexOf(separator, i, length - i);
            }

            if (i < length) // Has remainder?
            {
                yield return value.Substring(i, length - i); // Return remaining trail
            }
        }
    }
}
