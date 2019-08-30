using System;
using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public class CodePrimitiveArrayCreateExpression : CodeExpression
    {
        public CodePrimitiveArrayCreateExpression(int[] array) => Array = array ?? throw new ArgumentNullException(nameof(array));
        public CodePrimitiveArrayCreateExpression(string[] array) => Array = array ?? throw new ArgumentNullException(nameof(array));
        public CodePrimitiveArrayCreateExpression(double[] array) => Array = array ?? throw new ArgumentNullException(nameof(array));
        public CodePrimitiveArrayCreateExpression(float[] array) => Array = array ?? throw new ArgumentNullException(nameof(array));
        public CodePrimitiveArrayCreateExpression(long[] array) => Array = array ?? throw new ArgumentNullException(nameof(array));
        public CodePrimitiveArrayCreateExpression(short[] array) => Array = array ?? throw new ArgumentNullException(nameof(array));
        public CodePrimitiveArrayCreateExpression(bool[] array) => Array = array ?? throw new ArgumentNullException(nameof(array));
        public CodePrimitiveArrayCreateExpression(byte[] array) => Array = array ?? throw new ArgumentNullException(nameof(array));
        public CodePrimitiveArrayCreateExpression(char[] array) => Array = array ?? throw new ArgumentNullException(nameof(array));

        public object Array { get; set; }
    }
}
