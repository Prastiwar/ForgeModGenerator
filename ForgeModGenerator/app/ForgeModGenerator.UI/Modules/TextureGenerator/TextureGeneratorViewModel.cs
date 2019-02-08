using ForgeModGenerator.Models;
using ForgeModGenerator.Services;
using ForgeModGenerator.ViewModels;
using System.Windows.Forms;

namespace ForgeModGenerator.TextureGenerator.ViewModels
{
    /// <summary> TextureGenerator Business ViewModel </summary>
    public class TextureGeneratorViewModel : FolderListViewModelBase<ObservableFolder<FileItem>, FileItem>
    {
        public TextureGeneratorViewModel(ISessionContextService sessionContext) : base(sessionContext)
        {
            OpenFileDialog.Filter = "Image (*.png) | *.png";
            AllowedFileExtensions = new string[] { ".png" };
            Refresh();
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.Textures(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;
    }
}
