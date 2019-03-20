//using ForgeModGenerator.Utility;
//using ForgeModGenerator.Validation;
//using System;
//using System.ComponentModel;
//using System.IO;
//using System.Linq;

//namespace ForgeModGenerator.Models
//{
//    public enum ModSide
//    {
//        ClientServer,
//        Client,
//        Server
//    }

//    public class ModModel
//    {
//        public string Organization { get; set; }
//        public ModSide Side { get; set; }
//        public WorkspaceSetup WorkspaceSetup { get; set; }
//        public McModInfo ModInfo { get; set; }
//        public ForgeVersion ForgeVersion { get; set; }
//        public LaunchSetup LaunchSetup { get; set; }
//    }

//    public class Mod : ModModel, IDirty, ICopiable<Mod>, IDataErrorInfo, IValidable<Mod>
//    {
//        //public string Organization { get; set; }
//        //public ModSide Side { get; set; }
//        //public WorkspaceSetup WorkspaceSetup { get; set; }
//        //public McModInfo ModInfo { get; set; }
//        //public ForgeVersion ForgeVersion { get; set; }
//        //public LaunchSetup LaunchSetup { get; set; }

//        internal string CachedName { get; private set; }

//        public Mod(McModInfo modInfo, string organization, ForgeVersion forgeVersion, ModSide side = ModSide.ClientServer, LaunchSetup launchSetup = null, WorkspaceSetup workspaceSetup = null)
//        {
//            ModInfo = modInfo;
//            Organization = organization;
//            ForgeVersion = forgeVersion;
//            Side = side;
//            LaunchSetup = launchSetup ?? (
//                Side == ModSide.Client
//                    ? new LaunchSetup()
//                    : Side == ModSide.Server
//                        ? new LaunchSetup(false, true)
//                        : new LaunchSetup(true, true)
//            );
//            WorkspaceSetup = workspaceSetup ?? WorkspaceSetup.NONE;
//            CachedName = ModInfo.Name;
//        }

//        /// <summary> Shorthand for ModInfo.Name (used also for WPF validation) </summary>
//        public string Name { get => ModInfo.Name; set => ModInfo.Name = value; }

//        /// <summary> Shorthand for ModInfo.Modid (used also for WPF validation) </summary>
//        public string Modid { get => ModInfo.Modid; set => ModInfo.Modid = value; }

//        public ValidateResult Validate()
//        {
//            string errorString = OnValidate(nameof(Organization));
//            if (!string.IsNullOrEmpty(errorString))
//            {
//                return new ValidateResult(false, errorString);
//            }
//            errorString = OnValidate(nameof(Modid));
//            if (!string.IsNullOrEmpty(errorString))
//            {
//                return new ValidateResult(false, errorString);
//            }
//            errorString = OnValidate(nameof(Name));
//            if (!string.IsNullOrEmpty(errorString))
//            {
//                return new ValidateResult(false, errorString);
//            }
//            return ValidateResult.Valid;
//        }

//        public event PropertyValidationEventHandler<Mod> ValidateProperty;
//        string IDataErrorInfo.Error => null;
//        string IDataErrorInfo.this[string propertyName] => OnValidate(propertyName);
//        private string OnValidate(string propertyName) => ValidateHelper.OnValidateError(ValidateProperty, this, propertyName);

//        public static string GetModidFromPath(string path)
//        {
//            path = path.NormalizePath();
//            int index = !path.Contains(":/") ? path.IndexOf(':') : -1;
//            if (index >= 1)
//            {
//                return path.Substring(0, index);
//            }
//            string modname = GetModnameFromPath(path);
//            if (modname != null)
//            {
//                string assetsPath = ModPaths.AssetsFolder(modname).NormalizePath(); // in assets folder there should be always folder with modid

//                // case if modid is already in path
//                if (assetsPath.Length < path.Length && IOHelper.IsSubPathOf(path, assetsPath))
//                {
//                    string relativePathToAssetsPath = path.Remove(0, assetsPath.Length + 1);
//                    int slashIndex = relativePathToAssetsPath.IndexOf("/", 1);
//                    return slashIndex >= 0 ? relativePathToAssetsPath.Substring(0, slashIndex) : relativePathToAssetsPath;
//                }

//                int assetsPathLength = assetsPath.Length;
//                try
//                {
//                    string directory = Directory.EnumerateDirectories(assetsPath).First();
//                    string dir = directory.NormalizePath();
//                    return dir.Remove(0, assetsPathLength + 1);
//                }
//                catch (Exception)
//                {
//                    return null;
//                }
//            }
//            return null;
//        }

//        public static string GetModnameFromPath(string path)
//        {
//            if (!IOHelper.IsPathValid(path))
//            {
//                return null;
//            }
//            path = path.NormalizePath();
//            string modsPath = AppPaths.Mods.NormalizePath();
//            int length = modsPath.Length;
//            if (!path.StartsWith(modsPath))
//            {
//                return null;
//            }
//            try
//            {
//                string sub = path.Remove(0, length + 1);
//                int index = sub.IndexOf("/");
//                return index >= 1 ? sub.Substring(0, index) : sub;
//            }
//            catch (Exception)
//            {
//                return null;
//            }
//        }

//        public bool IsDirty { get; set; }

//        public bool CopyValues(Mod fromCopy)
//        {
//            Side = fromCopy.Side;
//            ModInfo = fromCopy.ModInfo;
//            Organization = fromCopy.Organization;
//            WorkspaceSetup = fromCopy.WorkspaceSetup;
//            LaunchSetup = fromCopy.LaunchSetup;
//            ForgeVersion = fromCopy.ForgeVersion;
//            return true;
//        }

//        public Mod DeepCopy()
//        {
//            Mod clone = new Mod(ModInfo.DeepCopy(),
//                                Organization,
//                                new ForgeVersion(ForgeVersion.ZipPath),
//                                Side,
//                                new LaunchSetup(LaunchSetup.RunClient, LaunchSetup.RunServer),
//                                WorkspaceSetup);
//            return clone;
//        }

//        public Mod ShallowCopy() => (Mod)((ICloneable)this).Clone();

//        bool ICopiable.CopyValues(object fromCopy) => fromCopy is Mod copyMod ? CopyValues(copyMod) : false;
//        object ICopiable.DeepClone() => DeepCopy();
//        object ICloneable.Clone() => MemberwiseClone();
//    }
//}
