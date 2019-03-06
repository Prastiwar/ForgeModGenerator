using ForgeModGenerator.Services;
using ForgeModGenerator.ViewModels;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ForgeModGenerator.TextureGenerator.ViewModels
{
    /// <summary> TextureGenerator Business ViewModel </summary>
    public class TextureGeneratorViewModel : FoldersWatcherViewModelBase<ObservableFolder<FileItem>, FileItem>
    {
        public TextureGeneratorViewModel(ISessionContextService sessionContext, IDialogService dialogService) : base(sessionContext, dialogService)
        {
            OpenFileDialog.Filter = "Image (*.png) | *.png";
            AllowedFileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png" };
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.TexturesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;
    }
}
