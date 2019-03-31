using System;

namespace ForgeModGenerator
{
    public interface IFolderBrowser : ICommonDialog
    {
        bool ShowNewFolderButton { get; set; }
        string SelectedPath { get; set; }
        Environment.SpecialFolder RootFolder { get; set; }
        string Description { get; set; }
    }
}
