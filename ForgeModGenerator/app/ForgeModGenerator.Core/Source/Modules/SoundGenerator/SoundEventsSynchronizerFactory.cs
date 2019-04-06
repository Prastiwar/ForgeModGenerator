using ForgeModGenerator.SoundGenerator.Models;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundEventsSynchronizerFactory : IFolderSynchronizerFactory<SoundEvent, Sound>
    {
        public SoundEventsSynchronizerFactory(IFoldersFactory<SoundEvent, Sound> foldersFactory) => this.foldersFactory = foldersFactory;

        private readonly IFoldersFactory<SoundEvent, Sound> foldersFactory;

        public IFolderSynchronizer<SoundEvent, Sound> Create() => new SoundEventsSynchronizer(null, null, foldersFactory);

        public IFolderSynchronizer<SoundEvent, Sound> Create(IFolderObject<SoundEvent> foldersToSync, string rootPath = null, string filters = null) =>
            new SoundEventsSynchronizer(null, foldersToSync, foldersFactory, rootPath, filters);
    }
}
