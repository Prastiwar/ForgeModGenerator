using ForgeModGenerator.BlockGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ForgeModGenerator.BlockGenerator.Controls
{
    public partial class BlockEditForm : UserControl, IUIElement
    {
        public BlockEditForm() => InitializeComponent();

        public IEnumerable<BlockType> BlockTypes => Enum.GetValues(typeof(BlockType)).Cast<BlockType>();

        public void SetDataContext(object context) => DataContext = context;
    }
}
