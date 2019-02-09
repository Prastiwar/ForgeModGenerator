using System.IO;

namespace ForgeModGenerator.CodeGeneration
{
    public static class CodeOperation
    {
        public static void ReplaceStringVariableValue(string filePath, string oldVarValue, string newVarValue)
        {
            string content = File.ReadAllText(filePath);
            string newContent = content.Replace($"\"{oldVarValue}\"", $"\"{newVarValue}\"");
            File.WriteAllText(filePath, newContent);
        }

        public static void ReplaceStringValue(string filePath, string oldText, string newText)
        {
            string content = File.ReadAllText(filePath);
            string newContent = content.Replace(oldText, newText);
            File.WriteAllText(filePath, newContent);
        }
    }
}
