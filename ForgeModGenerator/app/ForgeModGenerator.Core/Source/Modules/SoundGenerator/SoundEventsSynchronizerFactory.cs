using ForgeModGenerator.SoundGenerator.Models;
using System.ComponentModel;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundEventsSynchronizerFactory : IFolderSynchronizerFactory<SoundEvent, Sound>
    {
        public SoundEventsSynchronizerFactory(IFoldersFactory<SoundEvent, Sound> foldersFactory, ISynchronizeInvoke synchonizingObject)
        {
            this.foldersFactory = foldersFactory;
            this.synchonizingObject = synchonizingObject;
        }

        private readonly IFoldersFactory<SoundEvent, Sound> foldersFactory;
        private readonly ISynchronizeInvoke synchonizingObject;

        public IFolderSynchronizer<SoundEvent, Sound> Create() => new SoundEventsSynchronizer(synchonizingObject, null, foldersFactory);

        public IFolderSynchronizer<SoundEvent, Sound> Create(IFolderObject<SoundEvent> foldersToSync, string rootPath = null, string filters = null) =>
            new SoundEventsSynchronizer(synchonizingObject, foldersToSync, foldersFactory, rootPath, filters);
    }
}
