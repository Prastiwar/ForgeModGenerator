using System.Windows.Controls;

namespace ForgeModGenerator.MaterialGenerator.Controls
{
    public partial class MaterialEditForm : UserControl, IUIElement
    {
        public MaterialEditForm() => InitializeComponent();

        public void SetDataContext(object context) => DataContext = context;
    }
}
