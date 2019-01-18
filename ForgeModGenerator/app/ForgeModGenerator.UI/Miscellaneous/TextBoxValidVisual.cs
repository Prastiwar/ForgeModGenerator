using System.Windows.Controls;
using System.Windows.Media;

namespace ForgeModGenerator.Miscellaneous
{
    public class TextBoxValidVisual
    {
        private readonly Brush normalBackground;
        private readonly TextBox box;

        public TextBoxValidVisual(TextBox box)
        {
            this.box = box;
            normalBackground = box.Background;
        }

        public void SetValid(bool valid)
        {
            if (!valid)
            {
                if (box.Background != Brushes.IndianRed)
                {
                    box.Background = Brushes.DarkRed;
                }
            }
            else
            {
                if (box.Background != normalBackground)
                {
                    box.Background = normalBackground;
                }
            }
        }
    }
}
