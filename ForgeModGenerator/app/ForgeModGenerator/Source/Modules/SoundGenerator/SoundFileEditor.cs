using ForgeModGenerator.SoundGenerator.Controls;
using ForgeModGenerator.SoundGenerator.Models;
using GalaSoft.MvvmLight.Views;
using System.Windows;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundFileEditor : FileEditor<SoundEvent, Sound>
    {
        public SoundFileEditor(IDialogService dialogService, FrameworkElement fileEditForm) : base(dialogService, fileEditForm) { }

        protected override void OnFileEditorOpening(object sender, FileEditorOpeningDialogEventArgs eventArgs)
        {
            base.OnFileEditorOpening(sender, eventArgs);
            (FileEditForm as SoundEditForm).AllSounds = eventArgs.Folder.Files;
        }
    }
}
