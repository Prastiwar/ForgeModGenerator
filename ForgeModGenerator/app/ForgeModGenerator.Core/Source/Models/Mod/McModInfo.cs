//using System;
//using System.Collections.ObjectModel;

//namespace ForgeModGenerator.Models
//{
//    // struct for mcmod.info file
//    public class McModInfo : ICopiable<McModInfo>
//    {
//        public string Modid { get; set; }
//        public string Name { get; set; }
//        public string Description { get; set; }
//        public string Version { get; set; }
//        public string McVersion { get; set; }
//        public string Url { get; set; }
//        public string UpdateUrl { get; set; }
//        public string Credits { get; set; }
//        public string LogoFile { get; set; }

//        public ObservableCollection<string> AuthorList { get; set; }
//        public ObservableCollection<string> Screenshots { get; set; }
//        public ObservableCollection<string> Dependencies { get; set; }

//        public bool CopyValues(McModInfo fromCopy)
//        {
//            AuthorList = fromCopy.AuthorList;
//            Credits = fromCopy.Credits;
//            Dependencies = fromCopy.Dependencies;
//            Description = fromCopy.Description;
//            LogoFile = fromCopy.LogoFile;
//            McVersion = fromCopy.McVersion;
//            Modid = fromCopy.Modid;
//            Name = fromCopy.Name;
//            Screenshots = fromCopy.Screenshots;
//            UpdateUrl = fromCopy.UpdateUrl;
//            Url = fromCopy.Url;
//            Version = fromCopy.Version;
//            return true;
//        }

//        public McModInfo DeepCopy()
//        {
//            McModInfo clone = new McModInfo() {
//                AuthorList = AuthorList,
//                Credits = Credits,
//                Dependencies = Dependencies,
//                Description = Description,
//                LogoFile = LogoFile,
//                McVersion = McVersion,
//                Modid = Modid,
//                Name = Name,
//                Screenshots = Screenshots,
//                UpdateUrl = UpdateUrl,
//                Url = Url,
//                Version = Version
//            };
//            return clone;
//        }

//        public McModInfo ShallowCopy() => (McModInfo)((ICloneable)this).Clone();

//        bool ICopiable.CopyValues(object fromCopy) => fromCopy is McModInfo info ? CopyValues(info) : false;
//        object ICopiable.DeepClone() => DeepCopy();
//        object ICloneable.Clone() => MemberwiseClone();
//    }
//}
