﻿using ForgeModGenerator.Utility;

namespace ForgeModGenerator
{
    public class WpfFoldersFactory<TFolder, TFile> : FoldersFactory<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        public override IFolderObject<TFolder> CreateFolders() => ReflectionHelper.CreateInstance<WpfObservableFolder<TFolder>>(true);
    }
}
