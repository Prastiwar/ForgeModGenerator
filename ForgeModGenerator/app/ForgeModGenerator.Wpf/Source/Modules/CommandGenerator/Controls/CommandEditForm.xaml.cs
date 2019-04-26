using System.Windows.Controls;

namespace ForgeModGenerator.CommandGenerator.Controls
{
    public partial class CommandEditForm : UserControl, IUIElement
    {
        public CommandEditForm() => InitializeComponent();

        public void SetDataContext(object context) => DataContext = context;
    }
}
