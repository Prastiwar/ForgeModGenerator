using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.MaterialGenerator.Controls;
using ForgeModGenerator.MaterialGenerator.Models;

namespace ForgeModGenerator.MaterialGenerator
{

    public class MaterialModelFormProvider : ModelFormProvider<Material>
    {
        public MaterialModelFormProvider(SoundEventChooseCollection soundEvents) => this.soundEvents = soundEvents;

        private readonly SoundEventChooseCollection soundEvents;

        public override IUIElement GetUIElement() => new MaterialEditForm() {
            SoundEvents = soundEvents
        };
    }
}
