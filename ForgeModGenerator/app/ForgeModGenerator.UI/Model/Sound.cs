using GalaSoft.MvvmLight;

namespace ForgeModGenerator.Model
{
    public class Sound : ObservableObject
    {
        private string name;
        public string Name {
            get => name;
            set => Set(ref name, value);
        }
    }
}
