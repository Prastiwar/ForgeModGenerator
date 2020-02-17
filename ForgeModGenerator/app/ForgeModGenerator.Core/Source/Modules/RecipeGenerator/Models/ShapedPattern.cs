using ForgeModGenerator.Utility;
using System;
using System.Linq;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class ShapedPattern
    {
        protected string[] CharPattern { get; } = new string[3] { "   ", "   ", "   " };

        public bool IsEmpty => CharPattern.Any(x => !string.IsNullOrEmpty(x.Trim()));

        public string[] GetPattern()
        {
            string[] newPattern = new string[CharPattern.Length];
            Array.Copy(CharPattern, newPattern, newPattern.Length);
            return newPattern;
        }

        public char GetKey(int row, int column) => CharPattern[row][column];

        public void Set(ShapedPattern pattern) => Array.Copy(pattern.CharPattern, CharPattern, CharPattern.Length);

        public void Set(string[] pattern)
        {
            if (pattern.Length > 3 || pattern.Any(row => row.Length > 3))
            {
                throw new ArgumentException("Pattern layout is invalid", nameof(pattern));
            }
            Array.Copy(pattern, CharPattern, pattern.Length);
        }

        public void Set(int row, int column, char key)
        {
            if (row < 0 || row > 2)
            {
                throw new IndexOutOfRangeException(nameof(row));
            }
            if (column < 0 || column > 2)
            {
                throw new IndexOutOfRangeException(nameof(column));
            }
            CharPattern[row] = CharPattern[row].SetCharAt(column, key);
        }
    }
}
