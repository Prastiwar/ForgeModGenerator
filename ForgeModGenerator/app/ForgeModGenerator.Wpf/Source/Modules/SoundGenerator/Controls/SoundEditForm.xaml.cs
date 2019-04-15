﻿using ForgeModGenerator.SoundGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ForgeModGenerator.SoundGenerator.Controls
{
    public partial class SoundEditForm : UserControl, IUIElement
    {
        public SoundEditForm() => InitializeComponent();

        public IEnumerable<Sound.SoundType> SoundTypes => Enum.GetValues(typeof(Sound.SoundType)).Cast<Sound.SoundType>();

        public void SetDataContext(object context) => DataContext = context;
    }
}
