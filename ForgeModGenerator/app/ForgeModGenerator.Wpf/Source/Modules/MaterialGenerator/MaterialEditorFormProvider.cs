using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.MaterialGenerator.Controls;
using ForgeModGenerator.MaterialGenerator.Models;

namespace ForgeModGenerator.MaterialGenerator
{

    public class MaterialEditorFormProvider : ModelFormProvider<Material>
    {
        public MaterialEditorFormProvider(SoundEventChooseCollection soundEvents) => this.soundEvents = soundEvents;

        private readonly SoundEventChooseCollection soundEvents;

        public override IUIElement GetUIElement() => new MaterialEditForm() {
            SoundEvents = soundEvents
        };
    }
}
