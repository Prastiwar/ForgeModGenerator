using ForgeModGenerator.Core;
using ForgeModGenerator.Service;
using System.Windows.Forms;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> TextureGenerator Business ViewModel </summary>
    public class TextureGeneratorViewModel : FileListViewModelBase
    {
        public TextureGeneratorViewModel(ISessionContextService sessionContext) : base(sessionContext)
        {
            OpenFileDialog.Filter = "Image (*.png) | *.png";
            Refresh();
        }

        public override string CollectionRootPath => SessionContext.SelectedMod != null ? ModPaths.Textures(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;
    }
}
