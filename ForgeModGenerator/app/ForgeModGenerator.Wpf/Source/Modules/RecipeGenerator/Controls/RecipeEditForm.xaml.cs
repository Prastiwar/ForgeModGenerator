using System.Windows.Controls;

namespace ForgeModGenerator.RecipeGenerator.Controls
{
    public partial class RecipeEditForm : UserControl, IUIElement
    {
        public RecipeEditForm() => InitializeComponent();

        public void SetDataContext(object context) => DataContext = context;
    }
}
