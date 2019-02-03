using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using System.Windows.Forms;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> TextureGenerator Business ViewModel </summary>
    public class TextureGeneratorViewModel : FolderListViewModelBase<ObservableFolder<FileItem>, FileItem>
    {
        public TextureGeneratorViewModel(ISessionContextService sessionContext) : base(sessionContext)
        {
            OpenFileDialog.Filter = "Image (*.png) | *.png";
            AllowedExtensions = new string[] { ".png" };
            Refresh();
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.Textures(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;
    }
}
