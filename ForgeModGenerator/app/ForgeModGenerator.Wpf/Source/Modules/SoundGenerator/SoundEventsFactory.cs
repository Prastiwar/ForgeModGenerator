//using ForgeModGenerator.SoundGenerator.Models;
//using ForgeModGenerator.Utility;
//using System.Collections.Generic;
//using System.Linq;

//namespace ForgeModGenerator.SoundGenerator
//{
//    public class SoundEventsFactory : WpfFoldersFactory<SoundEvent, Sound>, ISoundEventsFactory
//    {
//        public IFolderObject<SoundEvent> SoundEventsRepository { get; set; }

//        public override IFolderObject<SoundEvent> CreateFolders() => ReflectionHelper.CreateInstance<ObservableSoundEvents>(true);

//        /// <inheritdoc/>
//        public override SoundEvent Create(string path, IEnumerable<string> filePaths)
//        {
//            SoundEvent soundEvent = base.Create(path, filePaths);
//            soundEvent.EventName = IOHelper.GetUniqueName(soundEvent.EventName, (name) => SoundEventsRepository.Files.All(inFolder => inFolder.EventName != name));
//            return soundEvent;
//        }
//    }
//}
