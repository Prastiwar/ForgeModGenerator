using ForgeModGenerator.SoundGenerator.Models;
using System.ComponentModel;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundEventsSynchronizerFactory : IFolderSynchronizerFactory<SoundEvent, Sound>
    {
        public SoundEventsSynchronizerFactory(IFoldersFinder<SoundEvent, Sound> finder, ISynchronizeInvoke synchonizingObject, IFileSystem fileSystem)
        {
            this.finder = finder;
            this.synchonizingObject = synchonizingObject;
            this.fileSystem = fileSystem;
        }

        private readonly IFoldersFinder<SoundEvent, Sound> finder;
        private readonly ISynchronizeInvoke synchonizingObject;
        private readonly IFileSystem fileSystem;

        public IFolderSynchronizer<SoundEvent, Sound> Create() => new SoundEventsSynchronizer(synchonizingObject, null, finder, fileSystem);

        public IFolderSynchronizer<SoundEvent, Sound> Create(IFolderObject<SoundEvent> foldersToSync, string rootPath = null, string filters = null) =>
            new SoundEventsSynchronizer(synchonizingObject, foldersToSync, finder, fileSystem, rootPath, filters);
    }
}
