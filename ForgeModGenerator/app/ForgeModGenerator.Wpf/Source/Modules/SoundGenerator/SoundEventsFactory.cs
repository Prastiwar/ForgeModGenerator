using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundEventsFactory : WpfFoldersFactory<SoundEvent, Sound>
    {
        public IFolderObject<SoundEvent> SoundEvents { get; set; }

        /// <inheritdoc/>
        public override SoundEvent Create(string path, IEnumerable<string> filePaths)
        {
            SoundEvent soundEvent = base.Create(path, filePaths);
            soundEvent.EventName = IOHelper.GetUniqueName(soundEvent.EventName, (name) => SoundEvents.Files.All(inFolder => inFolder.EventName != name));
            return soundEvent;
        }
    }
}
