using System;

namespace ForgeModGenerator
{
    public interface IFolderBrowser : ICommonDialog
    {
        Environment.SpecialFolder RootFolder { get; set; }
        bool ShowNewFolderButton { get; set; }
        string SelectedPath { get; set; }
        string Description { get; set; }
    }
}
