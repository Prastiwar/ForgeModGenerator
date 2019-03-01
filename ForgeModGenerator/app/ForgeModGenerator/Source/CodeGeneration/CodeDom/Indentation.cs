using System.CodeDom.Compiler;
using System.Text;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public class Indentation
    {
        public Indentation(IndentedTextWriter writer, int indent)
        {
            this.writer = writer;
            this.indent = indent;
            indentString = null;
        }

        private readonly IndentedTextWriter writer;
        private readonly int indent;

        private string indentString;
        public string IndentationString {
            get {
                if (indentString == null)
                {
                    string tabString = IndentedTextWriter.DefaultTabString;
                    StringBuilder sb = new StringBuilder(indent * tabString.Length);
                    for (int i = 0; i < indent; i++)
                    {
                        sb.Append(tabString);
                    }
                    indentString = sb.ToString();
                }
                return indentString;
            }
        }
    }
}
