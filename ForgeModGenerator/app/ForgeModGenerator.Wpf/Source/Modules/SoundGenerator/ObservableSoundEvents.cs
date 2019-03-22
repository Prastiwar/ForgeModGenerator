using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utility;
using System.Collections.Generic;
using System.IO;

namespace ForgeModGenerator.SoundGenerator
{
    public class ObservableSoundEvents : WpfObservableFolder<SoundEvent>
    {
        public ObservableSoundEvents(string path) : base(path) { }
        public ObservableSoundEvents(IEnumerable<string> filePaths) : base(filePaths) { }
        public ObservableSoundEvents(IEnumerable<SoundEvent> files) : base(files) { }
        public ObservableSoundEvents(string path, IEnumerable<string> filePaths) : base(path, filePaths) { }
        public ObservableSoundEvents(string path, IEnumerable<SoundEvent> files) : base(path, files) { }
        public ObservableSoundEvents(string path, SearchOption searchOption) : base(path, searchOption) { }
        public ObservableSoundEvents(string path, string fileSearchPatterns) : base(path, fileSearchPatterns) { }
        public ObservableSoundEvents(string path, string fileSearchPatterns, SearchOption searchOption) : base(path, fileSearchPatterns, searchOption) { }
        protected ObservableSoundEvents() { }

        protected override bool CanAdd(SoundEvent item) => !Files.Exists(x => item.EventName == x.EventName);
        protected override bool CanAdd(string filePath) => true;
    }
}
