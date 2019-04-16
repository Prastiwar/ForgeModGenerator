using System.IO;

namespace ForgeModGenerator.CodeGeneration
{
    /// <summary> Holds information about class name, file path and namespace </summary>
    public class ClassLocator
    {
        public ClassLocator(string importFullName)
        {
            ImportFullName = importFullName;
            int organizationDotIndex = ImportFullName.IndexOf('.');
            int modnameDotIndex = ImportFullName.IndexOf('.', organizationDotIndex + 1);
            int nextDotIndex = ImportFullName.IndexOf('.', modnameDotIndex + 1);
            string organization = ImportFullName.Substring(organizationDotIndex + 1, modnameDotIndex - organizationDotIndex - 1);
            string modname = ImportFullName.Substring(modnameDotIndex + 1, nextDotIndex - modnameDotIndex - 1).ToLower();

            ImportRelativeName = ImportFullName.Substring(nextDotIndex + 1, ImportFullName.Length - nextDotIndex - 1);

            RelativePath = ImportRelativeName.Replace('.', '/') + ".java";
            FullPath = Path.Combine(ModPaths.SourceCodeRootFolder(modname, organization), RelativePath);

            int lastRelativeDotIndex = ImportRelativeName.LastIndexOf('.');
            ClassName = ImportRelativeName.Substring(lastRelativeDotIndex + 1, ImportRelativeName.Length - lastRelativeDotIndex - 1);
            int lastDotIndex = ImportFullName.LastIndexOf('.');
            PackageName = ImportFullName.Substring(0, lastDotIndex).ToLower();
        }

        public string PackageName { get; }

        /// <summary> Class and file Name </summary>
        public string ClassName { get; }

        /// <summary> Relative file path to ./com/organization/projectname/RelativePath. </summary>
        public string RelativePath { get; }

        /// <summary> Full path to class file </summary>
        public string FullPath { get; }

        /// <summary> Full relative import path to(com.organization.modname.RelativeName </summary>
        public string ImportRelativeName { get; }

        /// <summary> Full import path = com.organization.modname.RelativePath.ClassName </summary>
        public string ImportFullName { get; }

        public static string CombineImport(params string[] strings) => string.Join(".", strings);
    }

    public class InitClassLocator : ClassLocator
    {
        public InitClassLocator(string importFullName, string initFieldName) : base(importFullName) => InitFieldName = initFieldName;

        /// <summary> Name of field (list) that holds a references to init types </summary>
        public string InitFieldName { get; }
    }
}
