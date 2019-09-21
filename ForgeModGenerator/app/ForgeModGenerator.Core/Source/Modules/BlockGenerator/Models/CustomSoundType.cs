using Prism.Mvvm;

namespace ForgeModGenerator.BlockGenerator.Models
{
    public class CustomSoundType : BindableBase
    {
        public CustomSoundType()
        {
            Volume = 1.0f;
            Pitch = 1.0f;
        }

        public CustomSoundType(float volume, float pitch, string sound)
        {
            Volume = volume;
            Pitch = pitch;
            BreakSound = sound;
            StepSound = sound;
            PlaceSound = sound;
            HitSound = sound;
            FallSound = sound;
        }

        public CustomSoundType(float volume, float pitch, string breakSound, string stepSound, string placeSound, string hitSound, string fallSound)
        {
            Volume = volume;
            Pitch = pitch;
            BreakSound = breakSound;
            StepSound = stepSound;
            PlaceSound = placeSound;
            HitSound = hitSound;
            FallSound = fallSound;
        }

        private float volume;
        public float Volume {
            get => volume;
            set => SetProperty(ref volume, value);
        }

        private float pitch;
        public float Pitch {
            get => pitch;
            set => SetProperty(ref pitch, value);
        }

        private string breakSound;
        public string BreakSound {
            get => breakSound;
            set => SetProperty(ref breakSound, value);
        }

        private string stepSound;
        public string StepSound {
            get => stepSound;
            set => SetProperty(ref stepSound, value);
        }

        private string placeSound;
        public string PlaceSound {
            get => placeSound;
            set => SetProperty(ref placeSound, value);
        }

        private string hitSound;
        public string HitSound {
            get => hitSound;
            set => SetProperty(ref hitSound, value);
        }

        private string fallSound;
        public string FallSound {
            get => fallSound;
            set => SetProperty(ref fallSound, value);
        }
    }
}
