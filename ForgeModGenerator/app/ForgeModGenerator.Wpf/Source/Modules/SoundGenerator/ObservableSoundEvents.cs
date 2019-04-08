using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utility;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator
{
    public class ObservableSoundEvents : WpfObservableFolder<SoundEvent>
    {
        public ObservableSoundEvents(string path) : base(path) { }
        public ObservableSoundEvents(IEnumerable<string> filePaths) : base(filePaths) { }
        public ObservableSoundEvents(IEnumerable<SoundEvent> files) : base(files) { }
        public ObservableSoundEvents(string path, IEnumerable<string> filePaths) : base(path, filePaths) { }
        public ObservableSoundEvents(string path, IEnumerable<SoundEvent> files) : base(path, files) { }
        protected ObservableSoundEvents() { }

        protected override bool CanAdd(SoundEvent item) => !Files.Exists(x => item.EventName == x.EventName);
        protected override bool CanAdd(string filePath) => true;
    }
}
