using ForgeModGenerator.SoundGenerator.Models;
using System.ComponentModel;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundEventsSynchronizerFactory : IFolderSynchronizerFactory<SoundEvent, Sound>
    {
        public SoundEventsSynchronizerFactory(IFoldersFinder<SoundEvent, Sound> finder, ISynchronizeInvoke synchonizingObject)
        {
            this.finder = finder;
            this.synchonizingObject = synchonizingObject;
        }

        private readonly IFoldersFinder<SoundEvent, Sound> finder;
        private readonly ISynchronizeInvoke synchonizingObject;

        public IFolderSynchronizer<SoundEvent, Sound> Create() => new SoundEventsSynchronizer(synchonizingObject, null, finder);

        public IFolderSynchronizer<SoundEvent, Sound> Create(IFolderObject<SoundEvent> foldersToSync, string rootPath = null, string filters = null) =>
            new SoundEventsSynchronizer(synchonizingObject, foldersToSync, finder, rootPath, filters);
    }
}
