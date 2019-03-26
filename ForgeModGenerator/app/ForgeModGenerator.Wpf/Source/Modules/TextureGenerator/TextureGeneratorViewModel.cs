using ForgeModGenerator.Services;
using ForgeModGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ForgeModGenerator.TextureGenerator.ViewModels
{
    /// <summary> TextureGenerator Business ViewModel </summary>
    public class TextureGeneratorViewModel : FoldersWatcherViewModelBase<WpfObservableFolder<FileObject>, FileObject>
    {
        public TextureGeneratorViewModel(ISessionContextService sessionContext, IDialogService dialogService, ISnackbarService snackbarService) :
            base(sessionContext, dialogService, snackbarService)
        {
            OpenFileDialog.Filter = "Image (*.png) | *.png";
            AllowedFileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png" };
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.TexturesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;
    }
}
