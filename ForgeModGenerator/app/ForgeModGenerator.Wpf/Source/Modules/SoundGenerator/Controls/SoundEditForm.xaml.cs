//using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utility;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ForgeModGenerator.SoundGenerator.Controls
{
    public partial class SoundEditForm : UserControl, IUIElement
    {
        public SoundEditForm() => InitializeComponent();

        //public IEnumerable<Sound.SoundType> SoundTypes => ReflectionHelper.GetEnumValues<Sound.SoundType>();

        public void SetDataContext(object context) => DataContext = context;
    }
}
