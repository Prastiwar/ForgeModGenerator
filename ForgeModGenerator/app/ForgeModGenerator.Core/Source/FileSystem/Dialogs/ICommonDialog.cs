using System;

namespace ForgeModGenerator
{
    public interface ICommonDialog
    {
        object Tag { get; set; }
        event EventHandler HelpRequest;
        void Reset();
        DialogResult ShowDialog();
    }
}
