using ForgeModGenerator.ItemGenerator.Controls;
using ForgeModGenerator.ItemGenerator.Models;

namespace ForgeModGenerator.ItemGenerator
{
    public class ItemModelFormProvider : ModelFormProvider<Item>
    {
        public ItemModelFormProvider()
        {
            // TODO: Inject Materials choose collection
        }

        public override IUIElement GetUIElement() => new ItemEditForm() {
            //Materials = 
        };
    }
}
