using ForgeModGenerator.SoundGenerator.Models;

namespace ForgeModGenerator.SoundGenerator
{
    public interface ISoundEventsFactory : IFoldersFactory<SoundEvent, Sound>
    {
        IFolderObject<SoundEvent> SoundEventsRepository { get; set; }
    }
}
