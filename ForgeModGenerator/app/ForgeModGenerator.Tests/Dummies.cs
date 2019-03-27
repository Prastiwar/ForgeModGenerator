using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ForgeModGenerator.Tests
{
    public interface IDummyInterface { }

    public interface DummyInterface { }

    public class DummyClass { public void Void() { } }

    public struct DummyStruct { }

    public static class OutputHelper
    {
        public static void Output(TestContext context, string content)
            => File.WriteAllText(Path.Combine(context.TestResultsDirectory, "OutputFile.txt"), content);
    }
}
