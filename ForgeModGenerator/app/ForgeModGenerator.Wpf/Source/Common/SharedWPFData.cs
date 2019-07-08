using Prism.Commands;
using System.Windows.Input;

namespace ForgeModGenerator
{
    public static class SharedWPFData
    {
        public static ICommand StartProcessCommand => new DelegateCommand<string>((uri) => System.Diagnostics.Process.Start(uri));
    }
}
