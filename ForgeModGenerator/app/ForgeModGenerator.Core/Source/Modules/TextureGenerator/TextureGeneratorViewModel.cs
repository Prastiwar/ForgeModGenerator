using ForgeModGenerator.Services;
using ForgeModGenerator.ViewModels;
using Prism.Commands;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgeModGenerator.TextureGenerator.ViewModels
{
    public class TextureGeneratorViewModel : ViewModelBase
    {
        public TextureGeneratorViewModel(ISessionContextService sessionContext) : base(sessionContext) { }

        private ICommand openTexturesCommand;
        public ICommand OpenTexturesCommand => openTexturesCommand ?? (openTexturesCommand = new DelegateCommand(OpenTexturesContainer));

        public override Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                IsLoading = true;

                // Do work

                IsLoading = false;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private void OpenTexturesContainer()
        {
            if (SessionContext.IsModSelected)
            {
                string path = ModPaths.TexturesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                Process.Start(path);
            }
        }
    }
}
