using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public class CodeContinueLabelStatement : CodeStatement
    {
        public CodeContinueLabelStatement(string label) => Label = label;

        private string label;
        public string Label {
            get => label;
            set => label = !string.IsNullOrEmpty(value) ? value : throw new System.ArgumentNullException(nameof(value));
        }
    }
}

